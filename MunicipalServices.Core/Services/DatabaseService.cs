using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MunicipalServices.Models;
using Newtonsoft.Json;
using System.Data;
using MunicipalServices.Core.Utilities;

namespace MunicipalServices.Core.Services
{
    public class DatabaseService
    {
        private const string DATABASE_NAME = "municipal_services.db";
        private readonly string _connectionString;
        private static DatabaseService instance;

        private readonly List<dynamic> testLocations = new List<dynamic>
        {
            new { Search = "Cape Town City Hall", Category = "Building Maintenance", Description = "Structural damage to historic facade", Priority = 7 },
            new { Search = "V&A Waterfront, Cape Town", Category = "Public Safety", Description = "Broken security camera at main entrance", Priority = 8 },
            new { Search = "Table Mountain National Park", Category = "Environmental Issues", Description = "Illegal dumping near hiking trail", Priority = 6 },
            new { Search = "Green Point Park", Category = "Parks", Description = "Damaged playground equipment needs repair", Priority = 5 },
            new { Search = "Long Street, Cape Town", Category = "Road Maintenance", Description = "Large pothole causing traffic issues", Priority = 7 }
        };

        public static DatabaseService Instance
        {
            get
            {
                if (instance == null)
                    instance = new DatabaseService();
                return instance;
            }
        }

        private DatabaseService()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATABASE_NAME);
            _connectionString = $"Data Source={dbPath};Version=3;";
            
            // Only create tables if they don't exist
            CreateTables();
            
            // Add logging to check database state
            using (var connection = CreateConnection())
            {
                var requestCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ServiceRequests");
                Logger.LogInfo($"Database initialized with {requestCount} existing requests");
            }
        }

        public List<Attachment> GetAttachmentsForRequest(string requestId)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT AttachmentId, RequestId, Name, Content FROM Attachments WHERE RequestId = @RequestId";
                    command.Parameters.AddWithValue("@RequestId", requestId);

                    var attachments = new List<Attachment>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var attachment = new Attachment
                            {
                                RequestId = reader.GetString(1),
                                Name = reader.GetString(2),
                                Content = (byte[])reader.GetValue(3)
                            };
                            attachments.Add(attachment);
                        }
                    }
                    return attachments;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting attachments for request {requestId}: {ex.Message}");
                return new List<Attachment>();
            }
        }

        public IDbConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                // First ensure tables exist
                CreateTables();

                using (var connection = CreateConnection())
                {
                    var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ServiceRequests");
                    Logger.LogInfo($"Number of service requests after initialization: {count}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error during database initialization: {ex.Message}");
                throw;
            }
        }

        private void CreateTables()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Create Users table
                        connection.Execute(@"
                            CREATE TABLE IF NOT EXISTS Users (
                                UserId TEXT PRIMARY KEY,
                                Username TEXT NOT NULL,
                                PasswordHash TEXT NOT NULL,
                                Role TEXT NOT NULL
                            )", transaction: transaction);

                        // Create ServiceRequests table with new columns
                        connection.Execute(@"
                            CREATE TABLE IF NOT EXISTS ServiceRequests (
                                RequestId TEXT PRIMARY KEY,
                                UserId TEXT NOT NULL,
                                Location TEXT NOT NULL,
                                Category TEXT NOT NULL,
                                Description TEXT NOT NULL,
                                Priority INTEGER NOT NULL,
                                Status TEXT NOT NULL,
                                CreatedDate TEXT NOT NULL,
                                Latitude REAL DEFAULT 0,
                                Longitude REAL DEFAULT 0,
                                FormattedAddress TEXT,
                                FOREIGN KEY(UserId) REFERENCES Users(UserId)
                            )", transaction: transaction);

                        // Create Attachments table
                        connection.Execute(@"
                            CREATE TABLE IF NOT EXISTS Attachments (
                                AttachmentId INTEGER PRIMARY KEY AUTOINCREMENT,
                                RequestId TEXT NOT NULL,
                                Name TEXT NOT NULL,
                                Content BLOB NOT NULL,
                                FOREIGN KEY(RequestId) REFERENCES ServiceRequests(RequestId)
                            )", transaction: transaction);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void SaveServiceRequest(ServiceRequest request, string userId, List<Attachment> attachments)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Save the service request
                        connection.Execute(@"
                    INSERT INTO ServiceRequests 
                    (RequestId, UserId, Location, Category, Description, Priority, Status, CreatedDate)
                    VALUES 
                    (@RequestId, @UserId, @Location, @Category, @Description, @Priority, @Status, @CreatedDate)",
                            new
                            {
                                request.RequestId,
                                UserId = userId,
                                request.Location,
                                request.Category,
                                request.Description,
                                request.Priority,
                                Status = "Pending",
                                CreatedDate = DateTime.Now.ToString("s")
                            }, transaction);

                        // Save attachments
                        if (attachments != null && attachments.Any())
                        {
                            foreach (var attachment in attachments)
                            {
                                connection.Execute(@"
                                    INSERT INTO Attachments (RequestId, Name, Content)
                                    VALUES (@RequestId, @Name, @Content)",
                                    new
                                    {
                                        request.RequestId,
                                        attachment.Name,
                                        Content = attachment.Content
                                    }, transaction);
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public IEnumerable<Attachment> GetAllAttachments()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    return connection.Query<Attachment>("SELECT a.RequestId, a.Name, a.Content FROM Attachments a");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading attachments: {ex.Message}");
                return Enumerable.Empty<Attachment>();
            }
        }

        public List<ServiceRequest> GetAllRequests()
        {
            using (var connection = CreateConnection())
            {
                return connection.Query<ServiceRequest>(
                    "SELECT * FROM ServiceRequests ORDER BY CreatedDate DESC"
                ).AsList();
            }
        }

        public void LogNotification(string userId, EmergencyNotice notice)
        {
            // Implementation for logging notifications to the database
            Console.WriteLine($"Logged notification for user {userId}: {notice.Title}");
            // TODO: Add actual database logging implementation
        }
    }
}
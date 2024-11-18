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
        private SQLiteConnection _connection;

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
            
            // Delete existing database file if it exists
            //if (File.Exists(dbPath))
            //{
            //    try
            //    {
            //        File.Delete(dbPath);
            //        Logger.LogInfo("Deleted existing database file for clean initialization");
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.LogError($"Failed to delete existing database: {ex.Message}");
            //    }
            //}
            
            _connectionString = $"Data Source={dbPath};Version=3;";
            
            // Create fresh tables
            //CreateTables();
            
            // Log initialization
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
                using (var connection = CreateConnection())
                {
                    return connection.Query<Attachment>(@"
                        SELECT RequestId, Name, ContentBase64 
                        FROM Attachments 
                        WHERE RequestId = @RequestId",
                        new { RequestId = requestId })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting attachments for request {requestId}: {ex.Message}");
                return new List<Attachment>();
            }
        }

        public SQLiteConnection CreateConnection()
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
                        // Create ServiceRequests table
                        connection.Execute(@"
                            CREATE TABLE IF NOT EXISTS ServiceRequests (
                                RequestId TEXT PRIMARY KEY,
                                UserId TEXT,
                                Location TEXT,
                                Category TEXT,
                                Description TEXT,
                                Priority INTEGER,
                                Status TEXT,
                                CreatedDate TEXT,
                                Latitude REAL,
                                Longitude REAL,
                                FormattedAddress TEXT
                            )", transaction: transaction);

                        // Create RelatedIssues table with proper constraints
                        connection.Execute(@"
                            CREATE TABLE IF NOT EXISTS RelatedIssues (
                                RequestId TEXT,
                                RelatedRequestId TEXT,
                                PRIMARY KEY (RequestId, RelatedRequestId),
                                FOREIGN KEY(RequestId) REFERENCES ServiceRequests(RequestId) ON DELETE CASCADE,
                                FOREIGN KEY(RelatedRequestId) REFERENCES ServiceRequests(RequestId) ON DELETE CASCADE
                            )", transaction: transaction);

                        // Create Attachments table with proper constraints
                        connection.Execute(@"
                            CREATE TABLE IF NOT EXISTS Attachments (
                                RequestId TEXT,
                                Name TEXT,
                                ContentBase64 TEXT,
                                PRIMARY KEY (RequestId, Name),
                                FOREIGN KEY(RequestId) REFERENCES ServiceRequests(RequestId) ON DELETE CASCADE
                            )", transaction: transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error creating database tables: {ex.Message}", ex);
                    }
                }
            }
        }

        public void SaveServiceRequest(ServiceRequest request, string userId, List<Attachment> attachments)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Save the service request
                        connection.Execute(@"
                            INSERT INTO ServiceRequests 
                            (RequestId, UserId, Location, Category, Description, Priority, Status, CreatedDate, Latitude, Longitude, FormattedAddress)
                            VALUES 
                            (@RequestId, @UserId, @Location, @Category, @Description, @Priority, @Status, @CreatedDate, @Latitude, @Longitude, @FormattedAddress)",
                            new
                            {
                                request.RequestId,
                                UserId = userId,
                                request.Location,
                                request.Category,
                                request.Description,
                                request.Priority,
                                Status = "Pending",
                                CreatedDate = DateTime.Now.ToString("s"),
                                request.Latitude,
                                request.Longitude,
                                request.FormattedAddress
                            }, transaction);

                        // Save attachments
                        if (attachments != null && attachments.Any())
                        {
                            foreach (var attachment in attachments)
                            {
                                connection.Execute(@"
                                    INSERT INTO Attachments (RequestId, Name, ContentBase64)
                                    VALUES (@RequestId, @Name, @ContentBase64)",
                                    new
                                    {
                                        request.RequestId,
                                        attachment.Name,
                                        attachment.ContentBase64
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

        private void SaveAttachment(SQLiteConnection connection, Attachment attachment)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Attachments (RequestId, Name, ContentBase64)
                    VALUES (@RequestId, @Name, @ContentBase64)";

                command.Parameters.AddWithValue("@RequestId", attachment.RequestId);
                command.Parameters.AddWithValue("@Name", attachment.Name);
                command.Parameters.AddWithValue("@ContentBase64", attachment.ContentBase64);

                command.ExecuteNonQuery();
            }
        }

        public void SaveAttachment(Attachment attachment)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                SaveAttachment(connection, attachment);
            }
        }

        public void SaveRelatedIssues(string requestId, IEnumerable<string> relatedRequestIds)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Clear existing relations
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "DELETE FROM RelatedIssues WHERE RequestId = @RequestId";
                            command.Parameters.AddWithValue("@RequestId", requestId);
                            command.ExecuteNonQuery();
                        }

                        // Add new relations
                        if (relatedRequestIds != null && relatedRequestIds.Any())
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = "INSERT INTO RelatedIssues (RequestId, RelatedRequestId) VALUES (@RequestId, @RelatedId)";
                                
                                var requestIdParam = command.Parameters.Add("@RequestId", DbType.String);
                                var relatedIdParam = command.Parameters.Add("@RelatedId", DbType.String);
                                
                                foreach (var relatedId in relatedRequestIds)
                                {
                                    requestIdParam.Value = requestId;
                                    relatedIdParam.Value = relatedId;
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException($"Failed to save related issues: {ex.Message}", ex);
                    }
                }
            }
        }

        public List<ServiceRequest> GetRelatedIssues(string requestId)
        {
            using (var connection = CreateConnection())
            {
                return connection.Query<ServiceRequest>(@"
                    SELECT sr.* 
                    FROM ServiceRequests sr
                    INNER JOIN RelatedIssues ri ON sr.RequestId = ri.RelatedRequestId
                    WHERE ri.RequestId = @RequestId",
                    new { RequestId = requestId })
                    .ToList();
            }
        }

        public IDbTransaction BeginTransaction()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SQLiteConnection(_connectionString);
                _connection.Open();
            }
            return _connection.BeginTransaction();
        }

        public void SaveServiceRequestWithTransaction(SQLiteConnection connection, ServiceRequest request, string userId, IEnumerable<Attachment> attachments)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT OR REPLACE INTO ServiceRequests 
                    (RequestId, Category, Description, Location, Status, UserId, CreatedDate, Priority, Latitude, Longitude, FormattedAddress) 
                    VALUES 
                    (@RequestId, @Category, @Description, @Location, @Status, @UserId, @CreatedDate, @Priority, @Latitude, @Longitude, @FormattedAddress)";

                command.Parameters.AddWithValue("@RequestId", request.RequestId);
                command.Parameters.AddWithValue("@Category", request.Category);
                command.Parameters.AddWithValue("@Description", request.Description);
                command.Parameters.AddWithValue("@Location", request.Location);
                command.Parameters.AddWithValue("@Status", request.Status);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@CreatedDate", request.SubmissionDate.ToString("s"));
                command.Parameters.AddWithValue("@Priority", request.Priority);
                command.Parameters.AddWithValue("@Latitude", request.Latitude);
                command.Parameters.AddWithValue("@Longitude", request.Longitude);
                command.Parameters.AddWithValue("@FormattedAddress", request.FormattedAddress ?? string.Empty);

                command.ExecuteNonQuery();

                // Save attachments if any
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        SaveAttachment(connection, attachment);
                    }
                }
            }
        }

        public string ConnectionString => _connectionString;

        public void SaveServiceRequestWithTransaction(SQLiteConnection connection, ServiceRequest request)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO ServiceRequests 
                    (RequestId, UserId, Location, Category, Description, Priority, Status, CreatedDate, Latitude, Longitude, FormattedAddress)
                    VALUES 
                    (@RequestId, @UserId, @Location, @Category, @Description, @Priority, @Status, @CreatedDate, @Latitude, @Longitude, @FormattedAddress)";

                command.Parameters.AddWithValue("@RequestId", request.RequestId);
                command.Parameters.AddWithValue("@UserId", request.UserId);
                command.Parameters.AddWithValue("@Location", request.Location);
                command.Parameters.AddWithValue("@Category", request.Category);
                command.Parameters.AddWithValue("@Description", request.Description);
                command.Parameters.AddWithValue("@Priority", request.Priority);
                command.Parameters.AddWithValue("@Status", request.Status);
                command.Parameters.AddWithValue("@CreatedDate", request.SubmissionDate.ToString("s"));
                command.Parameters.AddWithValue("@Latitude", request.Latitude);
                command.Parameters.AddWithValue("@Longitude", request.Longitude);
                command.Parameters.AddWithValue("@FormattedAddress", request.FormattedAddress ?? string.Empty);

                command.ExecuteNonQuery();

                // Save attachments within the same transaction
                if (request.Attachments != null)
                {
                    foreach (var attachment in request.Attachments)
                    {
                        SaveAttachmentWithTransaction(connection, attachment);
                    }
                }
            }
        }

        public void SaveRelatedIssuesWithTransaction(SQLiteConnection connection, string requestId, IEnumerable<string> relatedRequestIds)
        {
            // Clear existing relations
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM RelatedIssues WHERE RequestId = @RequestId";
                command.Parameters.AddWithValue("@RequestId", requestId);
                command.ExecuteNonQuery();
            }

            // Add new relations
            if (relatedRequestIds?.Any() == true)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO RelatedIssues (RequestId, RelatedRequestId) VALUES (@RequestId, @RelatedId)";
                    
                    var requestIdParam = command.Parameters.Add("@RequestId", DbType.String);
                    var relatedIdParam = command.Parameters.Add("@RelatedId", DbType.String);
                    
                    foreach (var relatedId in relatedRequestIds)
                    {
                        requestIdParam.Value = requestId;
                        relatedIdParam.Value = relatedId;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void SaveAttachmentWithTransaction(SQLiteConnection connection, Attachment attachment)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Attachments (RequestId, Name, ContentBase64)
                    VALUES (@RequestId, @Name, @ContentBase64)";

                command.Parameters.AddWithValue("@RequestId", attachment.RequestId);
                command.Parameters.AddWithValue("@Name", attachment.Name);
                command.Parameters.AddWithValue("@ContentBase64", attachment.ContentBase64);

                command.ExecuteNonQuery();
            }
        }
    }
}
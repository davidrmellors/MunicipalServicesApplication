using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Dapper;
using MunicipalServices.Models;
using Newtonsoft.Json;

namespace MunicipalServices.Core.Services
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Service for managing user profiles, including search history, category interactions, and viewed events
    /// </summary>
    public class UserProfileService
    {
        private const string DATABASE_NAME = "user_profiles.db";
        private string _connectionString;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the UserProfileService class and sets up the database connection
        /// </summary>
        public UserProfileService()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATABASE_NAME);
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates the user profiles database and required tables if they don't exist
        /// </summary>
        private void InitializeDatabase()
        {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATABASE_NAME)))
            {
                SQLiteConnection.CreateFile(DATABASE_NAME);
            }

            using (var connection = new System.Data.SQLite.SQLiteConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id TEXT PRIMARY KEY,
                        SearchHistory TEXT,
                        CategoryInteractions TEXT,
                        ViewedEventIds TEXT
                    )");
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets an existing user or creates a new one if not found
        /// </summary>
        /// <param name="userId">The ID of the user to get or create</param>
        /// <returns>A CurrentUser object containing the user's profile data</returns>
        public CurrentUser GetOrCreateUser(string userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var userDto = connection.QueryFirstOrDefault<UserDto>("SELECT * FROM Users WHERE Id = @Id", new { Id = userId });
                if (userDto == null)
                {
                    var newUser = new CurrentUser { Id = userId };
                    SaveUser(newUser);
                    return newUser;
                }
                else
                {
                    return new CurrentUser
                    {
                        Id = userDto.Id,
                        SearchHistory = string.IsNullOrEmpty(userDto.SearchHistory)
                            ? new List<string>()
                            : JsonConvert.DeserializeObject<List<string>>(userDto.SearchHistory),
                        CategoryInteractions = string.IsNullOrEmpty(userDto.CategoryInteractions)
                            ? new Dictionary<string, int>()
                            : JsonConvert.DeserializeObject<Dictionary<string, int>>(userDto.CategoryInteractions),
                        ViewedEventIds = string.IsNullOrEmpty(userDto.ViewedEventIds)
                            ? new List<string>()
                            : JsonConvert.DeserializeObject<List<string>>(userDto.ViewedEventIds)
                    };
                }
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Saves or updates a user's profile data in the database
        /// </summary>
        /// <param name="user">The user profile to save</param>
        public void SaveUser(CurrentUser user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(@"
                    INSERT OR REPLACE INTO Users (Id, SearchHistory, CategoryInteractions, ViewedEventIds)
                    VALUES (@Id, @SearchHistory, @CategoryInteractions, @ViewedEventIds)",
                    new
                    {
                        user.Id,
                        SearchHistory = JsonConvert.SerializeObject(user.SearchHistory),
                        CategoryInteractions = JsonConvert.SerializeObject(user.CategoryInteractions),
                        ViewedEventIds = JsonConvert.SerializeObject(user.ViewedEventIds)
                    });
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a search query to a user's search history
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="query">The search query to add</param>
        public void AddSearchQuery(string userId, string query)
        {
            var user = GetOrCreateUser(userId);
            user.SearchHistory.Add(query);
            SaveUser(user);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Increments the interaction count for a specific category for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="category">The category to increment interactions for</param>
        public void AddCategoryInteraction(string userId, string category)
        {
            var user = GetOrCreateUser(userId);
            if (!user.CategoryInteractions.ContainsKey(category))
            {
                user.CategoryInteractions[category] = 0;
            }
            user.CategoryInteractions[category]++;
            SaveUser(user);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds an event ID to a user's list of viewed events
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="eventId">The ID of the viewed event</param>
        public void AddViewedEvent(string userId, string eventId)
        {
            var user = GetOrCreateUser(userId);
            if (!user.ViewedEventIds.Contains(eventId))
            {
                user.ViewedEventIds.Add(eventId);
                SaveUser(user);
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Data transfer object for serializing user profile data to/from the database
        /// </summary>
        private class UserDto
        {
            /// <summary>
            /// The unique identifier for the user
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// JSON serialized list of search queries
            /// </summary>
            public string SearchHistory { get; set; }

            /// <summary>
            /// JSON serialized dictionary of category interaction counts
            /// </summary>
            public string CategoryInteractions { get; set; }

            /// <summary>
            /// JSON serialized list of viewed event IDs
            /// </summary>
            public string ViewedEventIds { get; set; }
        }

//-------------------------------------------------------------------------------------------------------------
    }

}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//
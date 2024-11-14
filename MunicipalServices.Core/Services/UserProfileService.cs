using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Dapper;
using MunicipalServices.Models;
using Newtonsoft.Json;

namespace MunicipalServices.Core.Services
{
    public class UserProfileService
    {
        private const string DATABASE_NAME = "user_profiles.db";
        private string _connectionString;

        public UserProfileService()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATABASE_NAME);
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

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

        public void AddSearchQuery(string userId, string query)
        {
            var user = GetOrCreateUser(userId);
            user.SearchHistory.Add(query);
            SaveUser(user);
        }

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

        public void AddViewedEvent(string userId, string eventId)
        {
            var user = GetOrCreateUser(userId);
            if (!user.ViewedEventIds.Contains(eventId))
            {
                user.ViewedEventIds.Add(eventId);
                SaveUser(user);
            }
        }

        private class UserDto
        {
            public string Id { get; set; }
            public string SearchHistory { get; set; }
            public string CategoryInteractions { get; set; }
            public string ViewedEventIds { get; set; }
        }

    }

}
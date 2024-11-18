using System;
using System.Diagnostics;

namespace MunicipalServices.Core.Utilities
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Static utility class for logging messages at different severity levels
    /// </summary>
    public static class Logger
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Logs an informational message with timestamp
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void LogInfo(string message)
        {
            Debug.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Logs an error message with timestamp
        /// </summary>
        /// <param name="message">The error message to log</param>
        public static void LogError(string message)
        {
            Debug.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Logs a warning message with timestamp
        /// </summary>
        /// <param name="message">The warning message to log</param>
        public static void LogWarning(string message)
        {
            Debug.WriteLine($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//
using System;
using System.Diagnostics;

namespace MunicipalServices.Core.Utilities
{
    public static class Logger
    {
        public static void LogInfo(string message)
        {
            Debug.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void LogError(string message)
        {
            Debug.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void LogWarning(string message)
        {
            Debug.WriteLine($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }
    }
}
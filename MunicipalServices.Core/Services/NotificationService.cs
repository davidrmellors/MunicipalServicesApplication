using MunicipalServices.Models;
using System;

namespace MunicipalServices.Core.Services
{
    public static class NotificationService
    {
        public static void Send(string userId, EmergencyNotice notice)
        {
            // Implementation for sending notifications
            Console.WriteLine($"Notification sent to {userId}: {notice.Title}");

            // Log the notification
            DatabaseService.Instance.LogNotification(userId, notice);
        }
    }
}
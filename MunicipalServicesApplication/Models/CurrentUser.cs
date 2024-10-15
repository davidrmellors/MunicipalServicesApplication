using System;

namespace MunicipalServicesApplication.Models
{
    public static class CurrentUser
    {
        public static string UserId { get; private set; }

        public static void SetCurrentUser(string userId)
        {
            UserId = userId;
        }

        public static void ClearCurrentUser()
        {
            UserId = null;
        }
    }
}
using Notifications.Wpf;
using System;

namespace DMTools.Helpers
{
    public static class NotificationHelper
    {
        private static NotificationManager _NotificationManager = new NotificationManager();

        public static void NewNotification(String title, String message, NotificationType type)
        {
            _NotificationManager.Show(new NotificationContent { Title = title, Message = message, Type = type });
        }
    }
}

namespace Bro.Toolbox.Client
{
    public class EmptyNotificationProvider: INotificationProvider
    {
        public string NotificationChannelId { get; }
        public string NotificationChannelName { get; }
        
        public EmptyNotificationProvider()
        {
            Log.Info("notification provider :: create empty provider");
        }
        
        public void SendNotification(LocalNotificationInfo localInfo)
        {
            Log.Info("notification provider :: send notification");
        }

        public void RemoveNotifications()
        {
            Log.Info("notification provider :: remove notification");
        }
    }
}
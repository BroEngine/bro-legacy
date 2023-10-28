namespace Bro.Toolbox.Client
{
    public interface INotificationProvider
    {
        string NotificationChannelId { get; }
        string NotificationChannelName { get; }

        void SendNotification(LocalNotificationInfo localInfo);
        void RemoveNotifications();
    }
}
#if UNITY_ANDROID
using Unity.Notifications.Android;



namespace Bro.Toolbox.Client
{
    public class AndroidNotificationProvider: INotificationProvider
    {
        public string NotificationChannelId { get; }
        public string NotificationChannelName { get; }
        
        public AndroidNotificationProvider(string channelId, string channelName)
        {
            NotificationChannelId = channelId;
            NotificationChannelName = channelName;

            AndroidNotificationCenter.Initialize();

            var channels = AndroidNotificationCenter.GetNotificationChannels();

            if (channels == null || channels.Length == 0)
            {
                var c = new AndroidNotificationChannel()
                {
                    Id = channelId,
                    Name = channelName,
                    Importance = Importance.Default,
                    Description = channelName,
                };

                AndroidNotificationCenter.RegisterNotificationChannel(c);
            }
        }

        public void SendNotification(LocalNotificationInfo localInfo)
        {
            var notification = new AndroidNotification
            {
                Title = localInfo.Title,
                Text = localInfo.Message,
                FireTime = System.DateTime.Now.AddSeconds(localInfo.Seconds),
                LargeIcon = "app_ico_large",
                SmallIcon = "app_ico_small"
            };
            
            AndroidNotificationCenter.SendNotification(notification, NotificationChannelId);
            //Log.Info($"Notification Key: {localInfo.Message}, Seconds: {localInfo.Seconds}, Fire Time: {notification.FireTime}, Title:{notification.Title}, Message:{notification.Text}");
        }


        public void RemoveNotifications()
        {
            AndroidNotificationCenter.CancelAllNotifications();
        }
    }
}
#endif
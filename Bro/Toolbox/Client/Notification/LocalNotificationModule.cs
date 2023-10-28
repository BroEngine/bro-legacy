using System;
using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class LocalNotificationModule : IClientContextModule
    {
        private INotificationProvider _notificationProvider;
        
        private const string NotificationPostfix = "notifications";
        private readonly string _notificationChannelId;
        private readonly string _notificationChannelName;

        private const int HourInDay = 24;

        public LocalNotificationModule()
        {
            #if ! CONSOLE_CLIENT
            var packageName = Application.identifier;
            
            _notificationChannelId = packageName + "_" + NotificationPostfix;
            _notificationChannelName = packageName + " " + NotificationPostfix;
            #endif
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IClientContextModule.Handlers => null;

        void IClientContextModule.Initialize(IClientContext context)
        {
#if UNITY_ANDROID
            _notificationProvider = new AndroidNotificationProvider(_notificationChannelId, _notificationChannelName);
#else
            _notificationProvider = new EmptyNotificationProvider();
#endif
        }

        IEnumerator IClientContextModule.Load()
        {
            RemoveNotifications();
            return null;
        }

        IEnumerator IClientContextModule.Unload()
        {
            return null;
        }

        public void RemoveNotifications()
        {
            _notificationProvider.RemoveNotifications();
        }
        
        public void AddDirectNotification(string title, string message, int seconds)
        {
            
            FireNotification(new LocalNotificationInfo()
            {
                Title = title,
                Message = message,
                Seconds = seconds
            });
        }

        public void AddTimeBindingNotification(string title, string message, int morningTime, int eveningTime)
        {
            var curHour = DateTime.Now.Hour;
            int nextTime;

            if (curHour < morningTime)
            {
                nextTime = morningTime;
            }
            else if (curHour < eveningTime)
            {
                nextTime = eveningTime;
            }
            else
            {
                nextTime = morningTime + HourInDay;
            }

            FireNotification(new LocalNotificationInfo()
            {
                Title = title,
                Message = message,
                Seconds = TimeDiff(DateTime.Today.AddHours(nextTime))
            });
        }

        private void FireNotification(LocalNotificationInfo notification)
        {
            _notificationProvider.SendNotification(notification);
        }

        private static int TimeDiff(DateTime targetDateTime)
        {
            return (int) (ToMilliseconds(targetDateTime) - TimeInfo.LocalTimestamp) / 1000;
        }

        private static long ToMilliseconds(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }
    }
}
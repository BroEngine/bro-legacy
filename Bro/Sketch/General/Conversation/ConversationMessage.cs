using System;

namespace Bro.Sketch
{
    public class ConversationMessage
    {
        public long Timestamp;
        public int UserId;
        public string UserName;
        public byte Type;
        public string Text;
        public string Meta;

        public override string ToString()
        {
            var data = "{" + Timestamp + " " + UserId + ":" + UserName + " " + Text + "}";
            return data;
        }

        public string GetDataTimeString()
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds( Timestamp  ).ToLocalTime();

            // if today
            
            if (dateTime.Date == DateTime.Today)
            {
                return dateTime.ToString("HH:mm");
            }
            
            return dateTime.ToString("MMMM dd, HH:mm");
        }
    }
}
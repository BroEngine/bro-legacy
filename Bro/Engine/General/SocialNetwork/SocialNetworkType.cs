using System.ComponentModel;

namespace Bro
{
    public enum SocialNetworkType : byte
    {
        [Description("")] Undefined = 0,
        [Description("fb")] Facebook = 1,
        [Description("vk")] Vkontakte = 2,
        [Description("ok")] Odnoklassniki = 3,
        [Description("gp")] GooglePlay = 4,
        [Description("gc")] GameCenter = 5
    }
}
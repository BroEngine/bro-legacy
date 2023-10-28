using System.ComponentModel;

namespace Bro
{
    public enum LanguageType : byte
    {
        [Description("")] Unknown = 0,
        [Description("ru")] Russian = 1,
        [Description("en")] English = 2,
        [Description("pl")] Polish = 3,
        [Description("de")] German = 4,
        [Description("fr")] French = 5,
        [Description("es")] Spanish = 6,
        [Description("it")] Italian = 7,
        [Description("lt")] Lithuanian = 8,
        [Description("ua")] Ukrainian = 9,
        [Description("pt")] Portuguese = 10,
        [Description("cz")] Czech = 11,
        [Description("gr")] Greek = 12,
        [Description("cn")] Chinese = 13,
        [Description("tr")] Turkish = 14,
        [Description("vi")] Vietnamese = 15
    }
}
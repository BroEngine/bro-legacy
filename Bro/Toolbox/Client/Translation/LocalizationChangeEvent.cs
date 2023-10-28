using Bro.Client;

namespace Bro.Toolbox.Client
{
    public class LocalizationChangeEvent : Event
    {
        public readonly LanguageType Language;
        
        public LocalizationChangeEvent(LanguageType lang)
        {
            Language = lang;
        }
    }
}
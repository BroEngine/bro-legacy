namespace Bro
{
    public static class LanguageCode
    {
        public static string Get( LanguageType type )
        {
            switch (type)
            {
                case LanguageType.Russian:
                    return "ru";
                case LanguageType.English:
                    return "en";
                case LanguageType.Polish:
                    return "pl";
                case LanguageType.German:
                    return "ge";
                case LanguageType.French:
                    return "fr";
                case LanguageType.Spanish:
                    return "es";
                case LanguageType.Italian:
                    return "it";
                case LanguageType.Lithuanian:
                    return "lt";
                case LanguageType.Ukrainian:
                    return "ua";
                case LanguageType.Portuguese:
                    return "pt";
                case LanguageType.Czech:
                    return "cz";
                case LanguageType.Greek:
                    return "gr";
                case LanguageType.Chinese:
                    return "ch";
                case LanguageType.Turkish:
                    return "tr";
                case LanguageType.Vietnamese:
                    return "vi";
                default:
                    return "en";
            }
        }
    }
}
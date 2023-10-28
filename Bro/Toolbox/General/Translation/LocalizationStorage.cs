using System.Collections.Generic;

namespace Bro.Toolbox
{
    public class LocalizationStorage : UnionConfigStorage<LanguageType, Dictionary<string, string>>
    {
        public override void AddConfigSubStorage(ConfigDetails details, IConfigProvider configProvider)
        {
            AddConfigSubStorage(GetLanguageType(details), (LocalizationSubStorage) configProvider);
        }
        
        private LanguageType GetLanguageType(ConfigDetails details)
        {
            switch (details.Name)
            {
                case "language/ru":
                    return LanguageType.Russian;
                case "language/en":
                    return LanguageType.English;
                default:
                    return LanguageType.Unknown;
            }
        }
    }
}
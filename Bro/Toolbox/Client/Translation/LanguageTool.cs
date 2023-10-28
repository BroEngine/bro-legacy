#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )
using System;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class LanguageTool
    {
        public static LanguageType Locale
        {
            get
            {
                var sysLang = SystemLanguage.English;

                try
                {
                    sysLang = Application.systemLanguage;
                }
                catch (Exception e)
                {
                    Bro.Log.Error("language tool :: exception " + e.ToString());
                }

                switch (sysLang)
                {
                    case SystemLanguage.Russian:
                    case SystemLanguage.Belarusian:
                        return LanguageType.Russian;
                    case SystemLanguage.Ukrainian:
                        return LanguageType.Ukrainian;
                    case SystemLanguage.Polish:
                        return LanguageType.Polish;
                    case SystemLanguage.French:
                        return LanguageType.French;
                    case SystemLanguage.German:
                        return LanguageType.German;
                    case SystemLanguage.Czech:
                        return LanguageType.Czech;
                    case SystemLanguage.Greek:
                        return LanguageType.Greek;
                    case SystemLanguage.Italian:
                        return LanguageType.Italian;
                    case SystemLanguage.Lithuanian:
                        return LanguageType.Lithuanian;
                    case SystemLanguage.Portuguese:
                        return LanguageType.Portuguese;
                    case SystemLanguage.Chinese:
                    case SystemLanguage.ChineseSimplified:
                        return LanguageType.Chinese;
                    case SystemLanguage.Spanish:
                        return LanguageType.Spanish;
                    default:
                        return LanguageType.English;
                }
            }
        }
    }
}
#endif
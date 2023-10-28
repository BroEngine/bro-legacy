using System;
using System.ComponentModel;
using System.Reflection;
using Bro.Json;

namespace Bro
{
    public static class StructExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? ((DescriptionAttribute) attributes[0]).Description : string.Empty;
        }
        
        public static string GetJsonProperty(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(JsonPropertyAttribute), false);
            return attributes.Length > 0 ? ((JsonPropertyAttribute) attributes[0]).PropertyName : string.Empty;
        }

        public static string GetJsonProperty<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();

            if (!type.IsEnum)
            {
                Bro.Log.Error("EnumerationValue must be of Enum type");
            }

            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(JsonPropertyAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((JsonPropertyAttribute) attrs[0]).PropertyName;
                }
            }

            return enumerationValue.ToString();
        }
        
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();

            if (!type.IsEnum)
            {
                Bro.Log.Error("EnumerationValue must be of Enum type");
            }

            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute) attrs[0]).Description;
                }
            }

            return enumerationValue.ToString();
        }

        public static string GetJsonPropertyName<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();

            if (!type.IsEnum)
            {
                Bro.Log.Error("EnumerationValue must be of Enum type");
            }

            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(JsonPropertyAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((JsonPropertyAttribute) attrs[0]).PropertyName;
                }
            }

            return enumerationValue.ToString();
        }
    }
}
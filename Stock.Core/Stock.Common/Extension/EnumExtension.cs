using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Stock.Common.Extension
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerationValue"></param>
        /// <param name="enumeration">if false: default</param>
        /// <returns></returns>
        public static bool TryGetDescription<T>(this T enumerationValue, out string enumeration) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }

            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    enumeration = ((DescriptionAttribute)attrs[0]).Description;
                    return true;
                }
            }

            enumeration = default;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerationValue"></param>
        /// <param name="enumeration">if false: default</param>
        /// <returns></returns>
        public static bool TryGetDescription<T>(this T? enumerationValue, out string enumeration) where T : struct
        {
            if (enumerationValue.HasValue)
            {
                var result = enumerationValue.Value.TryGetDescription(out string str);
                enumeration = str;
                return result;
            }

            enumeration = default;
            return false;
        }

        public static string GetDescriptionOrDefault<T>(this T enumerationValue, string defaultValue = null) where T : struct
        {
            if (TryGetDescription<T>(enumerationValue, out string enumeration))
            {
                return enumeration;
            }
            else
            {
                return defaultValue;
            }
        }
        public static string GetDescriptionOrDefault<T>(this T? enumerationValue, string defaultValue = null) where T : struct
        {
            if (TryGetDescription<T>(enumerationValue, out string enumeration))
            {
                return enumeration;
            }
            else
            {
                return defaultValue;
            }
        }

        public static string GetDescriptionOrDefault<T>(int enumerationValue, string defaultValue = null) where T : struct ,Enum
        {
            try
            {
                if (TryGetDescription((T)(object)enumerationValue, out string enumeration))
                {
                    return enumeration;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static List<KeyValuePair<int, string>> GetDescriptionList<T>() where T : struct
        {
            var list = new List<KeyValuePair<int, string>>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                list.Add(new KeyValuePair<int, string>(e.GetHashCode(), GetDescription((T)e)));
            }
            return list;
        }

        public static List<KeyValuePair<int, string>> GetList<T>() where T : struct
        {
            var list = new List<KeyValuePair<int, string>>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                list.Add(new KeyValuePair<int, string>(e.GetHashCode(), e.ToString()));
            }
            return list;
        }

        public static List<T> GetEnumValue<T>(Type attributeType)
        {
            Type tType = typeof(T);
            List<T> tValues = new List<T>();
            foreach (T tValue in Enum.GetValues(tType))
            {
                MemberInfo tMemberInfo = tType.GetMember(tValue.ToString())[0];
                if (Attribute.IsDefined(tMemberInfo, attributeType))
                    tValues.Add(tValue);
            }
            return tValues;
        }

        public static List<KeyValuePair<int, string>> GetEnumValueWithDesc<T>(Type attributeType) where T : struct
        {
            Type tType = typeof(T);
            var list = new List<KeyValuePair<int, string>>();
            foreach (T tValue in Enum.GetValues(tType))
            {
                MemberInfo tMemberInfo = tType.GetMember(tValue.ToString())[0];
                if (Attribute.IsDefined(tMemberInfo, attributeType))
                {
                    list.Add(new KeyValuePair<int, string>(tValue.GetHashCode(), GetDescription<T>(tValue)));
                }
            }
            return list;
        }

        public static T GetEnumValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                .SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Att = a })
                .SingleOrDefault(a => ((DescriptionAttribute)a.Att).Description == description);
            return field == null ? default : (T)field.Field.GetRawConstantValue();
        }

        public static bool TryParse<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                .SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Att = a })
                .SingleOrDefault(a => ((DescriptionAttribute)a.Att).Description == description);
            return field != null;
        }

        public static bool HasAttribute<T>(this Enum value) where T : Attribute
            => value?.GetType().GetField(value.ToString()).IsDefined(typeof(T), false) ?? false;


        public static Attribute GetCustomAttributeValue<T>(this Enum value) where T : Attribute
            => value.GetType().GetField(value.ToString()).GetCustomAttribute(typeof(T), false);
        
    }
}

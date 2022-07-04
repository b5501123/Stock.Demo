using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Stock.Common.Extension
{
    public static class StringExtension
    {
        public static List<T> ToListByCutString<T>(this string data, string cutString = ",")
        {
            List<T> result = null;
            if (!string.IsNullOrEmpty(data))
            {
                if (typeof(T) == typeof(bool))
                {
                    result = data.Split(cutString, StringSplitOptions.RemoveEmptyEntries).Select(i => (T)Convert.ChangeType(int.Parse(i), typeof(T))).ToList();
                }
                else if (typeof(T).IsEnum)
                {
                    result = data.Split(cutString, StringSplitOptions.RemoveEmptyEntries).Select(i => (T)Convert.ChangeType(Enum.Parse(typeof(T), i, true), typeof(T))).ToList();
                }
                else
                    result = data.Split(cutString, StringSplitOptions.RemoveEmptyEntries).Select(i => (T)Convert.ChangeType(i, typeof(T))).ToList();
            }
            return result;
        }

        public static List<T> ToListByCutString<T>(this string data, char[] cutString)
        {
            List<T> result = null;
            if (!string.IsNullOrEmpty(data))
            {
                if (typeof(T) == typeof(bool))
                {
                    result = data.Split(cutString, StringSplitOptions.RemoveEmptyEntries).Select(i => (T)Convert.ChangeType(int.Parse(i), typeof(T))).ToList();
                }
                else if (typeof(T).IsEnum)
                {
                    result = data.Split(cutString, StringSplitOptions.RemoveEmptyEntries).Select(i => (T)Convert.ChangeType(Enum.Parse(typeof(T), i, true), typeof(T))).ToList();
                }
                else
                    result = data.Split(cutString, StringSplitOptions.RemoveEmptyEntries).Select(i => (T)Convert.ChangeType(i, typeof(T))).ToList();
            }
            return result;
        }


        public static string SerializeToXmlString<T>(this T value)
        {
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(value.GetType());
            var settings = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = true
            };

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, value, emptyNamespaces);
                return stream.ToString();
            }
        }

        public static T XmlToClass<T>(this string xml) where T : new()
        {
            try
            {
                T result;
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (StringReader reader = new StringReader(xml))
                {
                    result = (T)(serializer.Deserialize(reader));
                }
                return result;
            }
            catch
            {
                return new T();
            }
        }

        public static string EndSubString(this string str, int charCount)
        {
            return str.Substring(str.Length - charCount, charCount);
        }

        public static string Right(this string s, int length)
        {
            length = Math.Max(length, 0);

            return s.Length > length ? s.Substring(s.Length - length, length) : s;
        }

        public static string Left(this string s, int length)
        {
            length = Math.Max(length, 0);

            return s.Length > length ? s.Substring(0, length) : s;
        }

        public static string OptionalFormat(this string formatString, params string[] args)
        {
            Regex curlyBracketRegex = new Regex("\\{(.+?)\\}");
            var numberOfArguments = curlyBracketRegex.Matches(formatString).Count;

            var missingArgumentCount = numberOfArguments - args.Length;
            if (missingArgumentCount <= 0)
                return string.Format(formatString, args);

            args = args.Concat(Enumerable.Range(0, missingArgumentCount).Select(_ => string.Empty)).ToArray();
            return string.Format(formatString, args);
        }

        /// <summary>
        /// 字串轉為Nullable Datetime時間格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime? TryToDateTime(this string str)
        {
            if (DateTime.TryParse(str, out var date))
                return date;

            return null;
        }

        /// <summary>
        /// 取出前N個字，並在最後加上 "..."
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <param name="truncation"></param>
        /// <returns></returns>
        public static string Truncate(this string s,
            int length = 30,
            string truncation = "...")
        {
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;

            return s.Length > length ?
                s.Substring(0,
                    length - truncation.Length) + truncation : s;
        }

        public static string RemovePrefix(this string fileName, char delimeter = '_')
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;

            var delimiterIndex = fileName.IndexOf(delimeter);
            return fileName.Remove(0, delimiterIndex + 1);
        }

        public static decimal ToRoundDown(this string data)
        {
            return Math.Truncate(decimal.Parse(data) * 100) / 100;
        }

        /// <summary>
        /// 設定資料遮罩
        /// </summary>
        /// <param name="source"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        public static string SetMask(this string source, int minLength = 3)
        {
            if (source.IsEmptyOrNull())
            {
                return string.Empty;
            }

            // 如果來源字串小於最小長度或是小於可處理最低長度，則不處理
            if (source.Length < minLength)
            {
                return source.Substring(0, 1) + "**";
            }

            if (source.Length == minLength)
            {
                return SetMaskForMinLength(source, minLength);
            }

            var mark = string.Empty;
            for (var i = 0; i < minLength; i++)
            {
                mark += "*";
            }

            var result = source.Substring(0, source.Length - minLength) +
                         mark;
            return result;
        }

        /// <summary>
        /// 為符合最小長度的字串設定
        /// 1. 會呼叫到這方式至少一定要是5個字元以上
        /// </summary>
        /// <param name="source"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        private static string SetMaskForMinLength(string source, int minLength)
        {
            var subLength = minLength / 2;
            var maskLength = source.Length - subLength;
            var result = source.Substring(0, 2);
            for (var i = 0; i < maskLength; i++)
            {
                result += "x";
            }

            return result;
        }
    }
}

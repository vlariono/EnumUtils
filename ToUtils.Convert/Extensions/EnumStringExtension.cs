using System;
using System.Linq;
using System.Reflection;
using ToUtils.Convert.Attributes;
using ToUtils.Convert.Enums;
using ToUtils.Convert.Handlers;

namespace ToUtils.Convert.Extensions
{
    /// <summary>
    /// Contains utils to convert between string and enums
    /// </summary>
    public static class EnumStringExtension
    {
        private static readonly MapHandler<(Type Type, ulong Value), (Type Type, string StringValue)> Map;

        static EnumStringExtension()
        {
            Map = new MapHandler<(Type, ulong), (Type, string)>();
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation
        /// </summary>
        /// <param name="enumValue">Enum value to convert to string</param>
        /// <param name="stringSource">Source of the string</param>
        public static string ToString<TEnum>(this TEnum enumValue, Source stringSource) where TEnum : struct, Enum
        {
            string? resultString = null;
            if ((stringSource & Source.Attribute) != 0)
            {
                var type = typeof(TEnum);
                var value = enumValue.ToULong();
                var key = (type, value);
                if (Map.TryGet(key, out var mapResult))
                {
                    return mapResult.StringValue;
                }

                if (Enum.IsDefined(enumValue))
                {
                    var name = enumValue.ToString();
                    var field = type.GetField(name);
                    if (field != null)
                    {
                        resultString = field.GetCustomAttribute<ToString>()?.Value;
                    }

                    if (string.IsNullOrEmpty(resultString))
                    {
                        resultString = name;
                    }

                    Map.TryAdd(key, (type, resultString));
                }
            }

            return resultString ?? enumValue.ToString();
        }

        /// <summary>
        /// Converts the string to an equivalent enumerated object
        /// </summary>
        public static bool TryToEnum<T>(this string stringValue, out T result) where T : struct, Enum
        {
            return stringValue.TryToEnum<T>(Source.Attribute, out result);
        }

        /// <summary>
        /// Converts the string to an equivalent enumerated object
        /// </summary>
        public static bool TryToEnum<T>(this string stringValue, Source stringSource, out T result) where T : struct, Enum
        {
            result = default;
            if (string.IsNullOrEmpty(stringValue))
            {
                return false;
            }

            if ((stringSource & Source.Attribute) != 0)
            {
                var type = typeof(T);
                var key = (type, stringValue);
                if (Map.TryGet(key, out var mapResult))
                {
                    result = mapResult.Value.ToEnum<T>();
                    return true;
                }

                foreach(var value in type.GetEnumValues().Cast<T>().Where(e => !Map.TryGet((typeof(T), e.ToULong()),out var _)))
                {
                    var currentString = value.ToString(Source.Attribute);
                    if(currentString.Equals(stringValue,StringComparison.Ordinal))
                    {
                        result = value;
                        return true;
                    }
                }

                if (Map.TryGet(key, out mapResult))
                {
                    result = mapResult.Value.ToEnum<T>();
                    return true;
                }
            }

            return Enum.TryParse(stringValue, out result) && Enum.IsDefined<T>(result);
        }

        /// <summary>
        /// Assigns custom string to enum value, this method should be called before any other methods
        /// Once string is assigned it can not be changed
        /// </summary>
        public static bool AssignString<T>(this T enumValue, string stringValue) where T : struct, Enum
        {
            var type = typeof(T);
            var source = (type, enumValue.ToULong());
            var result = (type, stringValue);
            return Map.TryAdd(source, result);
        }
    }
}

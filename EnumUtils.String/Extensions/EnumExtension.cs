using System;
using System.Collections.Concurrent;
using System.Reflection;
using EnumUtils.String.Attributes;
using EnumUtils.String.Enums;
using EnumUtils.String.Handlers;
using EnumUtils.UInt.Extensions;

namespace EnumUtils.String.Extensions
{
    /// <summary>
    /// Contains utils to convert between string and enums
    /// </summary>
    public static class EnumExtension
    {
        private static readonly MapHandler<(Type Type, ulong Value), (Type Type, string StringValue)> Map;
        private static readonly ConcurrentDictionary<Type, bool> MapsDone;

        static EnumExtension()
        {
            Map = new MapHandler<(Type, ulong), (Type, string)>();
            MapsDone = new ConcurrentDictionary<Type, bool>();
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation
        /// </summary>
        /// <param name="enumValue">Enum value to convert to string</param>
        /// <param name="stringSource">Source of the string</param>
        public static string ToString<T>(this T enumValue, StringSource stringSource) where T : struct, Enum
        {
            string? resultString = null;
            if ((stringSource & StringSource.CustomString) != 0)
            {
                var type = typeof(T);
                var value = enumValue.ToUInt();
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
                        resultString = field.GetCustomAttribute<CustomString>()?.Value;
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
            return stringValue.TryToEnum<T>(StringSource.CustomString, out result);
        }

        /// <summary>
        /// Converts the string to an equivalent enumerated object
        /// </summary>
        public static bool TryToEnum<T>(this string stringValue, StringSource stringSource, out T result) where T : struct, Enum
        {
            result = default;
            if (string.IsNullOrEmpty(stringValue))
            {
                return false;
            }

            if ((stringSource & StringSource.CustomString) != 0)
            {
                BuildMap<T>();
                var type = typeof(T);
                var key = (type, stringValue);
                if (Map.TryGet(key, out var mapResult))
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
            var source = (type, enumValue.ToUInt());
            var result = (type, stringValue);
            return Map.TryAdd(source, result);
        }

        internal static void BuildMap<T>() where T : struct, Enum
        {
            var type = typeof(T);
            if (MapsDone.TryGetValue(type, out var done) && done)
            {
                return;
            }

            foreach (T value in type.GetEnumValues())
            {
                value.ToString(StringSource.CustomString);
            }
            MapsDone.TryAdd(type, true);
        }
    }
}

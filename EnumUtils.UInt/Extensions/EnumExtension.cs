using System;
using System.Runtime.CompilerServices;

namespace EnumUtils.UInt.Extensions
{
    public static class EnumExtension
    {
        public static ulong ToUInt<T>(this T enumValue) where T : Enum
        {
            return Unsafe.SizeOf<T>() switch
            {
                1 => Unsafe.As<T, byte>(ref enumValue),
                2 => Unsafe.As<T, ushort>(ref enumValue),
                4 => Unsafe.As<T, uint>(ref enumValue),
                8 => Unsafe.As<T, ulong>(ref enumValue),
                _ => throw new InvalidCastException()
            };
        }

        public static T ToEnum<T>(this ulong value) where T : Enum
        {
            switch (Unsafe.SizeOf<T>())
            {
                case 1:
                    var byteValue = (byte)value;
                    return Unsafe.As<byte, T>(ref byteValue);
                case 2:
                    var ushortValue = (ushort)value;
                    return Unsafe.As<ushort, T>(ref ushortValue);
                case 4:
                    var uintValue = (uint)value;
                    return Unsafe.As<uint, T>(ref uintValue);
                case 8:
                    return Unsafe.As<ulong, T>(ref value);
            }

            throw new InvalidCastException();
        }
    }
}

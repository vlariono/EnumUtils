using System;
using System.Runtime.CompilerServices;

namespace ToUtils.Convert.Extensions
{
    public static class EnumULongExtension
    {
        public static ulong ToULong<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
        {
            return Unsafe.SizeOf<TEnum>() switch
            {
                1 => Unsafe.As<TEnum, byte>(ref enumValue),
                2 => Unsafe.As<TEnum, ushort>(ref enumValue),
                4 => Unsafe.As<TEnum, uint>(ref enumValue),
                8 => Unsafe.As<TEnum, ulong>(ref enumValue),
                _ => throw new InvalidCastException()
            };
        }

        public static TEnum ToEnum<TEnum>(this ulong value) where TEnum : struct, Enum
        {
            switch (Unsafe.SizeOf<TEnum>())
            {
                case 1:
                    var byteValue = (byte)value;
                    return Unsafe.As<byte, TEnum>(ref byteValue);
                case 2:
                    var ushortValue = (ushort)value;
                    return Unsafe.As<ushort, TEnum>(ref ushortValue);
                case 4:
                    var uintValue = (uint)value;
                    return Unsafe.As<uint, TEnum>(ref uintValue);
                case 8:
                    return Unsafe.As<ulong, TEnum>(ref value);
            }

            throw new InvalidCastException();
        }
    }
}
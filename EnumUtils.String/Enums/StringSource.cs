using System;

namespace EnumUtils.String.Enums
{
    [Flags]
    public enum StringSource : short
    {
        /// <summary>
        /// Default ToString behavior
        /// </summary>
        Default = 0,

        /// <summary>
        /// EnumString value or Default
        /// </summary>
        CustomString = 1
    }
}

using System;

namespace ToUtils.Convert.Enums
{
    [Flags]
    public enum Source : short
    {
        /// <summary>
        /// Default behavior
        /// </summary>
        Default = 0,

        /// <summary>
        /// Value from attribute or Default
        /// </summary>
        Attribute = 1
    }
}

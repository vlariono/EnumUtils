using System;

namespace EnumUtils.String.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CustomString : Attribute
    {
        public CustomString(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}

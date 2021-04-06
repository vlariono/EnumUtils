using System;

namespace ToUtils.Convert.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ToString : Attribute
    {
        public ToString(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}

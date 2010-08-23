using System;

namespace GoCommando.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PositionalArgumentAttribute : ArgumentAttribute
    {
        public int Index { get; set; }

        public PositionalArgumentAttribute(int index)
        {
            Index = index;
        }
    }
}
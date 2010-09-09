using System;

namespace GoCommando.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NamedArgumentAttribute : ArgumentAttribute
    {
        public string Name { get; private set; }
        public string ShortHand { get; private set; }
        public string Default { get; set; }

        public NamedArgumentAttribute(string name, string shortHand)
        {
            Name = name;
            ShortHand = shortHand;
        }
    }
}
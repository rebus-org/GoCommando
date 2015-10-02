using System;

namespace GoCommando
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NamedArgumentAttribute : ArgumentAttribute
    {
        public string Name { get; set; }
        public string ShortHand { get; set; }

        public NamedArgumentAttribute(string name, string shortHand)
        {
            Name = name;
            ShortHand = shortHand;
        }
    }
}
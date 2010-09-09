using System;

namespace GoCommando.Attributes
{
    public abstract class ArgumentAttribute : Attribute
    {
        public bool Required { get; protected set; }
    }
}
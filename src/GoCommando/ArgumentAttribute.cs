using System;

namespace GoCommando
{
    public abstract class ArgumentAttribute : Attribute
    {
        public bool Required { get; set; }
    }
}
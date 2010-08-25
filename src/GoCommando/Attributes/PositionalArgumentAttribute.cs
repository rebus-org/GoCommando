using System;

namespace GoCommando.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PositionalArgumentAttribute : ArgumentAttribute
    {
    }
}
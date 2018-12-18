using System;

namespace GoCommando
{
    /// <summary>
    /// Apply this attribute to the class that has your <code>Main</code> method in order to support impersonation
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SupportImpersonationAttribute : Attribute
    {
    }
}
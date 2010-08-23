using System;
using GoCommando.Api;

namespace GoCommando.Helpers
{
    public class DefaultCommandoFactory
    {
        public ICommando Create(Type type)
        {
            return (ICommando)Activator.CreateInstance(type);
        }
    }
}
using System;

namespace GoCommando.Exceptions
{
    public class CommandoException : ApplicationException
    {
        public CommandoException(string message, params object[] objs)
            : base(String.Format(message, objs))
        {
        }
    }
}
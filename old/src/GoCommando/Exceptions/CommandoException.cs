using System;

namespace GoCommando.Exceptions
{
    public class CommandoException : ApplicationException
    {
        public CommandoException(string message, params object[] objs)
            : base(string.Format(message, objs))
        {
        }

        public CommandoException(Exception innerException, string message, params object[] objs)
            : base(string.Format(message, objs), innerException)
        {
        }
    }
}
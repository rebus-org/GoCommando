using System;

namespace GoCommando
{
    public class CommandoException : ApplicationException
    {
        public CommandoException(string message, params object[] objs)
            : base(String.Format(message, objs))
        {
        }
    }
}
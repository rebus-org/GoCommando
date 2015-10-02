using System;
using System.Runtime.Serialization;

namespace GoCommando
{
    /// <summary>
    /// Friendly exception that can be thrown in cases where you want the program to exit with
    /// a nice, human-readable message. Only the message will be shown.
    /// </summary>
    [Serializable]
    public class GoCommandoException : Exception
    {
        protected GoCommandoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GoCommandoException(string message) : base(message)
        {
        }
    }
}
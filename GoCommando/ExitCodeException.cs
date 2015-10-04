using System;
using System.Runtime.Serialization;

namespace GoCommando
{
    /// <summary>
    /// Exception that can be used to exit the program with a custom exit code
    /// </summary>
    [Serializable]
    public class CustomExitCodeException : Exception
    {
        protected CustomExitCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CustomExitCodeException(int exitCode, string message) : base(message)
        {
            ExitCode = exitCode;
        }

        public int ExitCode { get; }
    }
}
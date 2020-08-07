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
        /// <summary>
        /// Constructs the exception
        /// </summary>
        protected CustomExitCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Constructs the exception
        /// </summary>
        public CustomExitCodeException(int exitCode, string message) : base(message)
        {
            ExitCode = exitCode;
        }

        /// <summary>
        /// Gets the exit code that the program must exit with
        /// </summary>
        public int ExitCode { get; }
    }
}
using System;

namespace GoCommando
{
    /// <summary>
    /// Attribute that can be applied to a class that represents a command. The class must implement <see cref="ICommand"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the command
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the name of the command's group (if any). Grouping commands affects how they are presented when printing
        /// help texts
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// Constructs the attribute
        /// </summary>
        public CommandAttribute(string command, string group = null)
        {
            Command = command;
            Group = group ?? "";
        }
    }
}
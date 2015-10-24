using System;

namespace GoCommando
{
    /// <summary>
    /// Apply this attribute to a property of a command class (i.e. one that implements <see cref="ICommand"/>)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets the primary parameter name
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Gets a shorthand for the parameter (or null if none has been specified)
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// Gets whether this parameter is optional
        /// </summary>
        public bool Optional { get; }
        
        /// <summary>
        /// Gets a default value for the parameter (or null if none has been specified)
        /// </summary>
        public string DefaultValue { get; }
        
        /// <summary>
        /// Gets whether this parameter can have its value resolved from the <code>&lt;appSettings&gt;</code> section of the application configuration file.
        /// If the value is provided with a command-line switch, the provided value takes precedence.
        /// </summary>
        public bool AllowAppSetting { get; }

        /// <summary>
        /// Gets whether this parameter can have its value resolved from the <code>&lt;connectionStrings&gt;</code> section of the application configuration file.
        /// If the value is provided with a command-line switch, the provided value takes precedence.
        /// </summary>
        public bool AllowConnectionString { get; }

        /// <summary>
        /// Gets whether this parameter can have its value resolved from an environment variable with the same name as specified by <see cref="Name"/>
        /// If the value is provided with a command-line switch, the provided value takes precedence.
        /// </summary>
        public bool AllowEnvironmentVariable { get; }

        /// <summary>
        /// Constructs the parameter attribute
        /// </summary>
        /// <param name="name">Primary name of the parameter</param>
        /// <param name="shortName">Optional shorthand of the parameter</param>
        /// <param name="optional">Indicates whether the parameter MUST be specified or can be omitted</param>
        /// <param name="defaultValue">Provides a default value to use when other values could not be found</param>
        /// <param name="allowAppSetting">
        /// Indicates whether parameter value resolution can go and look in the <code>&lt;appSettings&gt;</code> section of
        /// the current application configuration file for a value. Will look for the key specified by <paramref name="name"/>
        /// </param>
        /// <param name="allowConnectionString">
        /// Indicates whether parameter value resolution can go and look in the <code>&lt;connectionStrings&gt;</code> section of
        /// the current application configuration file for a value. Will look for the name specified by <paramref name="name"/>
        /// </param>
        /// <param name="allowEnvironmentVariable">
        /// Indicates whether parameter value resolution can go and look for an environment variable for a value.
        /// Will look for the name specified by <paramref name="name"/>
        /// </param>
        public ParameterAttribute(string name, string shortName = null, bool optional = false, string defaultValue = null, bool allowAppSetting = false, bool allowConnectionString = false, bool allowEnvironmentVariable = false)
        {
            Name = name;
            ShortName = shortName;
            Optional = optional;
            DefaultValue = defaultValue;
            AllowAppSetting = allowAppSetting;
            AllowConnectionString = allowConnectionString;
            AllowEnvironmentVariable = allowEnvironmentVariable;
        }
    }
}
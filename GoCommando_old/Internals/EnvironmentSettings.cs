using System;
using System.Collections.Generic;

namespace GoCommando.Internals
{
    class EnvironmentSettings
    {
        static readonly IDictionary<string, string> None = new Dictionary<string, string>();
        public static readonly EnvironmentSettings Empty = new EnvironmentSettings(None, None);

        readonly IDictionary<string, string> _appSettings;
        readonly IDictionary<string, string> _connectionStrings;
        readonly IDictionary<string, string> _environmentVariables;

        public EnvironmentSettings(IDictionary<string, string> appSettings = null, IDictionary<string, string> connectionStrings = null, IDictionary<string, string> environmentVariables = null)
        {
            _environmentVariables = environmentVariables ?? None;
            _appSettings = appSettings ?? None;
            _connectionStrings = connectionStrings ?? None;
        }

        public bool HasAppSetting(string name)
        {
            return _appSettings.ContainsKey(name);
        }

        public bool HasConnectionString(string name)
        {
            return _connectionStrings.ContainsKey(name);
        }

        public bool HasEnvironmentVariable(string name)
        {
            return _environmentVariables.ContainsKey(name);
        }

        public string GetAppSetting(string key)
        {
            try
            {
                return _appSettings[key];
            }
            catch (Exception exception)
            {
                throw new KeyNotFoundException($"Could not find appSetting with key '{key}'", exception);
            }
        }

        public string GetEnvironmentVariable(string name)
        {
            try
            {
                return _environmentVariables[name];
            }
            catch (Exception exception)
            {
                throw new KeyNotFoundException($"Could not find environment variable with the name '{name}'", exception);
            }
        }

        public string GetConnectionString(string name)
        {
            try
            {
                return _connectionStrings[name];
            }
            catch (Exception exception)
            {
                throw new KeyNotFoundException($"Could not find connectionString with key '{name}'", exception);
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace GoCommando.Internals
{
    class EnvironmentSettings
    {
        static readonly Dictionary<string, string> None = new Dictionary<string, string>();
        public static readonly EnvironmentSettings Empty = new EnvironmentSettings(None, None);

        readonly IDictionary<string, string> _appSettings;
        readonly Dictionary<string, string> _connectionStrings;

        public EnvironmentSettings(IDictionary<string, string> appSettings = null, Dictionary<string, string> connectionStrings = null)
        {
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

        public string GetAppSetting(string name)
        {
            try
            {
                return _appSettings[name];
            }
            catch (Exception exception)
            {
                throw new KeyNotFoundException($"Could not find appSetting with key '{name}'", exception);
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
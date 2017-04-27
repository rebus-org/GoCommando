using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GoCommando.Internals
{
    class CommandInvoker
    {
        readonly Settings _settings;
        readonly ICommand _commandInstance;
        readonly Action<ICommand> _releaser;

        public CommandInvoker(string command, Type type, Settings settings, string group = null, ICommandFactory commandFactory = null)
            : this(command, settings, CreateInstance(type, GetFactoryMethod(commandFactory)), group, GetReleaseMethod(commandFactory))
        {
        }

        public CommandInvoker(string command, Settings settings, ICommand commandInstance, string group = null, Action<ICommand> releaseMethod = null)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (commandInstance == null) throw new ArgumentNullException(nameof(commandInstance));

            _settings = settings;
            _commandInstance = commandInstance;
            _releaser = releaseMethod ?? DefaultReleaseMethod;

            Command = command;
            Group = group;
            Parameters = GetParameters(Type);
        }

        static void DefaultReleaseMethod(ICommand command)
        {
            var disposable = command as IDisposable;

            disposable?.Dispose();
        }

        static Func<Type, ICommand> GetFactoryMethod(ICommandFactory commandFactory)
        {
            if (commandFactory == null) return null;

            return commandFactory.Create;
        }

        static Action<ICommand> GetReleaseMethod(ICommandFactory commandFactory)
        {
            if (commandFactory == null) return null;

            return commandFactory.Release;
        }

        static ICommand CreateInstance(Type type, Func<Type, ICommand> commandFactory = null)
        {
            try
            {
                var instance = commandFactory?.Invoke(type)
                               ?? Activator.CreateInstance(type);

                if (!(instance is ICommand))
                {
                    throw new ApplicationException($"{instance} does not implement ICommand!");
                }

                return (ICommand)instance;
            }
            catch (Exception exception)
            {
                throw new ApplicationException($"Could not use type {type} as a GoCommando command", exception);
            }
        }

        static IEnumerable<Parameter> GetParameters(Type type)
        {
            return type
                .GetProperties()
                .Select(p => new
                {
                    Property = p,
                    ParameterAttribute = GetSingleAttributeOrNull<ParameterAttribute>(p),
                    DescriptionAttribute = GetSingleAttributeOrNull<DescriptionAttribute>(p),
                    ExampleAttributes = p.GetCustomAttributes<ExampleAttribute>()
                })
                .Where(a => a.ParameterAttribute != null)
                .Select(a => new Parameter(a.Property,
                    a.ParameterAttribute.Name,
                    a.ParameterAttribute.ShortName,
                    a.ParameterAttribute.Optional,
                    a.DescriptionAttribute?.DescriptionText,
                    a.ExampleAttributes.Select(e => e.ExampleValue),
                    a.ParameterAttribute.DefaultValue,
                    a.ParameterAttribute.AllowAppSetting,
                    a.ParameterAttribute.AllowConnectionString,
                    a.ParameterAttribute.AllowEnvironmentVariable))
                .ToList();
        }

        static TAttribute GetSingleAttributeOrNull<TAttribute>(PropertyInfo p) where TAttribute : Attribute
        {
            return p.GetCustomAttributes(typeof(TAttribute), false)
                .Cast<TAttribute>()
                .FirstOrDefault();
        }

        public string Group { get; }

        public string Command { get; }

        public Type Type => _commandInstance.GetType();

        public IEnumerable<Parameter> Parameters { get; }

        public string Description => Type.GetCustomAttribute<DescriptionAttribute>()?.DescriptionText ??
                                     "(no help text for this command)";

        public ICommand CommandInstance => _commandInstance;

        public void Invoke(IEnumerable<Switch> switches, EnvironmentSettings environmentSettings)
        {
            try
            {
                InnerInvoke(switches, environmentSettings);
            }
            finally
            {
                _releaser(CommandInstance);
            }
        }

        void InnerInvoke(IEnumerable<Switch> switches, EnvironmentSettings environmentSettings)
        {
            var commandInstance = _commandInstance;

            var requiredParametersMissing = Parameters
                .Where(p => !p.Optional
                            && !p.HasDefaultValue
                            && !CanBeResolvedFromSwitches(switches, p)
                            && !CanBeResolvedFromEnvironmentSettings(environmentSettings, p))
                .ToList();

            var optionalParamtersNotSpecified = Parameters
                .Where(p => p.Optional
                            && !CanBeResolvedFromSwitches(switches, p)
                            && !CanBeResolvedFromEnvironmentSettings(environmentSettings, p))
                .ToList();

            if (requiredParametersMissing.Any())
            {
                var requiredParametersMissingString = string.Join(Environment.NewLine,
                    requiredParametersMissing.Select(p => $"    {_settings.SwitchPrefix}{p.Name} - {p.DescriptionText}"));

                var text = $@"The following required parameters are missing:

{requiredParametersMissingString}";

                if (optionalParamtersNotSpecified.Any())
                {
                    var optionalParamtersNotSpecifiedString = string.Join(Environment.NewLine,
                        optionalParamtersNotSpecified.Select(p => $"    {_settings.SwitchPrefix}{p.Name} - {p.DescriptionText}"));

                    var moreText = $@"The following optional parameters are also available:

{optionalParamtersNotSpecifiedString}";

                    throw new GoCommandoException(string.Concat(
                        text,
                        Environment.NewLine,
                        Environment.NewLine,
                        moreText
                    ));
                }

                throw new GoCommandoException(text);
            }

            var switchesWithoutMathingParameter = switches
                .Where(s => !Parameters.Any(p => p.MatchesKey(s.Key)))
                .ToList();

            if (switchesWithoutMathingParameter.Any())
            {
                var switchesWithoutMathingParameterString = string.Join(Environment.NewLine,
                    switchesWithoutMathingParameter.Select(p => p.Value != null
                        ? $"    {_settings.SwitchPrefix}{p.Key} = {p.Value}"
                        : $"    {_settings.SwitchPrefix}{p.Key}"));

                throw new GoCommandoException(
                    $@"The following switches do not have a corresponding parameter:

{switchesWithoutMathingParameterString}");
            }

            var setParameters = new HashSet<Parameter>();

            ResolveParametersFromSwitches(switches, commandInstance, setParameters);

            ResolveParametersFromEnvironmentSettings(environmentSettings, commandInstance, setParameters, Parameters);

            ResolveParametersWithDefaultValues(setParameters, commandInstance);

            commandInstance.Run();
        }

        static void ResolveParametersFromEnvironmentSettings(EnvironmentSettings environmentSettings, ICommand commandInstance, HashSet<Parameter> setParameters, IEnumerable<Parameter> parameters)
        {
            foreach (var parameter in parameters.Where(p => p.AllowAppSetting && !setParameters.Contains(p)))
            {
                if (!environmentSettings.HasAppSetting(parameter.Name)) continue;

                var appSettingValue = environmentSettings.GetAppSetting(parameter.Name);

                SetParameter(commandInstance, setParameters, parameter, appSettingValue);
            }

            foreach (var parameter in parameters.Where(p => p.AllowConnectionString && !setParameters.Contains(p)))
            {
                if (!environmentSettings.HasConnectionString(parameter.Name)) continue;

                var appSettingValue = environmentSettings.GetConnectionString(parameter.Name);

                SetParameter(commandInstance, setParameters, parameter, appSettingValue);
            }

            foreach (var parameter in parameters.Where(p => p.AllowEnvironmentVariable && !setParameters.Contains(p)))
            {
                if (!environmentSettings.HasEnvironmentVariable(parameter.Name)) continue;

                var appSettingValue = environmentSettings.GetEnvironmentVariable(parameter.Name);

                SetParameter(commandInstance, setParameters, parameter, appSettingValue);
            }
        }

        void ResolveParametersWithDefaultValues(IEnumerable<Parameter> setParameters, ICommand commandInstance)
        {
            foreach (var parameterWithDefaultValue in Parameters.Where(p => p.HasDefaultValue).Except(setParameters))
            {
                parameterWithDefaultValue.ApplyDefaultValue(commandInstance);
            }
        }

        void ResolveParametersFromSwitches(IEnumerable<Switch> switches, ICommand commandInstance, ISet<Parameter> setParameters)
        {
            foreach (var switchToSet in switches)
            {
                var correspondingParameter = Parameters.FirstOrDefault(p => p.MatchesKey(switchToSet.Key));

                if (correspondingParameter == null)
                {
                    throw new GoCommandoException(
                        $"The switch {_settings}{switchToSet.Key} does not correspond to a parameter of the '{Command}' command!");
                }

                var value = switchToSet.Value;

                SetParameter(commandInstance, setParameters, correspondingParameter, value);
            }
        }

        static void SetParameter(ICommand commandInstance, ISet<Parameter> setParameters, Parameter parameter, string value)
        {
            parameter.SetValue(commandInstance, value);
            setParameters.Add(parameter);
        }

        static bool CanBeResolvedFromEnvironmentSettings(EnvironmentSettings environmentSettings, Parameter parameter)
        {
            var name = parameter.Name;

            if (parameter.AllowAppSetting && environmentSettings.HasAppSetting(name))
            {
                return true;
            }

            if (parameter.AllowConnectionString && environmentSettings.HasConnectionString(name))
            {
                return true;
            }

            if (parameter.AllowEnvironmentVariable && environmentSettings.HasEnvironmentVariable(name))
            {
                return true;
            }

            return false;
        }

        static bool CanBeResolvedFromSwitches(IEnumerable<Switch> switches, Parameter p)
        {
            return switches.Any(s => p.MatchesKey(s.Key));
        }
    }
}
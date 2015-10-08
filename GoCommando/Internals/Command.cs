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

        public CommandInvoker(string command, Type type, Settings settings)
            : this(command, settings, CreateInstance(type))
        {
        }

        public CommandInvoker(string command, Settings settings, ICommand commandInstance)
        {
            _settings = settings;
            _commandInstance = commandInstance;

            Command = command;
            Parameters = GetParameters(Type);
        }

        static ICommand CreateInstance(Type type)
        {
            return (ICommand)Activator.CreateInstance(type);
        }

        IEnumerable<Parameter> GetParameters(Type type)
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
                    a.ParameterAttribute.DefaultValue))
                .ToList();
        }

        static TAttribute GetSingleAttributeOrNull<TAttribute>(PropertyInfo p) where TAttribute : Attribute
        {
            return p.GetCustomAttributes(typeof(TAttribute), false)
                .Cast<TAttribute>()
                .FirstOrDefault();
        }

        public string Command { get; }

        public Type Type => _commandInstance.GetType();

        public IEnumerable<Parameter> Parameters { get; }

        public string Description => Type.GetCustomAttribute<DescriptionAttribute>()?.DescriptionText ??
                                     "(no help text for this command)";

        public ICommand CommandInstance => _commandInstance;

        public void Invoke(IEnumerable<Switch> switches)
        {
            var commandInstance = _commandInstance;

            var requiredParametersMissing = Parameters
                .Where(p => !p.Optional && !p.HasDefaultValue && !switches.Any(s => s.Key == p.Name))
                .ToList();

            if (requiredParametersMissing.Any())
            {
                var requiredParametersMissingString = string.Join(Environment.NewLine,
                    requiredParametersMissing.Select(p => $"    {_settings.SwitchPrefix}{p.Name}"));

                throw new GoCommandoException($@"The following required parameters are missing:

{requiredParametersMissingString}");
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

                throw new GoCommandoException($@"The following switches do not have a corresponding parameter:

{switchesWithoutMathingParameterString}");
            }

            var setParameters = new HashSet<Parameter>();

            foreach (var switchToSet in switches)
            {
                var correspondingParameter = Parameters.FirstOrDefault(p => p.MatchesKey(switchToSet.Key));

                if (correspondingParameter == null)
                {
                    throw new GoCommandoException($"The switch {_settings}{switchToSet.Key} does not correspond to a parameter of the '{Command}' command!");
                }

                correspondingParameter.SetValue(commandInstance, switchToSet.Value);

                setParameters.Add(correspondingParameter);
            }

            foreach (var parameterWithDefaultValue in Parameters.Where(p => p.HasDefaultValue).Except(setParameters))
            {
                parameterWithDefaultValue.ApplyDefaultValue(commandInstance);
            }

            commandInstance.Run();
        }
    }
}
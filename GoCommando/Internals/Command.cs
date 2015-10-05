using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GoCommando.Internals
{
    class CommandInvoker
    {
        readonly Settings _settings;

        public CommandInvoker(string command, Type type, Settings settings)
        {
            _settings = settings;

            if (!typeof(ICommand).IsAssignableFrom(type))
            {
                throw new ApplicationException($"Command tyep {type} does not implement {typeof(ICommand)} as it should!");
            }

            Command = command;
            Type = type;
            Parameters = GetParameters(Type);
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
                    a.ExampleAttributes.Select(e => e.ExampleValue)))
                .ToList();
        }

        static TAttribute GetSingleAttributeOrNull<TAttribute>(PropertyInfo p) where TAttribute : Attribute
        {
            return p.GetCustomAttributes(typeof(TAttribute), false)
                .Cast<TAttribute>()
                .FirstOrDefault();
        }

        public string Command { get; }
        public Type Type { get; }
        public IEnumerable<Parameter> Parameters { get; }

        public string Description => Type.GetCustomAttribute<DescriptionAttribute>()?.DescriptionText ??
                                     "(no help text for this command)";


        public void Invoke(IEnumerable<Switch> switches)
        {
            var commandInstance = (ICommand)Activator.CreateInstance(Type);

            var requiredParametersMissing = Parameters
                .Where(p => !p.Optional && !switches.Any(s => s.Key == p.Name))
                .ToList();

            if (requiredParametersMissing.Any())
            {
                var requiredParametersMissingString = string.Join(Environment.NewLine,
                    requiredParametersMissing.Select(p => "    " + p.Name));

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

            foreach (var switchToSet in switches)
            {
                var correspondingParameter = Parameters.FirstOrDefault(p => p.MatchesKey(switchToSet.Key));

                correspondingParameter?.SetValue(commandInstance, switchToSet.Value);
            }

            commandInstance.Run();
        }
    }
}
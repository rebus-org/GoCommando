using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCommando.Internals
{
    class CommandInvoker
    {
        public CommandInvoker(string command, Type type)
        {
            if (!typeof (ICommand).IsAssignableFrom(type))
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
                    Attribute = p.GetCustomAttributes(typeof (ParameterAttribute), false)
                        .Cast<ParameterAttribute>()
                        .FirstOrDefault()
                })
                .Where(a => a.Attribute != null)
                .Select(a => new Parameter(a.Property, a.Attribute.Name, a.Attribute.ShortName, a.Attribute.Optional))
                .ToList();
        }

        public string Command { get; }
        public Type Type { get; }
        public IEnumerable<Parameter> Parameters { get; }

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
                        ? $"    {p.Key} = {p.Value}"
                        : $"    {p.Key}"));

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
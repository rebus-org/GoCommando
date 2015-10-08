﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoCommando.Internals;

namespace GoCommando
{
    /// <summary>
    /// Here's how to go commando: 1. <code>Go.Run()</code>
    /// </summary>
    public static class Go
    {
        /// <summary>
        /// Call this method from your <code>Main</code> method
        /// </summary>
        public static void Run()
        {
            try
            {
                var bannerAttribute = Assembly.GetEntryAssembly()?.EntryPoint?.DeclaringType
                    .GetCustomAttribute<BannerAttribute>();

                if (bannerAttribute != null)
                {
                    Console.WriteLine(bannerAttribute.BannerText);
                }

                InnerRun();
            }
            catch (GoCommandoException friendlyException)
            {
                Environment.ExitCode = -1;
                Console.WriteLine(friendlyException.Message);
                Console.WriteLine();
                Console.WriteLine("Invoke with -help <command> to get help for each command.");
                Console.WriteLine();
                Console.WriteLine("Exit code: -1");
                Console.WriteLine();
            }
            catch (CustomExitCodeException customExitCodeException)
            {
                FailAndExit(customExitCodeException, customExitCodeException.ExitCode);
            }
            catch (Exception exception)
            {
                FailAndExit(exception, -2);
            }
        }

        static void FailAndExit(Exception customExitCodeException, int exitCode)
        {
            Console.WriteLine();
            Console.Error.WriteLine(customExitCodeException);
            Console.WriteLine();
            Console.WriteLine("Exit code: {0}", exitCode);
            Console.WriteLine();

            Environment.ExitCode = exitCode;
        }

        static void InnerRun()
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToList();
            var settings = new Settings();
            var arguments = Parse(args, settings);
            var commandTypes = GetCommands(settings);

            var helpSwitch = arguments.Switches.FirstOrDefault(s => s.Key == "?")
                             ?? arguments.Switches.FirstOrDefault(s => s.Key == "help");

            if (helpSwitch != null)
            {
                var exe = Assembly.GetEntryAssembly().GetName().Name + ".exe";

                if (helpSwitch.Value != null)
                {
                    var command = commandTypes.FirstOrDefault(c => c.Command == helpSwitch.Value);

                    if (command != null)
                    {
                        if (command.Parameters.Any())
                        {
                            Console.WriteLine(@"{0}

Type

    {1} {2} <args>

where <args> can consist of the following parameters:

{3}", 
command.Description,
exe, 
command.Command,
string.Join(Environment.NewLine, command.Parameters.Select(parameter => FormatParameter(parameter, settings))));
                        }
                        else
                        {
                            Console.WriteLine(@"Type

    {0} {1}

", exe, command.Command);
                        }
                        return;
                    }

                    throw new GoCommandoException($"Unknown command: '{helpSwitch.Value}'");
                }

                var availableCommands = string.Join(Environment.NewLine, commandTypes.Select(c => $"    {c.Command} - {c.Description}"));

                Console.WriteLine($@"The following commands are available:

{availableCommands}

Type

    {exe} -help <command>

to get help for a command.
");
                return;
            }

            var commandToRun = commandTypes.FirstOrDefault(c => c.Command == arguments.Command);

            if (commandToRun == null)
            {
                var errorText = !string.IsNullOrWhiteSpace(arguments.Command)
                    ? $"Could not find command '{arguments.Command}'"
                    : "Please invoke with a command";

                var availableCommands = string.Join(Environment.NewLine, commandTypes.Select(c => $"    {c.Command} - {c.Description}"));

                throw new GoCommandoException($@"{errorText} - the following commands are available:

{availableCommands}");
            }

            commandToRun.Invoke(arguments.Switches);
        }

        static string FormatParameter(Parameter parameter, Settings settings)
        {
            var shorthand = parameter.Shortname != null
                ? $" / {settings.SwitchPrefix}{parameter.Shortname}"
                : "";

            var additionalProperties = new List<string>();

            var isFlag = parameter.IsFlag;

            if (isFlag)
            {
                additionalProperties.Add("flag");
            }

            if (parameter.Optional)
            {
                additionalProperties.Add("optional");
            }

            var additionalPropertiesText = additionalProperties.Any()
                ? $" ({string.Join("/", additionalProperties)})"
                : "";

            var helpText = "        " + (parameter.DescriptionText ?? "(no help text available)");

            var examplesText = !parameter.ExampleValues.Any()
                ? ""
                : FormatExamples(parameter, settings);

            var switchText = $"{settings.SwitchPrefix}{parameter.Name}{shorthand}{additionalPropertiesText}";

            return $@"    {switchText} 
{helpText}
{examplesText}";
        }

        static string FormatExamples(Parameter parameter, Settings settings)
        {
            var examples = string.Join(Environment.NewLine, parameter.ExampleValues
                .Select(e => $"          {settings.SwitchPrefix}{parameter.Name} {e}"));

            return $@"
        Examples:
{examples}
";
        }

        internal static List<CommandInvoker> GetCommands(Settings settings)
        {
            return Assembly.GetEntryAssembly().GetTypes()
                .Select(t => new
                {
                    Type = t,
                    Attribute = t.GetCustomAttribute<CommandAttribute>()
                })
                .Where(a => a.Attribute != null)
                .Select(a => new CommandInvoker(a.Attribute.Command, a.Type, settings))
                .ToList();
        }

        internal static Arguments Parse(IEnumerable<string> args, Settings settings)
        {
            var list = args.ToList();

            if (!list.Any()) return new Arguments(null, Enumerable.Empty<Switch>(), settings);

            var first = list.First();

            string command;
            List<string> switchArgs;

            if (first.StartsWith(settings.SwitchPrefix))
            {
                command = null;
                switchArgs = list;
            }
            else
            {
                command = first;
                switchArgs = list.Skip(1).ToList();
            }

            var switches = new List<Switch>();

            string key = null;

            foreach (var arg in switchArgs)
            {
                if (arg.StartsWith(settings.SwitchPrefix))
                {
                    if (key != null)
                    {
                        switches.Add(Switch.Flag(key));
                    }

                    key = arg.Substring(settings.SwitchPrefix.Length);

                    if (HasKeyAndValue(key))
                    {
                        var keyAndValue = GetKeyAndValueFromKey(key);
                        if (keyAndValue == null)
                        {
                            throw new ApplicationException($"Expected to get key-value-pair from key '{key}'");
                        }
                        switches.Add(Switch.KeyValue(keyAndValue.Value.Key, keyAndValue.Value.Value));
                        key = null;
                    }

                    continue;
                }

                var value = arg;

                if (key == null)
                {
                    throw new GoCommandoException($"Got command line argument '{value}' without a switch in front of it - please specify switches like this: '{settings.SwitchPrefix}switch some-value'");
                }

                switches.Add(Switch.KeyValue(key, value));

                key = null;
            }

            if (key != null)
            {
                switches.Add(Switch.Flag(key));
            }

            return new Arguments(command, switches, settings);
        }

        static bool HasKeyAndValue(string key)
        {
            return GetKeyAndValueFromKey(key) != null;
        }

        static KeyValuePair<string,string>? GetKeyAndValueFromKey(string key)
        {
            for (var index = 0; index < key.Length; index++)
            {
                var c = key[index];

                if (c == ':')
                {
                    return new KeyValuePair<string, string>(key.Substring(0, index), key.Substring(index + 1));
                }

                if (c == '=')
                {
                    return new KeyValuePair<string, string>(key.Substring(0, index), key.Substring(index + 1));
                }

                if (!char.IsLetter(c))
                {
                    return new KeyValuePair<string, string>(key.Substring(0, index), key.Substring(index));
                }
            }

            return null;
        }
    }
}

using System;
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
            }
            catch (Exception exception)
            {
                Environment.ExitCode = -2;
                Console.Error.WriteLine(exception);
            }
        }

        static void InnerRun()
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToList();
            var arguments = Parse(args, new Settings());
            var commandTypes = GetCommands();

            var commandToRun = commandTypes.FirstOrDefault(c => c.Command == arguments.Command);

            if (commandToRun == null)
            {
                var availableCommands = string.Join(Environment.NewLine, commandTypes.Select(c => "    " + c.Command));

                throw new GoCommandoException($@"Could not find command '{arguments.Command}' - the following commands are available:

{availableCommands}");
            }

            commandToRun.Invoke(arguments.Switches);
        }

        internal static List<CommandInvoker> GetCommands()
        {
            return Assembly.GetEntryAssembly().GetTypes()
                .Select(t => new
                {
                    Type = t,
                    Attribute = t.GetCustomAttribute<CommandAttribute>()
                })
                .Where(a => a.Attribute != null)
                .Select(a => new CommandInvoker(a.Attribute.Command, a.Type))
                .ToList();
        }

        internal static Arguments Parse(IEnumerable<string> args, Settings settings)
        {
            var list = args.ToList();
            var command = list.First();

            if (command.StartsWith(settings.SwitchPrefix))
            {
                throw new GoCommandoException($"Invalid command: '{command}' - the command must not start with the switch prefix '{settings.SwitchPrefix}'");
            }

            var switches = new List<Switch>();

            string key = null;

            foreach (var arg in args.Skip(1))
            {
                if (arg.StartsWith(settings.SwitchPrefix))
                {
                    if (key != null)
                    {
                        switches.Add(Switch.Flag(key));
                    }

                    key = arg.Substring(settings.SwitchPrefix.Length);
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

            return new Arguments(command, switches);
        }
    }
}

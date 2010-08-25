using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using GoCommando.Api;
using GoCommando.Attributes;
using GoCommando.Exceptions;
using GoCommando.Extensions;
using GoCommando.Helpers;
using GoCommando.Parameters;
using Binder=GoCommando.Helpers.Binder;

namespace GoCommando
{
    public static class Go
    {
        /// <summary>
        /// Runs the specified type, GoCommando style. What this means is that:
        ///     * if type is decorated with [Banner(...)], that banner will be output
        ///     * args will be bound to public properties decorated with [PositionalArgument] and [NamedArgument]
        ///     * validation (if any) will be run
        ///     * help text will be shown where appropriate
        /// </summary>
        public static int Run<TCommando>(string[] args) where TCommando : ICommando
        {
            try
            {
                var instance = CreateInstance<TCommando>();

                PossiblyShowBanner(instance);

                if (ShouldShowHelpText(args))
                {
                    ShowHelpText(instance);
                    return 0;
                }

                var parameters = GetParameters(args);

                PopulateProperties(parameters, instance);

                Execute(instance);

                return 0;
            }
            catch (CommandoException e)
            {
                Write(e.Message);

                return 2;
            }
            catch (Exception e)
            {
                Write(e.ToString());

                return 1;
            }
        }

        static void ShowHelpText(ICommando commando)
        {
            var helper = new Helper();
            var parameters = helper.GetParameters(commando);

            var exeName = Assembly.GetEntryAssembly().GetName().Name + ".exe";
            var parameterList = string.Join(" ", parameters.Where(p => p.Position > 0)
                                                     .Select(p => string.Format("[{0}]", p.Position))
                                                     .ToArray());

            var thereAreRequiredArguments = parameters.Any(p => p.Position > 0);
            var thereAreOptionalArguments = parameters.Any(p => p.Position == 0);

            Write("Usage:");
            Write();
            Write("\t{0} {1}{2}",
                  exeName,
                  parameterList,
                  thereAreOptionalArguments ? " [...]" : "");

            if (thereAreRequiredArguments)
            {
                Write();
                Write("Required arguments:");
                Write();

                foreach (var parameter in parameters.Where(p => p.Position > 0))
                {
                    Write("\t[{0}] {1}", parameter.Position, parameter.Description);
                }
            }

            if (thereAreOptionalArguments)
            {
                Write();
                Write("Optional arguments:");
                Write();

                foreach (var parameter in parameters.Where(p => p.Position == 0))
                {
                    Write("\t/{0}\t{1}", parameter.Name, parameter.Description);
                }
            }
        }

        static bool ShouldShowHelpText(string[] strings)
        {
            return strings.Length == 1
                   && new List<string> {"-h", "--h", "/h", "-?", "/?"}.Contains(strings[0].ToLowerInvariant());
        }

        static ICommando CreateInstance<TCommando>()
        {
            var factory = new DefaultCommandoFactory();
            return factory.Create(typeof(TCommando));
        }

        static void PossiblyShowBanner(object obj)
        {
            var type = obj.GetType();
            type.WithAttributes<BannerAttribute>(ShowBanner);
            Write();
        }

        static void ShowBanner(BannerAttribute attribute)
        {
            Write(attribute.Text);
        }

        static void Write()
        {
            Console.WriteLine();
        }

        static void Write(string text, params object[] objs)
        {
            Console.WriteLine(text, objs);
        }

        static List<CommandLineParameter> GetParameters(string[] args)
        {
            var parser = new ArgParser();
            return parser.Parse(args);
        }

        static void PopulateProperties(IEnumerable<CommandLineParameter> parameters, ICommando instance)
        {
            var binder = new Binder();
            binder.Bind(instance, parameters);
        }

        static void Execute(ICommando instance)
        {
            instance.Run();
        }
    }
}
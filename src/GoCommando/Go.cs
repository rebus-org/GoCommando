using System;
using System.Collections.Generic;
using GoCommando.Api;
using GoCommando.Attributes;
using GoCommando.Exceptions;
using GoCommando.Extensions;
using GoCommando.Helpers;
using GoCommando.Parameters;

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

        static ICommando CreateInstance<TCommando>()
        {
            var factory = new DefaultCommandoFactory();
            return factory.Create(typeof(TCommando));
        }

        static void PossiblyShowBanner(object obj)
        {
            var type = obj.GetType();
            type.WithAttributes<BannerAttribute>(ShowBanner);
        }

        static void ShowBanner(BannerAttribute attribute)
        {
            Write(attribute.Text);
        }

        static void Write(string text)
        {
            Console.WriteLine(text);
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
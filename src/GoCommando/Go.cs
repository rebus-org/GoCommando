using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoCommando.Api;
using GoCommando.Attributes;
using GoCommando.Exceptions;
using GoCommando.Extensions;
using GoCommando.Helpers;
using GoCommando.Parameters;
using Binder = GoCommando.Helpers.Binder;

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

                var bindingReport = PopulateProperties(parameters, instance);

                if (RequiredParameterMissing(bindingReport))
                {
                    ShowMissingParameters(bindingReport);

                    return 1;
                }

                Execute(instance);

                return 0;
            }
            catch (CommandoException e)
            {
                Write(e.Message);

                WriteHelpInstructions();

                return 2;
            }
            catch (Exception e)
            {
                Write(e.ToString());

                return 1;
            }
        }

        static void ShowMissingParameters(BindingReport report)
        {
            Write("One or more required arguments are missing!");
            Write();

            var context = new BindingContext();

            foreach (var parameter in report.PropertiesNotBound.Select(p => new Parameter(p, context)).Where(p => p.Position > 0))
            {
                WritePositionalParameter(parameter, ExampleOutputSettings.DontShowExamples);
            }

            WriteHelpInstructions();
        }

        static void WriteHelpInstructions()
        {
            Write();
            Write("Invoke with /? for detailed help.");
        }

        class BindingContext : IHasPositionerCounter
        {
            public BindingContext()
            {
                Position = 1;
            }

            public int Position { get; set; }
        }

        static bool RequiredParameterMissing(BindingReport bindingReport)
        {
            return bindingReport.RequiredPropertiesNotBound.Any();
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
            Write("\t{0} {1}{2}{3}",
                  exeName,
                  parameterList,
                  parameterList.Length > 0 ? " " : "",
                  thereAreOptionalArguments ? "[args]" : "");

            if (thereAreRequiredArguments)
            {
                Write();
                Write();
                Write("Required arguments:");

                foreach (var parameter in parameters.Where(p => p.Position > 0))
                {
                    Write();

                    WritePositionalParameter(parameter, ExampleOutputSettings.ShowExamples);
                }
            }

            if (thereAreOptionalArguments)
            {
                Write();
                Write();
                Write("Additional arguments:");

                foreach (var parameter in parameters.Where(p => p.Position == 0))
                {
                    Write();

                    Write("\t/{0}\t{1}", parameter.Name, parameter.Description);

                    PossibleWriteExamples(parameter);
                }
            }
        }

        enum ExampleOutputSettings
        {
            ShowExamples,
            DontShowExamples,
        }

        static void WritePositionalParameter(Parameter parameter, ExampleOutputSettings outputExamples)
        {
            Write("\t[{0}] {1}", parameter.Position, parameter.Description);

            if (outputExamples == ExampleOutputSettings.ShowExamples)
            {
                PossibleWriteExamples(parameter);
            }
        }

        static void PossibleWriteExamples(Parameter parameter)
        {
            var examples = parameter.Examples;

            if (!examples.Any()) return;

            var headline = examples.Count > 1 ? "Examples" : "Example";
            var maxLength = examples.Max(e => e.Text.Length);
            var printExamplesOnMultipleLines = maxLength > 7;
            var separator = printExamplesOnMultipleLines ? (Environment.NewLine + "\t\t\t") : ", ";

            Write("\t\t{0}: {1}{2}", 
                headline, 
                printExamplesOnMultipleLines ? separator : "",
                string.Join(separator, examples.Select(e => e.Text).ToArray()));
        }

        static bool ShouldShowHelpText(string[] strings)
        {
            return strings.Length == 1
                   && new List<string> {"-h", "--h", "/h", "-?", "/?", "?"}.Contains(strings[0].ToLowerInvariant());
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

        static BindingReport PopulateProperties(IEnumerable<CommandLineParameter> parameters, ICommando instance)
        {
            var binder = new Binder();
            var report = binder.Bind(instance, parameters);
            return report;
        }

        static void Execute(ICommando instance)
        {
            instance.Run();
        }
    }
}
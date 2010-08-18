using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCommando
{
    public static class Go
    {
        public static int Run<TGoCommando>(string[] args) where TGoCommando : IGoCommando
        {
            try
            {
                PossiblyShowBanner(typeof(TGoCommando));
                var instance = (IGoCommando)Activator.CreateInstance(typeof(TGoCommando));
                PopulateInstanceParameters(instance, args);
                instance.Run();
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

        static void PossiblyShowBanner(Type type)
        {
            type.WithAttributes<BannerAttribute>(ShowBanner);
        }

        static void ShowBanner(BannerAttribute attribute)
        {
            Write(attribute.Text);
        }

        static void PopulateInstanceParameters(IGoCommando commando, string[] args)
        {
            var parser = new ArgParser();
            var parameters = parser.Parse(args);
            var matches = Match(parameters, commando.GetType().GetAttributes<ArgumentAttribute>());
            
        }

        static List<ArgumentMatch> Match(List<CommandLineParameter> parameters, IEnumerable<ArgumentAttribute> attributes)
        {
            var positionalAttributes = attributes.Where(a => a is PositionalArgumentAttribute).Cast<PositionalArgumentAttribute>();
            var otherAttributes = attributes.Where(a => a is NamedArgumentAttribute).Cast<NamedArgumentAttribute>();
            var positionalParameters = parameters.Where(p => p is PositionalCommandLineParameter).Cast<PositionalCommandLineParameter>();
            var otherParameters = parameters.Where(p => p is NamedCommandLineParameter).Cast<NamedCommandLineParameter>();
            var matches = new List<ArgumentMatch>();

            //drop alt det med positional args!!

            return matches;
        }

        static void Write(string text)
        {
            Console.WriteLine(text);
        }
    }

    public class ArgumentMatch
    {
    }
}
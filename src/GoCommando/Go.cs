using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            
            foreach(var property in commando.GetType().GetProperties().Where(ShouldBeBound))
            {
                Bind(commando, property, parameters, property.GetAttributes<ArgumentAttribute>().Single());
            }
        }

        static void Bind(IGoCommando commando, PropertyInfo property, List<CommandLineParameter> parameters, ArgumentAttribute attribute)
        {
            if(attribute is PositionalArgumentAttribute)
            {
                BindPositional(commando, property, parameters, (PositionalArgumentAttribute) attribute);
            }
            else if (attribute is NamedArgumentAttribute)
            {
                BindNamed(commando, property, parameters, (NamedArgumentAttribute) attribute);
            }
            else
            {
                throw new CommandoException("Don't know the attribute type {0}", attribute.GetType().Name);
            }
        }

        static void BindNamed(IGoCommando commando, PropertyInfo property, List<CommandLineParameter> parameters, NamedArgumentAttribute attribute)
        {
            var name = attribute.Name;
            var shortHand = attribute.ShortHand;
            var parameter = parameters.Where(p => p is NamedCommandLineParameter)
                .Cast<NamedCommandLineParameter>()
                .SingleOrDefault(p => p.Name == name || p.Name == shortHand);

            if (parameter == null)
            {
                throw new CommandoException("Could not find parameter matching required positional parameter named {0}", name);
            }

            property.SetValue(commando, Convert.ChangeType(parameter.Value, property.PropertyType), null);
        }

        static void BindPositional(IGoCommando commando, PropertyInfo property, List<CommandLineParameter> parameters, PositionalArgumentAttribute attribute)
        {
            var position = attribute.Index;
            var parameter = parameters
                .Where(p => p is PositionalCommandLineParameter)
                .Cast<PositionalCommandLineParameter>()
                .SingleOrDefault(p => p.Index == position);

            if (parameter == null)
            {
                throw new CommandoException(
                    "Could not find parameter matching required positional parameter at index {0}", position);
            }

            property.SetValue(commando, Convert.ChangeType(parameter.Value, property.PropertyType), null);
        }

        static bool ShouldBeBound(PropertyInfo info)
        {
            return info.HasAttribute<ArgumentAttribute>();
        }

        static void Write(string text)
        {
            Console.WriteLine(text);
        }
    }
}
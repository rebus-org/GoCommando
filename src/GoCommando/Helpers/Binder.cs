using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoCommando.Attributes;
using GoCommando.Exceptions;
using GoCommando.Parameters;

namespace GoCommando.Helpers
{
    public class Binder
    {
        public BindingReport Bind(object targetObjectWithAttributes, IEnumerable<CommandLineParameter> parametersToBind)
        {
            var context = new BindingContext();
            var helper = new Helper();

            foreach (var parameter in helper.GetParameters(targetObjectWithAttributes))
            {
                Bind(targetObjectWithAttributes, parameter.PropertyInfo, parametersToBind.ToList(),
                     parameter.ArgumentAttribute,
                     context);
            }

            return context.Report;
        }

        class BindingContext
        {
            readonly BindingReport report = new BindingReport();

            public int Position { get; set; }

            public BindingReport Report
            {
                get { return report; }
            }
        }

        void Bind(object commando, PropertyInfo property, List<CommandLineParameter> parameters, ArgumentAttribute attribute, BindingContext context)
        {
            if (attribute is PositionalArgumentAttribute)
            {
                BindPositional(commando, property, parameters, (PositionalArgumentAttribute) attribute, context);
                context.Position++;
            }
            else if (attribute is NamedArgumentAttribute)
            {
                BindNamed(commando, property, parameters, (NamedArgumentAttribute) attribute, context);
            }
            else
            {
                throw Ex("Don't know the attribute type {0}", attribute.GetType().Name);
            }
        }

        void BindNamed(object commando, PropertyInfo property, List<CommandLineParameter> parameters, NamedArgumentAttribute attribute, BindingContext context)
        {
            var name = attribute.Name;
            var shortHand = attribute.ShortHand;
            var parameter = parameters.Where(p => p is NamedCommandLineParameter)
                .Cast<NamedCommandLineParameter>()
                .SingleOrDefault(p => p.Name == name || p.Name == shortHand);

            if (parameter == null)
            {
                context.Report.PropertiesNotBound.Add(property);

                if (!attribute.Required) return;

                throw Ex("Could not find parameter matching required parameter named {0}", name);
            }

            property.SetValue(commando, Mutate(parameter, property), null);

            context.Report.PropertiesBound.Add(property);
        }

        void BindPositional(object commando, PropertyInfo property, List<CommandLineParameter> parameters, PositionalArgumentAttribute attribute, BindingContext context)
        {
            var parameter = parameters
                .Where(p => p is PositionalCommandLineParameter)
                .Cast<PositionalCommandLineParameter>()
                .SingleOrDefault(p => p.Index == context.Position);

            if (parameter == null)
            {
                context.Report.PropertiesNotBound.Add(property);

                if (!attribute.Required) return;

                throw Ex("Could not find parameter matching required positional parameter at index {0}", context.Position);
            }

            property.SetValue(commando, Mutate(parameter, property), null);

            context.Report.PropertiesBound.Add(property);
        }

        object Mutate(CommandLineParameter parameter, PropertyInfo property)
        {
            var value = parameter.Value;
            var propertyType = property.PropertyType;

            try
            {
                return Convert.ChangeType(value, propertyType);
            }
            catch(Exception e)
            {
                throw Ex("Could not automatically turn '{0}' into a value of type {1}", value,
                         propertyType.Name);
            }
        }

        CommandoException Ex(string message, params object[] objs)
        {
            return new CommandoException(message, objs);
        }
    }
}
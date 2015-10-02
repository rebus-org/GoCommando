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
            var parameters = helper.GetParameters(targetObjectWithAttributes);
            
            foreach (var parameter in parameters)
            {
                Bind(targetObjectWithAttributes, parameter.PropertyInfo, parametersToBind.ToList(),
                     parameter.ArgumentAttribute,
                     context);
            }

            return context.Report;
        }

        class BindingContext
        {
            public BindingContext()
            {
                Position = 1;
            }

            readonly BindingReport report = new BindingReport();

            public int Position { get; private set; }

            public void IncrementPosition()
            {
                Position++;
            }

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

            var value = parameter != null
                            ? Mutate(parameter, property)
                            : attribute.Default != null
                                  ? Mutate(attribute.Default, property)
                                  : null;

            if (value == null)
            {
                context.Report.PropertiesNotBound.Add(property);

                if (!attribute.Required) return;

                throw Ex("Could not find parameter matching required parameter named {0}", name);
            }

            property.SetValue(commando, value, null);

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
                context.Report.RequiredPropertiesNotBound.Add(property);

                if (!attribute.Required) return;

                throw Ex("Could not find parameter matching required positional parameter at index {0}", context.Position);
            }

            property.SetValue(commando, Mutate(parameter, property), null);

            context.Report.PropertiesBound.Add(property);
            context.IncrementPosition();
        }

        object Mutate(CommandLineParameter parameter, PropertyInfo property)
        {
            return Mutate(parameter.Value, property);
        }

        object Mutate(string value, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            try
            {
                return Convert.ChangeType(value, propertyType);
            }
            catch(Exception)
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
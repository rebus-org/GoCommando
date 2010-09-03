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
        public void Bind(object targetObjectWithAttributes, IEnumerable<CommandLineParameter> parametersToBind)
        {
            var context = new BindingContext();
            var helper = new Helper();

            foreach (var parameter in helper.GetParameters(targetObjectWithAttributes))
            {
                Bind(targetObjectWithAttributes, parameter.PropertyInfo, parametersToBind.ToList(),
                     parameter.ArgumentAttribute,
                     context);
            }
        }

        class BindingContext
        {
            public int Position { get; set; }
        }

        void Bind(object commando, PropertyInfo property, List<CommandLineParameter> parameters, ArgumentAttribute attribute, BindingContext context)
        {
            if (attribute is PositionalArgumentAttribute)
            {
                context.Position++;
                BindPositional(commando, property, parameters, (PositionalArgumentAttribute) attribute, context.Position);
            }
            else if (attribute is NamedArgumentAttribute)
            {
                BindNamed(commando, property, parameters, (NamedArgumentAttribute) attribute);
            }
            else
            {
                throw Ex("Don't know the attribute type {0}", attribute.GetType().Name);
            }
        }

        void BindNamed(object commando, PropertyInfo property, List<CommandLineParameter> parameters, NamedArgumentAttribute attribute)
        {
            var name = attribute.Name;
            var shortHand = attribute.ShortHand;
            var parameter = parameters.Where(p => p is NamedCommandLineParameter)
                .Cast<NamedCommandLineParameter>()
                .SingleOrDefault(p => p.Name == name || p.Name == shortHand);

            if (parameter == null)
            {
                if (!attribute.Required) return;

                throw Ex("Could not find parameter matching required parameter named {0}", name);
            }

            property.SetValue(commando, Convert.ChangeType(parameter.Value, property.PropertyType), null);
        }

        void BindPositional(object commando, PropertyInfo property, List<CommandLineParameter> parameters, PositionalArgumentAttribute attribute, int position)
        {
            var parameter = parameters
                .Where(p => p is PositionalCommandLineParameter)
                .Cast<PositionalCommandLineParameter>()
                .SingleOrDefault(p => p.Index == position);

            if (parameter == null)
            {
                if (!attribute.Required) return;

                throw Ex("Could not find parameter matching required positional parameter at index {0}", position);
            }

            property.SetValue(commando, Convert.ChangeType(parameter.Value, property.PropertyType), null);
        }

        CommandoException Ex(string message, params object[] objs)
        {
            return new CommandoException(message, objs);
        }
    }
}
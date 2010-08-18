using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCommando
{
    public class ArgParser
    {
        class ParserContext
        {
            public ParserContext()
            {
                ParsingPositionalParameters = true;
            }

            public int Index { get; set; }
            public bool ParsingPositionalParameters { get; set; }
        }

        public List<CommandLineParameter> Parse(string[] args)
        {
            var context = new ParserContext();

            return args
                .Where(s => s != null && s.Trim() != "")
                .Select(arg => ToCommandLineParameter(arg, context)).ToList();
        }

        CommandLineParameter ToCommandLineParameter(string arg, ParserContext context)
        {
            return
                !arg.StartsWith("/")
                    ? CreatePositionalCommandLineParameter(arg, context)
                    : CreateNamedCommandLineParameter(arg, context);
        }

        PositionalCommandLineParameter CreatePositionalCommandLineParameter(string arg, ParserContext context)
        {
            if (!context.ParsingPositionalParameters)
            {
                throw new FormatException(string.Format("Named parameters cannot be followed by positional parameters"));
            }

            return new PositionalCommandLineParameter(context.Index++, arg);
        }

        CommandLineParameter CreateNamedCommandLineParameter(string arg, ParserContext context)
        {
            context.ParsingPositionalParameters = false;

            // trim /
            arg = arg.Substring(1);

            var tokens = arg.Split(':');

            if (tokens.Length == 1)
            {
                return new NamedFlagCommandLineParameter(tokens.Single());
            }

            return new NamedCommandLineParameter(tokens.First(), string.Join("", tokens.Skip(1).ToArray()));
        }
    }
}
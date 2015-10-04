using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCommando.Internals
{
    class Arguments
    {
        readonly Settings _settings;

        public Arguments(string command, IEnumerable<Switch> switches, Settings settings)
        {
            _settings = settings;
            var switchList = switches.ToList();
            var duplicateSwitchKeys = switchList.GroupBy(s => s.Key).Where(g => g.Count() > 1).ToList();

            if (duplicateSwitchKeys.Any())
            {
                var dupes = string.Join(", ", duplicateSwitchKeys.Select(g => $"{settings.SwitchPrefix}{g.Key}"));

                throw new GoCommandoException($"The following switches have been specified more than once: {dupes}");
            }

            Command = command;
            Switches = switchList;
        }

        public string Command { get; }

        public IEnumerable<Switch> Switches { get; }

        public TValue Get<TValue>(string key)
        {
            var desiredType = typeof(TValue);

            try
            {
                if (desiredType == typeof(bool))
                {
                    return (TValue)Convert.ChangeType(Switches.Any(s => s.Key == key), desiredType);
                }

                var relevantSwitch = Switches.FirstOrDefault(s => s.Key == key);

                if (relevantSwitch != null)
                {
                    return (TValue)Convert.ChangeType(relevantSwitch.Value, desiredType);
                }

                throw new GoCommandoException($"Could not find switch '{key}'");
            }
            catch (Exception exception)
            {
                throw new FormatException($"Could not get switch '{key}' as a {desiredType}", exception);
            }        }

        public override string ToString()
        {
            return $@"{Command}

{string.Join(Environment.NewLine, Switches.Select(s => "    " + s))}";
        }
    }
}
// ReSharper disable LoopCanBeConvertedToQuery
namespace GoCommando.Internals
{
    class Switch
    {
        static readonly char[] AcceptedQuoteCharacters = { '"', '\'' };

        public static Switch KeyValue(string key, string value)
        {
            return new Switch(key, value);
        }

        public static Switch Flag(string key)
        {
            return new Switch(key, null);
        }

        Switch(string key, string value)
        {
            Key = key;
            Value = Unquote(value);
        }

        static string Unquote(string value)
        {
            if (value == null) return null;

            // can't be quoted
            if (value.Length < 2) return value;

            foreach (var quoteChar in AcceptedQuoteCharacters)
            {
                var quote = quoteChar.ToString();

                if (value.StartsWith(quote)
                    && value.EndsWith(quote))
                {
                    return value.Substring(1, value.Length - 2);
                }
            }

            return value;
        }

        public string Key { get; }
        public string Value { get; }

        public override string ToString()
        {
            return Value == null
                ? Key
                : $"{Key} = {Value}";
        }
    }
}
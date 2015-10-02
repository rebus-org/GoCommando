namespace GoCommando.Internals
{
    class Switch
    {
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
            Value = value;
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
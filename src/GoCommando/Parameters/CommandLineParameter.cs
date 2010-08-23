namespace GoCommando.Parameters
{
    public abstract class CommandLineParameter
    {
        protected CommandLineParameter(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
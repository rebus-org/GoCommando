namespace GoCommando.Parameters
{
    public class NamedCommandLineParameter : CommandLineParameter
    {
        public NamedCommandLineParameter(string name, string value) : base(value)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("/{0}:{1}", Name, Value);
        }
    }
}
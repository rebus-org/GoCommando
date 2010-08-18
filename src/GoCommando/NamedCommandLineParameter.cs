namespace GoCommando
{
    class NamedCommandLineParameter : CommandLineParameter
    {
        public NamedCommandLineParameter(string name, string value) : base(value)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
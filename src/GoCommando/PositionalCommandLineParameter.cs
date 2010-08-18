namespace GoCommando
{
    class PositionalCommandLineParameter : CommandLineParameter
    {
        public PositionalCommandLineParameter(int index, string value) : base(value)
        {
            Index = index;
        }

        public int Index { get; set; }
    }
}
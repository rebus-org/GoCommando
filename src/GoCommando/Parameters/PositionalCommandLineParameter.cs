namespace GoCommando.Parameters
{
    public class PositionalCommandLineParameter : CommandLineParameter
    {
        public PositionalCommandLineParameter(int index, string value) : base(value)
        {
            Index = index;
        }

        public int Index { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]: {1}", Index, Value);
        }
    }
}
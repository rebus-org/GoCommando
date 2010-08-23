using System;

namespace GoCommando.ConsoleApplication
{
    [Banner(@"This is a sample command line app

(c) Some Dude 2010")]
    class Program : IGoCommando
    {
        [PositionalArgument(0)]
        public string Path { get; set; }

        [PositionalArgument(1)]
        public double Value { get; set; }

        [NamedArgument("someflag", "sf")]
        public bool SomeFlag { get; set; }

        [NamedArgument("anotherflag", "af")]
        public bool AnotherFlag { get; set; }

        [NamedArgument("someValue", "sv")]
        public int SomeValue { get; set; }

        [NamedArgument("anotherValue", "av")]
        public string AnotherValue { get; set; }

        static void Main(string[] args)
        {
            Go.Run<Program>(args);
        }

        public void Run()
        {
            Console.WriteLine(@"Path: {0} (expected: c:\program files\somepath\somefile.txt)
Value: {1} (expected 34.67)
Some flag: {2} (expected True)
Some value: {3} (expected 23))
Another value: {4} (expected bim)
Another flag: {5} (expected False)", 
                   Path, 
                   Value,
                              SomeFlag, 
                              SomeValue, 
                              AnotherValue);
        }
    }
}

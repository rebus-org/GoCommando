using System;
using GoCommando.Api;
using GoCommando.Attributes;

namespace GoCommando.ConsoleApplication
{
    [Banner(@"This is a sample command line app

(c) Some Dude 2010")]
    class Program : ICommando
    {
        [PositionalArgument]
        [Description("The path to the file you wish to do stuff to")]
        [Example("c:\\temp\\somefile.txt")]
        public string Path { get; set; }

        [PositionalArgument]
        [Description("The value of something")]
        public double Value { get; set; }

        [NamedArgument("someflag", "sf")]
        [Description("Extra-special flag")]
        public bool SomeFlag { get; set; }

        [NamedArgument("anotherflag", "af")]
        [Description("Pretty important flag")]
        public bool AnotherFlag { get; set; }

        [NamedArgument("someValue", "sv")]
        [Description("An integer value specifying something important")]
        [Example("1"), Example("2"), Example("3")]
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
                              AnotherValue,
                              AnotherFlag);
        }
    }
}

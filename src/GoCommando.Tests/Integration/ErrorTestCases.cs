using GoCommando.Api;
using GoCommando.Attributes;
using NUnit.Framework;
using DescriptionAttribute = NUnit.Framework.DescriptionAttribute;

namespace GoCommando.Tests.Integration
{
    [TestFixture]
    public class ErrorTestCases
    {
        [SetUp]
        public void SetUp()
        {
            CommandoWithTwoArguments.SomePathStatic = null;
            CommandoWithTwoArguments.AnotherPathStatic = null;
        }

        [Test, Description("This error was due to named argument syntax using slash, thus resulting in arguments with forward slash (e.g. an URL) being interpreted as such")]
        public void ReproduceError()
        {
            var errorCode = Go.Run<CommandoWithTwoArguments>(new[] {"/src/somepath", "/src/anotherpath"});
            
            Assert.AreEqual(0, errorCode);
            Assert.AreEqual("/src/somepath", CommandoWithTwoArguments.SomePathStatic);
            Assert.AreEqual("/src/anotherpath", CommandoWithTwoArguments.AnotherPathStatic);
        }

        class CommandoWithTwoArguments : ICommando
        {
            [PositionalArgument]
            public string SomePath { get; set; }

            [PositionalArgument]
            public string AnotherPath { get; set; }

            public void Run()
            {
                SomePathStatic = SomePath;
                AnotherPathStatic = AnotherPath;
            }

            public static string SomePathStatic { get; set; }
            public static string AnotherPathStatic { get; set; }
        }

        [Test, Description("This was the actual parameters resulting in an error")]
        public void HowAboutThis()
        {
            var errorCode = Go.Run<CommandoWithThreeArguments>(new[]
                                                   {
                                                       @"src\Noder.Test\bin\Debug\Noder.Test.dll",
                                                       @"src\Noder.Test\Features\00_basics.feature"
                                                   });

            Assert.AreEqual(0, errorCode);
            Assert.AreEqual(@"src\Noder.Test\bin\Debug\Noder.Test.dll", CommandoWithThreeArguments.SomePathStatic);
            Assert.AreEqual(@"src\Noder.Test\Features\00_basics.feature", CommandoWithThreeArguments.AnotherPathStatic);
            Assert.IsFalse(CommandoWithThreeArguments.SomeFlagStatic);
        }

        class CommandoWithThreeArguments : ICommando
        {
            [PositionalArgument]
            public string SomePath { get; set; }

            [PositionalArgument]
            public string AnotherPath { get; set; }

            [NamedArgument("flag", "f")]
            public bool SomeFlag { get; set; }

            public void Run()
            {
                SomePathStatic = SomePath;
                AnotherPathStatic = AnotherPath;
                SomeFlagStatic = SomeFlag;
            }

            public static string SomePathStatic { get; set; }
            public static string AnotherPathStatic { get; set; }
            public static bool SomeFlagStatic { get; set; }
        }
    }
}
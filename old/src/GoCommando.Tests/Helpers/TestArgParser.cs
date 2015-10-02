using System;
using System.Linq;
using System.Timers;
using GoCommando.Helpers;
using GoCommando.Parameters;
using NUnit.Framework;

namespace GoCommando.Tests.Helpers
{
    [TestFixture]
    public class TestArgParser : FixtureBase
    {
        ArgParser parser;

        protected override void DoSetUp()
        {
            parser = new ArgParser();
        }

        [Test]
        public void CanParseParameterWithAbsolutePath()
        {
            var parameter = parser.Parse(new[]{"-path:c:\\temp\\someFile.txt"})
                .Cast<NamedCommandLineParameter>().Single();

            Assert.AreEqual("path", parameter.Name);
            Assert.AreEqual("c:\\temp\\someFile.txt", parameter.Value);
        }

        [Test]
        public void IndexIsOneBased()
        {
            var parameters = parser.Parse(new[]{"firstArg", "secondArg"})
                .Cast<PositionalCommandLineParameter>().ToList();

            Assert.AreEqual(1, parameters[0].Index);
            Assert.AreEqual(2, parameters[1].Index);
        }

        [Test]
        public void EmptyArrayYieldsNoArguments()
        {
            var timer = new Timer();
            timer.Elapsed += (_, __) => Console.WriteLine("hej");
            timer.Start();

            var args1 = new string[0];
            var args2 = new[]{"", "  ", ""};
            AssertYieldsNoParameters(args1);
            AssertYieldsNoParameters(args2);
        }

        [Test]
        public void DoesNotThrowIfItsAFlag()
        {
            var parameter = parser.Parse(new[]{"-param"}).Cast<NamedCommandLineParameter>().Single();
            Assert.AreEqual("param", parameter.Name);
            Assert.AreEqual("True", parameter.Value);
        }

        [Test]
        public void CanParseArrayOfArguments()
        {
            var args = new []{"some.assembly", "another.assembly", "-param2:bimmelim", "-param1:boom"};
            var parameters = parser.Parse(args);

            Assert.AreEqual(4, parameters.Count);

            var p1 = parameters[0];
            var p2 = parameters[1];
            var p3 = parameters[2];
            var p4 = parameters[3];
            Assert.AreEqual("some.assembly", p1.Value);
            Assert.AreEqual("another.assembly", p2.Value);
            Assert.AreEqual("bimmelim", p3.Value);
            Assert.AreEqual("boom", p4.Value);
        }

        [Test]
        public void ThrowsIfPositionalParametersAreMixedWithNamedParameters()
        {
            Assert.Throws<FormatException>(() => parser.Parse(new[] {"-named:parameter", "positional.parameter"}));
            Assert.Throws<FormatException>(() => parser.Parse(new[] {"-named.flag", "positional.parameter"}));
        }

        [Test]
        public void AcceptsSlashAsPositionalParameter()
        {
            var args = new []{"/src/somepath", "/src/anotherpath"};
            var parameters = parser.Parse(args);

            Assert.AreEqual(2, parameters.Count);
            Assert.IsTrue(parameters[0] is PositionalCommandLineParameter);
            Assert.IsTrue(parameters[1] is PositionalCommandLineParameter);
        }

        void AssertYieldsNoParameters(string[] args)
        {
            var parameters = parser.Parse(args);
            var argsAsString = string.Join(", ", args.Select(s => string.Format(@"""{0}""", s)).ToArray());

            Assert.AreEqual(0, parameters.Count, "Expected 0 parameters yielded from parsing {0}",
                            argsAsString);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using GoCommando.Internals;
using NUnit.Framework;

namespace GoCommando.Tests
{
    [TestFixture]
    public class TestArgParser
    {
        [Test]
        public void CanReturnSimpleCommand()
        {
            var arguments = Parse(new[] { "run" });

            Assert.That(arguments.Command, Is.EqualTo("run"));
        }

        [Test]
        public void CommandIsNullWhenNoCommandIsGiven()
        {
            var arguments = Parse(new[] { "-file", @"""C:\temp\file.json""" });

            Assert.That(arguments.Command, Is.Null);
        }

        [Test, Ignore("arguments.Command should just be null")]
        public void DoesNotAcceptSwitchAsCommand()
        {
            var ex = Assert.Throws<GoCommandoException>(() =>
            {
                Parse(new[] { "-file", @"""C:\temp\file.json""" });
            });

            Console.WriteLine(ex);
        }

        [Test]
        public void CanParseOrdinaryArguments()
        {
            var args = @"run
-path
c:\Program Files
-dir
c:\Windows\Microsoft.NET\Framework
-flag
-moreflag".Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var arguments = Parse(args);

            Console.WriteLine(arguments);

            Assert.That(arguments.Command, Is.EqualTo("run"));
            Assert.That(arguments.Switches.Count(), Is.EqualTo(4));
            Assert.That(arguments.Get<string>("path"), Is.EqualTo(@"c:\Program Files"));
            Assert.That(arguments.Get<string>("dir"), Is.EqualTo(@"c:\Windows\Microsoft.NET\Framework"));

            Assert.That(arguments.Get<bool>("flag"), Is.True);
            Assert.That(arguments.Get<bool>("moreflag"), Is.True);
            Assert.That(arguments.Get<bool>("flag_not_specified_should_default_to_false"), Is.False);
        }

        [TestCase(@"-path:""c:\temp""")]
        [TestCase(@"-path=""c:\temp""")]
        [TestCase(@"-path""c:\temp""")]
        public void SupportsVariousSingleTokenAliases(string alias)
        {
            var arguments = Parse(new[] { alias });

            Assert.That(arguments.Switches.Count(), Is.EqualTo(1));
            Assert.That(arguments.Switches.Single().Key, Is.EqualTo("path"));
            Assert.That(arguments.Switches.Single().Value, Is.EqualTo(@"c:\temp"));
        }

        [TestCase(@"-n23")]
        public void SupportsShortFormWithNumber(string alias)
        {
            var arguments = Parse(new[] { alias });

            Assert.That(arguments.Switches.Count(), Is.EqualTo(1));
            Assert.That(arguments.Switches.Single().Key, Is.EqualTo("n"));
            Assert.That(arguments.Switches.Single().Value, Is.EqualTo(@"23"));
        }

        static Arguments Parse(IEnumerable<string> args)
        {
            return Go.Parse(args, new Settings());
        }
    }
}

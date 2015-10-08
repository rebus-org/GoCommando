using GoCommando.Internals;
using NUnit.Framework;

namespace GoCommando.Tests
{
    [TestFixture]
    public class TestCommand
    {
        [TestCase("-switch:value2")]
        [TestCase("-switch=value2")]
        [TestCase(@"-switch""value2""")]
        public void CanCorrectlyHandleDifferentAlternativeSwitchFormatsFoundInOneSingleTokenOnly(string switchText)
        {
            var settings = new Settings();
            var invoker = new CommandInvoker("bimse", settings, new Bimse());
            var arguments = Go.Parse(new[] {switchText}, settings);

            invoker.Invoke(arguments.Switches);

            var bimseInstance = (Bimse)invoker.CommandInstance;

            Assert.That(bimseInstance.Switch, Is.EqualTo("value2"));
        }

        [Command("bimse")]
        class Bimse : ICommand
        {
            [Parameter("switch")]
            public string Switch { get; set; }

            public void Run()
            {
            }
        }
    }
}
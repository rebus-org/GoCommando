using System.Collections.Generic;
using System.Linq;
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
            var arguments = Go.Parse(new[] { switchText }, settings);

            invoker.Invoke(arguments.Switches, EnvironmentSettings.Empty);

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

        [Test]
        public void CanGetParameterFromAppSettingsAndConnectionStrings()
        {
            var invoker = new CommandInvoker("null", typeof(CanUseAppSetting), new Settings());

            var appSettings = new Dictionary<string, string>
            {
                {"my-setting", "my-value"}
            };

            var connectionStrings = new Dictionary<string, string>
            {
                {"my-conn", "my-value"}
            };

            invoker.Invoke(Enumerable.Empty<Switch>(), new EnvironmentSettings(appSettings, connectionStrings));

            var instance = (CanUseAppSetting) invoker.CommandInstance;

            Assert.That(instance.AppSetting, Is.EqualTo("my-value"));
            Assert.That(instance.ConnectionString, Is.EqualTo("my-value"));
        }

        class CanUseAppSetting : ICommand
        {
            [Parameter("my-setting", allowAppSetting: true)]
            public string AppSetting { get; set; }

            [Parameter("my-conn", allowConnectionString: true)]
            public string ConnectionString { get; set; }

            public void Run()
            {

            }
        }
    }
}
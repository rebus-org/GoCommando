using System;
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

        [TestCase("-s:value2")]
        [TestCase("-s=value2")]
        [TestCase(@"-s""value2""")]
        public void CanCorrectlyHandleDifferentAlternativeSwitchFormatsFoundInOneSingleTokenOnly_Shortname(string switchText)
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
            [Parameter("switch", shortName: "s")]
            public string Switch { get; set; }

            public void Run()
            {
            }
        }

        [Test]
        public void CanUseSuppliedCommandFactory()
        {
            var commandFactory = new CustomFactory();

            var commandInvoker = new CommandInvoker("null", typeof(CreatedByFactory), new Settings(), commandFactory: commandFactory);

            commandInvoker.Invoke(Enumerable.Empty<Switch>(), new EnvironmentSettings());

            Assert.That(commandInvoker.CommandInstance, Is.TypeOf<CreatedByFactory>());

            var createdByFactory = (CreatedByFactory)commandInvoker.CommandInstance;
            Assert.That(createdByFactory.CtorInjectedValue, Is.EqualTo("ctor!!"));

            Assert.That(commandFactory.WasProperlyReleased, Is.True, "The created command instance was NOT properly released after use!");
        }

        class CustomFactory : ICommandFactory
        {
            CreatedByFactory _instance;

            public bool WasProperlyReleased { get; set; }

            public ICommand Create(Type type)
            {
                if (type == typeof(CreatedByFactory))
                {
                    _instance = new CreatedByFactory("ctor!!");
                    return _instance;
                }

                throw new ArgumentException($"Unknown command type: {type}");
            }

            public void Release(ICommand command)
            {
                if (_instance == command)
                {
                    WasProperlyReleased = true;
                }
            }
        }

        class CreatedByFactory : ICommand
        {
            public string CtorInjectedValue { get; }

            public CreatedByFactory(string ctorInjectedValue)
            {
                CtorInjectedValue = ctorInjectedValue;
            }

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

            var environmentVariables = new Dictionary<string, string>
            {
                {"my-env", "my-value"}
            };

            invoker.Invoke(Enumerable.Empty<Switch>(), new EnvironmentSettings(appSettings, connectionStrings, environmentVariables));

            var instance = (CanUseAppSetting)invoker.CommandInstance;

            Assert.That(instance.AppSetting, Is.EqualTo("my-value"));
            Assert.That(instance.ConnectionString, Is.EqualTo("my-value"));
            Assert.That(instance.EnvironmentVariable, Is.EqualTo("my-value"));
        }

        class CanUseAppSetting : ICommand
        {
            [Parameter("my-setting", allowAppSetting: true)]
            public string AppSetting { get; set; }

            [Parameter("my-conn", allowConnectionString: true)]
            public string ConnectionString { get; set; }

            [Parameter("my-env", allowEnvironmentVariable: true)]
            public string EnvironmentVariable { get; set; }

            public void Run()
            {

            }
        }
    }
}
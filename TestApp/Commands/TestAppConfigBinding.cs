using System;
using GoCommando;

namespace TestApp.Commands
{
    [Command("appconfig")]
    public class TestAppConfigBinding : ICommand
    {
        [Parameter("appSettingsKey", allowAppSetting: true)]
        [Description("Demonstrates how an appSetting can be automatically bound")]
        public string AppSetting { get; set; }

        [Parameter("connectionStringsName", allowConnectionString: true)]
        [Description("Demonstrates how a connectionString can be automatically bound")]
        public string ConnectionString { get; set; }

        [Parameter("windir", allowEnvironmentVariable: true)]
        [Description("Demonstrates how an environment variable can be automatically bound")]
        public string EnvironmentVariable { get; set; }

        public void Run()
        {
            Console.WriteLine($@"AppSetting: {AppSetting}

ConnectionString: {ConnectionString}

EnvironmentVariable: {EnvironmentVariable}");
        }
    }
}
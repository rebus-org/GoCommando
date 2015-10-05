using System;
using GoCommando;

namespace TestApp.Commands
{
    [Command("stop")]
    public class StopCommand : ICommand
    {
        [Parameter("bimse", defaultValue: "default_value")]
        public string Bimse { get; set; }

        public void Run()
        {
            Console.WriteLine("BIMSE: {0}", Bimse);
        }
    }
}
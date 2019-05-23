using System;
using System.Globalization;
using GoCommando;

namespace CoreTestApp.Commands
{
    [Command("new")]
    [Description("Generates a new guid")]
    public class GuidCommand : ICommand
    {
        public void Run()
        {
            Console.WriteLine(Guid.NewGuid());
        }
    }
}
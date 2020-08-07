using System;
using GoCommando;

namespace TestApp.Commands
{
    [Command("new")]
    [Description("Does something new")]
    public class NewCommand : ICommand
    {
        [Parameter("qid")]
        public string QueryId { get; set; }

        public void Run()
        {
            Console.WriteLine($"QueryId {QueryId}");   
        }
    }
}
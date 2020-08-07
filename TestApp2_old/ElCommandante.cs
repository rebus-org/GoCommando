using System;
using GoCommando;

namespace TestApp2
{
    [Command("el-commandante")]
    public class ElCommandante : ICommand
    {
        public ElCommandante(string text)
        {
            Text = text;
        }

        public string Text { get; }

        public void Run()
        {
            Console.WriteLine($"hoy!: {Text}");
        }
    }
}
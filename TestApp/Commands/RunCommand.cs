using System;
using GoCommando;

namespace TestApp.Commands
{
    [Command("run")]
    public class RunCommand : ICommand
    {
        [Parameter("path")]
        public string Path { get; set; }

        [Parameter("dir")]
        public string Dir { get; set; }

        [Parameter("flag")]
        public bool Flag { get; set; }

        [Parameter("moreflag")]
        public bool MoreFlag { get; set; }

        [Parameter("optionalFlag", optional: true)]
        public bool OptionalFlag { get; set; }

        public void Run()
        {
            Console.WriteLine($@"Running!

    Path:   {Path}
    Dir:    {Dir}
    Flag:   {Flag}
    MFlag:  {MoreFlag},
    OFlag:  {OptionalFlag}");
        }
    }
}
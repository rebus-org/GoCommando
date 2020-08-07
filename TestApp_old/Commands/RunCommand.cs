using System;
using GoCommando;

namespace TestApp.Commands
{
    [Command("run")]
    [Description("Runs the program")]
    public class RunCommand : ICommand
    {
        [Description("Specifies the path with which stuff is to be done")]
        [Parameter("path", shortName: "p")]
        [Example(@"c:\temp\somefile.json")]
        public string Path { get; set; }

        [Parameter("dir")]
        [Example(@"C:\temp")]
        [Example(@"C:\windows\microsoft.net")]
        [Example(@"C:\windows\system32")]
        public string Dir { get; set; }

        [Parameter("flag")]
        public bool Flag { get; set; }

        [Parameter("moreflag", shortName: "m")]
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
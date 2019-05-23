using System;
using GoCommando;

namespace CoreTestApp
{
    [Banner(
@"---------
This is actually a netcore application using GoCommando...
---------
")]
    class Program
    {
        static void Main(string[] args)
        {
            Go.Run();
        }
    }
}

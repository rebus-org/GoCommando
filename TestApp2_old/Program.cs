using System;
using GoCommando;

namespace TestApp2
{
    class Program
    {
        static void Main()
        {
            Go.Run<CustomFactory>();
        }
    }

    class CustomFactory : ICommandFactory
    {
        public ICommand Create(Type commandType)
        {
            if (commandType == typeof(ElCommandante))
            {
                return new ElCommandante("che!");
            }

            throw new ArgumentException($"Unknown command type: {commandType}");
        }

        public void Release(ICommand command)
        {
            Console.WriteLine($"imonna let you go: {command}");
        }
    }
}

using GoCommando;

namespace Beverage.Commands
{
    [Command("martini", group: "simple")]
    [Description("It's just Martini.")]
    public class Martini : ICommand
    {
        public void Run()
        {
            throw new System.NotImplementedException();
        }
    }
}
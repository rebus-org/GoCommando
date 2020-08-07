using System;
using GoCommando;

namespace Beverage.Commands
{
    [Command("black-russian", group: "duderino")]
    [Description("Mixes a White Russian, pouring in milk till full")]
    public class BlackRussian : ICommand
    {
        [Parameter("vodka")]
        [Description("How many cl of vodka?")]
        public double Vodka { get; set; }

        [Parameter("kahlua")]
        [Description("How many cl of Kahlua?")]
        public double Kahlua { get; set; }

        [Parameter("lukewarm", optional: true)]
        [Description("Avoid refrigerated ingredients?")]
        public bool LukeWarm { get; set; }

        public void Run()
        {
            Console.WriteLine($"Making a {(LukeWarm ? "luke-warm" : "")} beverage" +
                              $" with {Vodka:0.#} cl of vodka" +
                              $" and {Kahlua:0.#} cl of Kahlua");
        }
    }
}
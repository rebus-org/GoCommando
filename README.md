What?
====

GoCommando is a small command line utility helper that does the boring work when creating command line utilities in .NET.

More info coming soon at http://mookid.dk/gocommando

One day, maybe I'll tweet something as well... [@mookid8000][2]

How?
====

Create a new console application. `Install-Package GoCommando` to get the DLL, and then `Go.Run()` in the `Main` method.

And then you add some classes that implement `ICommand` and you decorate them with `[Command]` and
then you add properties to those classes, and then you decorate those with `[Parameter]`.

And then you decorate the command class and the parameter properties with `[Description]`.

And then you add a couple of `[Example]` to some of the parameter properties, just to be nice.

Example!
====

This is the `white-russian` command in a fictive Beverage utility:


    [Command("white-russian")]
    [Description("Mixes a White Russian, pouring in milk till full")]
    public class WhiteRussian : ICommand
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

If you invoke it without its command, it will print out the available commands:

    C:\> beverage.exe

    ------------------------------
    Beverage Utility

    Copyright (c) 2015 El Duderino
    ------------------------------
    Please invoke with a command - the following commands are available:

        white-russian - Mixes a White Russian, pouring in milk till full

    Invoke with -help <command> to get help for each command.

    Exit code: -1

and then, if you add `-help white-russian` as an argument, you'll get detailed help for that command:

    C:\> beverage.exe -help white-russian

    ------------------------------
    Beverage Utility

    Copyright (c) 2015 El Duderino
    ------------------------------
    Mixes a White Russian, pouring in milk till full

    Type

        Beverage.exe white-russian <args>

    where <args> can consist of the following parameters:

        -vodka
            How many cl of vodka?

        -kahlua
            How many cl of Kahlua?

        -lukewarm (flag/optional)
            Avoid refrigerated ingredients?

If you try to invoke a command, and one or more of the arguments are missing, you'll know:

    C:\> beverage white-russian -vodka 1

    ------------------------------
    Beverage Utility

    Copyright (c) 2015 El Duderino
    ------------------------------
    The following required parameters are missing:

        -kahlua - How many cl of Kahlua?

    Invoke with -help <command> to get help for each command.

    Exit code: -1

and then, when you finally invoke the command with the right arguments, it'll run as you would probably expect:

    C:\> beverage white-russian -vodka 2 -kahlua 2

    ------------------------------
    Beverage Utility

    Copyright (c) 2015 El Duderino
    ------------------------------
    Making a  beverage with 2 cl of vodka and 2 cl of Kahlua

Neat.

License
====

GoCommando is [Beer-ware][1].

[1]: http://en.wikipedia.org/wiki/Beerware
[2]: http://twitter.com/mookid8000

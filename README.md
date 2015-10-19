What?
====

GoCommando is a small command line utility helper that does the boring work when creating command line utilities in .NET.

More info coming soon at http://mookid.dk/oncode/gocommando

One day, maybe I'll tweet something as well... [@mookid8000][2]

How?
====

Create a new console application. `Install-Package GoCommando` to get the DLL, and then `Go.Run()` in the `Main` method.

And then you add some classes that implement `ICommand` and you decorate them with `[Command]` and
then you add properties to those classes, and then you decorate those with `[Parameter]`.

And then you decorate the command class and the parameter properties with `[Description]`.

And then you add a couple of `[Example]` to some of the parameter properties, just to be nice.

License
====

GoCommando is [Beer-ware][1].

[1]: http://en.wikipedia.org/wiki/Beerware
[2]: http://twitter.com/mookid8000
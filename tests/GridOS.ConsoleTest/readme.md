## GridOS.ConsoleTest

This is a very simple project that allows you to run GridOS as a console app, completely isolated from the Space Engineers game.

It achieves this via faking the interface of a few in-game components – most significantly, LCD displays. Basically it just outputs what would appear on the in-game display to the console window, and allows you to issue menu navigation commands via keypresses (up, down, enter).

It was created to test the menu functionality. You could do actual all-around testing of modules as well, but for that you'd need to create fakes of all the in-game components your modules use (e.g. gyros, lights, thrusters, and even the grid system generally).
# DCPM - WIP
This is a repository for a plugin manager that I've been working on for a game called DeadCore, it is currently a work in progress so it's not perfect but it works, I'm in the process of improving it and adding things to make it easier for plugins to be made.

The plugins and the manager are all written in C# (.NET 2.0 since Unity libraries are 2.0 and I was too lazy to figure out how to make 4.5 and 2.0 runtimes work nicely together).

Disclaimer: I'm not a professional by any means, there will be bugs and badly written things that make no sense.

Do **NOT** under **ANY CIRCUMSTANCES** place an untrustworthy dll file into the 'dcpm-plugins' folder, the reason for this is that all dll files in that folder are loaded in order to retrieve the plugins from them regardless of whether they contain a plugin or not.

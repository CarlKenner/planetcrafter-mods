# tpc

WIP, experimental.

Command line utility to support installing/removing plugins locally. Has bigger ideas but it's not there yet.

## A Friendly Warning and Avoidance of All Responsibility

While I will never knowingly push broken code, mistakes can and will happen. 
Please treat `tcp` as *very* experimental. 

It does work, and it is very simple. However, it does include file copy/delete in some of its operations,
so proceed with caution.

Before trying `tpc`, it is strongly advised that you zip up or make a copy of your
`The Planet Crafter\BepInEx\plugins` directory and stash it somewhere.

Absolutely no warranties or guarantees are offered or implied. Run this code at your own risk.

# Available Commands

Run `tpc -h` for a list of commands, or run `tpc <command> -h` for help with a particular command.

## Quick note on search terms

Some commands support search terms, explained here.

Search terms...
* support `*` and `?` wildcards, or regex (when `-e` is used). 
* are (unless otherwise noted) matched against a plugin's name, filename, and guid. 
  * eg, if plugin A's name matches a term, and plugin B's guid matches the same term, both plugins are included in the search results.
* are not case-sensitive.
  * ie, `*plugin*` and `*PLUGIN*` are equivalent.
  * also true for regular expressions. `^PLUGIN` and `^plugin` are equivalent.

### Substring vs exact matching

Like `dir` on the command line, search terms are matched exactly (ie, a==b)
unless a search term begins or ends with a wildcard.

This behavior changes when using a command's `-e` flag, which treats search
terms as regular expressions. Regular expressions naturally match any part
of the string unless the regex pattern includes start/end terminator checks.

Please be aware of this difference when using regular expressions, particularly with the `remove` command.

## ls

Lists currently installed plugins along with metadata and file information. Similar to `dir` and `ls` console commands.

Supports multiple output formats: table summary (default), detailed list, names-only, filenames-only.

### Examples

#### Show all plugins

`tpc ls`

#### Show all plugins as a detailed list

`tpc ls -od`

#### Show all plugins, filenames only

`tpc ls -of`

#### Show all plugins containing the word 'buffalo'

`tpc ls *buffalo*`


## add

Adds a plugin to The Planet Crafter by copying the plugin's BepInEx plugin assembly (eg, `Doublestop.RotatedCompass.dll`)
to the game's `BepInEx\plugins` subdirectory. 

Currently, this command supports two installation sources, with a couple more in progress.

1. Add a plugin's assembly file directly.

    `tpc add c:\projects\Doublestop.RotatedCompass\bin\Debug\net462\Doublestop.RotatedCompass.dll`

2. Add a plugin contained in a directory.

    `tpc add c:\downloads\Doublestop.RotatedCompass-0.0.1\`

3. (TBD) Add a plugin from a downloaded zip file.

    eg, `tpc add c:\downloads\Doublestop.RotatedCompass-0.0.1.zip`

4. (TBD) Add a plugin from a url.

    eg,  `tpc add https://github.com/doublestop/planetcrafter-mods/releases/download/RotatedCompass_v0.0.1/Doublestop.RotatedCompass-0.0.1.zip`


When installing from a directory (option #2 above), it is required that only one `.dll` file exists in that directory.
If more than one `.dll` file is found, the installer will refuse to continue and throw an error. 
This is because plugin assemblies are self-contained, and so there should only ever be one `.dll` file for a given plugin 
and only that one `.dll` file is copied to the root of the game's `plugins` directory. If multiple `.dll` files are found,
the installer cannot be sure which one to install.

To be absolutely safe, and to rule out the possibility of inadvertently overwriting another author's plugin,
the installer enforces this limitation when installing from a directory.

In situations where one directory will contain multiple plugin assemblies, eg a shared build output directory,
you may circumvent this limitation by pointing the installer directly at one of the `.dll` files, as in option #1.

`tpc add c:\projects\shared-build-output\Doublestop.RotatedCompass.dll`

Note: This limitation will eventually be removed, and `tpc` will eventually support installing multiple plugins simultaneously.

## remove

Removes one or more plugins using a search pattern. See the `ls` command for search behavior.

User confirmation is required before any files are removed. Confirmation can be skipped by 
including the `-f` switch after the `remove` command, eg `tpc remove -f someplugin`

# Installation

At this time, `tpc` does not have an installer or a release cycle. The project must be downloaded and built locally.

## Building from Source
* Unlike plugins, which are currently limited to targeting .NET 4.6.2, this project targets .NET 6.0.
* Make sure you have installed the [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).
* Grab the source, build.
  * Open `planetcrafter-mods.sln` in Visual Studio, build in the IDE; or,
  * Open a command prompt in the same directory as `planetcrafter-mods.sln`, and run `dotnet build`.
  * `tpc.exe` will be output to `src/tools/tpc/bin/Debug/net6.0/tpc.exe`

If you clone this repository, please disconnect from the remote. I will not be accepting pull requests at this time.

`git remote remove origin`

## Configuration
* Create a directory called `.tpc` in your home directory.
  * eg, `c:\users\shanevanboening\.tpc`
* Add a text file in that directory named `tpc.cfg`.
  * eg, `c:\users\aloysiusyapp\.tpc\tpc.cfg`
* Open a command prompt, and enter `edlin tpc.cfg`
* Only kidding. Use a text editor like a normal person.
* Open `tpc.cfg` in wordpad, then add the following line:
  * `game_dir = <path\to\Steam>\steamapps\common\The Planet Crafter`
  * Replace `<path\to\Steam>` with the path to your local Steam install, probably `C:\Program Files (x86)\Steam`.
  * If you skip this step, `tpc` will attempt to discover the game's directory
    by looking it up in your local Steam library.
* Create a batch file, or add `planetcrafter-mods/src/tools/tpc/bin/Debug/net6.0` to your `PATH`.
* Yeah... Anyway, here's a batch file.
```
    @echo off
    set assembly_dir="C:\Projects\planetcrafter-mods\src\tools\tpc\bin\Debug\net6.0"
    set assembly=tpc.exe
    %assembly_dir%\%assembly% %*
```

## Always Never Forget

* You're bigger and bolder
* And rougher and tougher
* In other words, sucker, there is no other
* You're the one and only dominator
* You're the one and only [dominator](https://www.youtube.com/watch?v=-dohzrXT09w)

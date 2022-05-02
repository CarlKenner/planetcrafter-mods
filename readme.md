# Doublestop's Planet Crafter Mods

A buncha mods for The Planet Crafter, possibly.

## The Mods

* [Rotated Compass](src/mods/RotatedCompass)
* [Compass Always Visible](src/mods/CompassAlwaysVisible)

## TBD

Not entirely sure. I'm just getting my feet wet with modding. I have several ideas for small QoL improvements I'd personally
like to see added to the game. I may try my hand modding them in myself. 

# Installation

Installation is manual, and instructions are currently the same for every mod. 

* Ensure the `BepInEx\\plugins` folder exists in the game's installation folder.
  * ex: C:\\Program Files (x86)\\Steam\\steamapps\\common\\The Planet Crafter\\BepInEx\\plugins
  * The `BepInEx` subfolder is part of the game installation, and will exist on your system. However, it may be necessary
to create the `plugins` subfolder yourself. 
* Select and download the mod you would like to install from the [releases](//github.com/doublestop/planetcrafter-mods/releases) page.
  * Click the `Assets` widget to display the files associated with the release.
  * Download the listed `.dll` file (ex: `Doublestop.RotatedCompass.dll`).
  * It's very likely your browser will warn you about downloading binaries such as `.dll` files. Ignore the warning and keep the file. More details on this below.
  * Copy the downloaded `.dll` to the `BepInEx\\plugins` folder.
    * ex: C:\\Program Files (x86)\\Steam\\steamapps\\common\\The Planet Crafter\\BepInEx\\plugins\\Doublestop.RotatedCompass.dll
* Launch the game.
* Enjoy the mod!

## Downloading binaries from this repository

I've made releases available in binary form for users who are not set up to build from source. 
Every release is tied to a git tag, which is in turn tied to a particular revision of the source. 
The binary you download is built directly from that revision.

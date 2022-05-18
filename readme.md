# CarlKenner's Planet Crafter Mods

Mods for The Planet Crafter and The Planet Crafter Prologue.
Tested on version 0.4.008.

## Available Mods

### [Fix Shallow Water](src/mods/FixShallowWater)
The original game contains a bug where you can't pick up items or mine resources that are under shallow water.
This mod fixes that bug. Objects within range under the water now have priority over the water itself...
unless you are in the "WARNING - Hydration level low" state, in which case water has the priority until you drink.
Water with nothing behind it can still always be targetted, of course.
In other words, everything just works now.

This may slightly reduce performance as it now has to do collision detection twice.

### [Fix Units](src/mods/FixUnits)
Fixes how units, such as kW, are displayed. Power is now measured in kW instead of kW/h or kW/s (which made no sense).
Oxygen no longer stops at ppm, and is now displayed as a percentage once it reaches 1,000 ppm. Earth is 21% oxygen, if you want a target to aim for.
All values are now displayed with thousand separators (commas), except in the build menu.

# Mod Installation

Installation is manual, and instructions are currently the same for every mod.

* Install `BepInEx`, a one-time requirement, before using any mods. Follow the instructions on The Planet Crafter Wiki's Modding page, in particular the `Using Mods` section at the top.
  * https://planet-crafter.fandom.com/wiki/Modding
* Select and download a plugin from the [releases](//github.com/carlkenner/planetcrafter-mods/releases) page.
  * Click the `Assets` widget to show the files attached to the release.
  * Download the attached `.zip` file (ex: `CarlKenner.FixUnits-0.0.1.zip`).
  * Open the downloaded zip and copy the contained `.dll` to the `BepInEx\\plugins` folder under the game's installation directory.
    * ex: C:\\Program Files (x86)\\Steam\\steamapps\\common\\The Planet Crafter\\BepInEx\\plugins\\CarlKenner.FixUnits.dll
* Launch the game.
* Enjoy the plugin!

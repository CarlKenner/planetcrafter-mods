# Fix Shallow Water

The original game contains a bug where you can't pick up items or mine resources that are under shallow water.
This mod fixes that bug. Objects within range under the water now have priority over the water itself...
unless you are in the "WARNING - Hydration level low" state, in which case water has the priority until you drink.
Water with nothing behind it can still always be targetted, of course.
In other words, everything just works now.

This may slightly reduce performance as it now has to do collision detection twice.

Tested on 0.4.008

# Installation

See the [readme](/readme.md) at the root of this repository for installation instructions.

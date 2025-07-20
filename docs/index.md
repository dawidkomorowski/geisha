# Geisha Engine
Geisha is a game engine written in C#. It is personal project with objective to learn about different aspects of technology behind video games and implement it by myself.

Name of the engine stands for **G**ame **E**ngine **I**n c**SHA**rp.

[View on GitHub](https://github.com/dawidkomorowski/geisha)

> [!NOTE]
> This documentation provides information on Geisha SDK 0.10.0.

## Features
Geisha Engine currently supports only Windows platform.

Following list presents high level overview of engine features grouped by module:
- Animation
    - 2D sprite animation - animation based on sequence of sprites
- Audio
    - Basic sound playback - play sounds, pause or stop sound playback
- Core
    - Entity-Component architecture - scene consist of entities of which behavior and functionality is defined by attached components
    - Define your game logic in components or systems depending on your needs
    - Scene serialization
    - Support for coroutine programming model - easily implement functions executed over multiple frames
- Input
    - Keyboard - read keyboard state
    - Mouse - read mouse state
    - Input binding and mapping - bind input devices to actions and axes
- Physics
    - 2D collision shapes - use rectangle or circle colliders to define static and kinematic bodies
    - Tile based collision geometry - define static level geometry as rectangular tiles in a grid
    - 2D collision detection - use kinematic bodies to get information about overlapping entities
    - 2D collision resolution - use kinematic bodies to simulate collision impact and avoid overlap
- Rendering
    - 2D primitives rendering - render rectangles and ellipses
    - 2D sprite rendering - render sprites
    - 2D text rendering - render text with support for basic layout (wrapping, clipping, alignment)
    - 2D camera support - control what part of scene is visible on the screen

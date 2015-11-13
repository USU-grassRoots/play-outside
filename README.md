# Outside Simulator
A simple game using DirectX 11 and C#. Features a computer nerd that, due to a utilities outage, has to go outside.

## Objectives
The Outside Simulator project seeks to create a simple game that can be used to illustrate various game programming concepts. It is implemented in C# and DirectX 11 for ease of coding and familiarity with the environment, as opposed to the traditional C++ and custom everything platform. As such, there is an immediate performance hit, and the game will not have advanced memory management features (as the .NET framework doesn't react kindly to performing custom memory management) or custom thread management systems. The objective of this project is to demonstrate an *implementation* of *game programming concepts*, and *NOT* to demonstrate optimization techniques. That being said, optimizations are still implemented in the game logic itself - things such as frustum culling, Level-Of-Detail (LOD), (where possible) contiguous memory allignment, and asynchronous tasking.

## Concepts Demonstrated
(PENDING) These are the main concepts of game programming that are demonstrated in this application:
* Engine vs. Game Separation
  * Game and Engine are separated, allowing for future games to be built on similar code
* 3D Graphics
  * Shaders (vertex, pixel, geometry, compute)
  * Rendering pipeline
  * Effects
  * Lighting / Shadows
* Resource Management
  * Textures
  * Sounds
  * Game level loading
* 3D Audio
* Game Logic
  * World State
  * Player State
  * Agent interaction (NPCs)
* Physics
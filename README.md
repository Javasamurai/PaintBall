# FPS Game with netcode entities

A standard first person multiplayer shooter with heavily optimized netcode utilizing netcode for entities

##  Controls : 
PC: WASD, Mouse for camera control, left click to shoot
Mobile: Touch Controls

### Technologies used:
Unity, C#, DOTS, Burst compiler

### GameArchitechture

```
Bootstrap.cs and Bootstram_scene.unity controls the game flow.
Game.cs help with the Dependency requirement and game initilization
NetworkServices.cs connects with the netcode
```
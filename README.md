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
Used ECS architecture for rest of the gameplay netcode. Very decoupled and independent.

<img width="1679" height="851" alt="Screenshot 2025-10-23 at 10 59 13 AM" src="https://github.com/user-attachments/assets/f51b4632-7b88-422d-b739-d1e21aa9e24c" />
<img width="865" height="430" alt="Screenshot 2025-10-23 at 10 59 57 AM" src="https://github.com/user-attachments/assets/3861e0e1-58a1-4908-8293-71265995388f" />

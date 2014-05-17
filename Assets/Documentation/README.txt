Project 3
IMGD4000
Joshua Morse jbmorse@wpi.edu
Tim Calvert tncalvert@wpi.edu
3/28/2014


MazeGenerator.cs :
The maze generator file. This file generates the maze off of a given width, height, and seed. 
If not given width or height, it will use default values. If a seed isn't given, it will use
a random seed. Our maze generator currently produces a "perfect" maze in that it has no loops,
and it fills up the entire m x n grid. There is one path from the start to any one point.
The maze is made up of cells, which has a position, the four walls, a hascoin boolean, a
hasmushroom int, and a hasghoulie boolean. The maze generator procedurally steps through the 
(unbuilt) maze and creates corridors as it walks, keeping track of where it needs to go to 
complete the maze. After the maze is generated, it produces coins mushrooms (good and bad), 
and ghoulies randomly in the maze.


PlaceMaze.cs :
This is a maze placer that takes the maze produced from the MazeGenerator.cs and generates
what you see in unity. We have a debug mode that allows us to see the maze from the top
without the textured cells, and only using the default walls and floors.
It instantiates the correct cell prefab by stepping through a series of 4 if statements 
testing each wall value. Once it locates the proper cell to place, it will do that, then
place any special items that can be available on that certain cell, like torches or statues.
The torches and statues have a percent chance of spawning, and if they do spawn, they spawn in
a semi-random spot on the cell. After these things are placed, mushrooms, coins, and ghoulies will be
distributed throughout the maze based on the given maze object.


CameraOperations.cs :
This is the god view camera controller that toggles it on and off for debugging purposes. 
It can be controlled with "ijkl" to look around, "," and "." to zoom, and shift to move faster.
Press "o" to enter god view.


GoldHandler.cs :
Handles picking up gold coins and making them inactive upon pickup.
Also keeps track of how many coins have been picked up.


GummyThrower.cs :
This is the gummy script file that is attached to the player that determines how the gummies are
thrown. It listens for input and whenever "r", "g", or "b" is pressed, it will throw the appropriate
gummy. If shift is held when the button is pressed, it will be dropped instead.


Highlighter.cs :
This is the file that highlights objects that the player is looking at. It does this with a raycast.


Illuminate.cs :
This is used in conjunction with highlighter to actually highlight the objects. 


Pickup.cs :
This file is (obviously) the handler for pickup of objects. It handles pickup based on distance 
from the raycast sphere, so you don't have to look directly at a gummy to pick it up. 


MushroomTypeHandler.cs:
This handles the pickup interaction between the player and the mushroom once it is picked up.
It allows the event of "good" or "bad" mushrooms to be registered. 


MushroomHandler.cs:
This is the file that handles the "good" or "bad" notification from the above file.
It displays what the type of mushroom that was picked up is, and reports the time remaining of
the effect on the screen.


GhoulieHandler.cs:
This is the global handler for keeping track of all the ghoulies present on the map,
and whenever a mushroom is picked up, notifying them all of such, so they can
take the appropriate action. GhoulieMovement.cs populates the ghoulies,


GhoulieMovement.cs:
This is the file that handles how the ghoulies move at each frame, and holds the path
for each. When the path count reaches 0, it generates a new random path. When the player
picks up a mushroom, it generates a path to follow the player or to run away from the 
player.


StatusCreator.cs:
This file generates the status messages for the player whenever they press a number key.
Used for testing purposes.


GuiHandler.cs:
This file shows all the GUI aspects to our game, IE the hud. It shows the points, it shows the
mushroom updates, it shows the test messages, and it shows the crosshair.


GummyCollision.cs:
This file is what destroys the gummys (when thrown) when they collide with another object.


GummyColor.cs:
Simple file that holds the gummy color. Useful for collisions with ghouls.


GhoulieColorMatching.cs:
This handles the color collisions with the ghoulies. If 3 daggers of the correct color collide
with the ghoulie, it will be destroyed. Whenever a ghoul is hit with the correct color, it auto
generates a new color, and sets the render material to reflect that.


DaggerRoation.cs
Applies a torque to daggers so they spin after being thrown.


BossActions.cs
The handles AI for the boss, deciding what it should do and performing those actions. It also
manages health and other attributes for the boss.


Orb.cs
The handles interactions with the orbs placed around the boss room, which are used to damage
the boss.


Cheats.cs
Manages a couple of cheat functions for testing.


BossTrigger.cs
Handles triggering on player entering the "end cell" so that we can move it to the boss room.


MenuScript.cs
Does the GUI for the main menu.


LevelChanger.cs
Provides a way to switch between levels easily. It also executes arbitrary functions after loading
the level.


Boss_Waypoint_Generator.cs
Loads waypoits for the boss room.


PickupNetwork.cs
Handles pickup stuff over the network.


GoldHandlerNetwork.cs
Handles gold management for network clients.


IGoldHandler.cs
An interface for the two gold handlers.


AxeImpactReceiver.cs
Deals with the boss hitting the player with his axe.


NetworkManager.cs
Handles creating game instances and connecting to games over the network.


NetworkRigidbody.cs
Jia's script for managing rigidbodies over the network.


LevelManager.cs
Helps with loading the boss room over the network.


PlayerAnimations.cs
Handles player animations.


StateMachine\
Contains an attempt at implementing the hierarchical state machine described in the code provided
to use. It does not work.

# MyGame
A game I've been working on...

Gameplay demo video here:

The game is still in very early development :), at the moment it is a simple 2d platformer with some basic enemy Ai and Pathfinding, full player control with jumping, grabing onto walls and attacking, some environment interaction and a Skill Tree (which is also in early development). I am the single person working on this game and the code has been written by me (except for some libraries that I used, the code for those libraries is also present in the folder and some scripts for optimization, light flickering and parallax and probably others which I expanded upon or changed and I don't remember) and also I have to mention that I only designed and animated a few of the main characters and enemies, the rest were bought from itch.io.

Platform : Windows

Files:
  - UndeadTomb.exe is a playable build of the game : you can simply run it  and the press the continue button to play the game. Press ESC if you want to quit.
  - Assets files : I included only the code because this is relevant, the rest are just unity standard files with shaders and materials etc.

Player Controls: 
  - Movement: up,down,left,right arrow keys ( or Keys W S A D ) for moving, space is for jumping (also double jump) ( you can see the script in Assets/Scripts/Player/CharacterController2D.cs)
  - Attacking : Key Q (Assets/Scripts/Player/PlayerCombat.cs)
  - Interacting : (with portals or chests) Key E (Assets/Scripts/Interactibles in this folder is mainly everything and it works in conjuction with the PlayerCombat.cs)

EnemyAi:
  - I used a behaviour tree for the "so-called" Ai :) (I don't know if this is really considered an Ai but that's how I called it), mainly working around pandBT http://www.pandabehaviour.com/?page_id=23, EnemyAi.cs, SkeletonGfx.cs and SkeletonGfxCombat.cs are the old versions. The newer versions and the ones that I am using are in Assets/Scripts/EnemyAI_CS_BT
 
 Audio:
  - All present in Assets/Scripts/AudioManager.
  
 SkillTree:
  - Assets/Scripts/UiMenu/Ui_SkillTree.cs and Assets/Scripts/Player/PlayerSkills.cs
 
 The code is free to use :), it can be used as a starting point if you are looking to make a 2D platformer,metroidvania or side-scroller. 
 I have to mention that the characters and the enviorments are not free to use because, as I said, some were bought online.
 


# Neon Sprint

Neon Sprint is a neon-styled arcade racing game developed in Unity as my Final Year Project. The game focuses on fast, simple, session-based racing with AI opponents, checkpoint-based lap tracking, drifting, and a full race flow from menu to results screen.

## Project Overview

The aim of this project is to create a playable arcade racing game with a clear gameplay loop and a strong retro-inspired visual style. The player selects a car, races against AI drivers, completes laps around the track, and finishes based on race position. The project was built as a playable prototype with the core systems needed for a complete race experience.

## Features

- Arcade-style player car controller
- Forward drive, reverse, braking, and steering
- Handbrake drifting
- AI opponents that follow waypoint paths around the track
- AI recovery system if cars get stuck or crash out
- Countdown system at race start
- Checkpoint and lap tracking
- Race position tracking
- In-race HUD
- Finish/results screen
- Main menu with Play and Quit
- Swappable car models

## Built With

- Unity
- C#

## Current Game Flow

1. Launch into the main menu
2. Press Play to start the race
3. Countdown begins
4. Race against AI opponents
5. Complete checkpoints and laps
6. Finish the race
7. View results screen
8. Restart the race or exit

## Controls

- **W** - Accelerate
- **S** - Brake / Reverse
- **A / D** - Steer
- **Space** - Handbrake / Drift
- **C** - Change car model
- **R** - Restart race on results screen
- **Esc** - Exit or return from results flow depending on scene setup

## How to Run the Project

1. Open the project in Unity
2. Make sure the correct scenes are added to the Scene List / Build Profiles
3. Set the main menu scene as the first scene
4. Press Play in the Unity Editor

## Build Instructions

1. Open **File > Build Profiles**
2. Make sure the scenes are in the correct order:
   - MainMenu
   - Race scene
3. Select the Windows build profile
4. Click **Build**
5. Choose an output folder
6. Run the generated `.exe`

## Project Structure

A simplified overview of the main systems:

- **Main Menu** - Starts or exits the game
- **Car Controller** - Handles player and AI vehicle movement
- **AI Input** - Controls AI behaviour using waypoint navigation
- **Checkpoint System** - Tracks checkpoint progression and lap completion
- **Race Position Manager** - Calculates player race position
- **HUD** - Displays speed, lap, and race position
- **Results UI** - Displays finishing position and restart options

## Development Focus

This project focused on building the full playable race loop first, including:
- player driving
- AI racing
- race management
- UI
- scene flow

Audio and some extra polish features were not prioritised in order to get the main gameplay systems completed and working reliably.

## Known Limitations

- AI is functional but still basic
- Audio is not currently implemented
- The project is focused on one main race experience rather than a large content set
- Some systems could be polished further with more time

## Future Improvements

- Better AI behaviour and overtaking
- More tracks
- More car variety and balancing
- Audio and music
- Improved VFX and UI polish
- Car selection screen
- Options/settings menu

## Author

Alan Fitzgerald

Final Year Project  
Creative Computing  
SETU Waterford

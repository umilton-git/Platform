# Platformer Prototype

Prototype for a platform game with movement based on Smash Bros. Series.


## Features
- Jump that can be adjusted (Initial velocity, character weight, gravity, etc.)
- Separation between platforms and floors (Can jump through platforms but not floors)
- Running with an initial dash (double tap)
- Air movement values separated from ground movement

## To Be Implemented
- Ability to fall through platforms
- More robust air movement system (Momentum?)
- Character model to practice animations and animation blending with
- Smash character movement is heavily rooted in analog stick controls; Should implement those as an alternative

## Might Be Implemented
- Turning/Pivoting System (How useful is this in an actual platformer?)
- Jump Squat (Doesn't seem too useful/intriguing outside of a platform fighter)
- Friction (Used in some platformers but not all)

## Current Implementation Thoughts
- More to it than I thought
- A State Machine would make handling a lot of these + animating easier
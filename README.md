# Balloon

Multiplayer game based on preventing a balloon from touching the floor. AR Cloud Anchors is used for its development.
Project created for ITCL programming challenge 2021.

## Test

This project has been tested on Android phone Realme 7 and Realme 7 pro.

## Instructions

On first scene we can see the rooms that are available and create a new room. There is an input field to introduce the name of room that we want to create, and the button `Create Room` to create it. When a room is created the owner join it automatically, other user can join it through the button that appears below the text *Rooms*.

Into the room there is an area of text on top of the screen that guide the user during the game. To start the game is necessary that two players are on the same room and synchronize their balloons.

### AR Cloud Anchors Synchronization

Is the synchronization by default. AR Cloud Anchors is a google service that allows to place objects on augmented reality on an especific location, and another users can see this object with their own mobiles. This service store the location of the object but not the object so is needed that users have the same app to reproduce the object correctly.

This service have two phases, host and resolve. Host to store the location of object, and Resolve that is the response of this google service indicating the location that previously has been hosted.

Balloon game follows the two phases, but 3 buttons have been added in case any of these phases fail. `Clear` to restart host, `Manual Host` and `Manual Resolve` to place the balloon manually. There is an option `Manual Balloon Positioning` that allows to locate the balloon manually from the beginning.

The better balloons are synchronized the better experience. 

### Game Flow

When balloon is synchonized, it is raised and there is five seconds wait, then the game starts. There are turns per player, each player's turn is indicated in the message area at the top of the screen. The players have to touch the balloon to prevent it from falling to the ground. When the balloon hits the ground the game ends.

### Options

There is an `Options` button at the bottom of the screen. Pressing on it shows and options panel, another press hides the options panel.

- `Manual Balloon Positioning` button: allows to locate the balloon manually. By default is used AR Cloud Anchors Synchronization.
- `Hosting with sufficient FeatureMapQuality` button: applies a restriction.'Host' phase only begins if a good or sufficient FeatureMapQuality is captured. By default is off.
- `Enable vertical and forward force to tap` button: tap on balloon applies forward force in addition to the vertical. By default is vertical force.
- `The game is over when balloon touch any plane` button: when balloon touch any plane game is over. By default the game is over when touch the ground.
- `Restar Game` button: restarts the game (only when balloon is synchronized).
- `Exit Game` button: exits game.
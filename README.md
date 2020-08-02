# Gods Unchained Deck Tracker

Windows Application for deck tracking in Gods Unchained TCG


# Setup

**1. Download the .zip folder**

The published version (ready to start - just unzip): [Published](https://github.com/coin-coop/godsunchained-deck-tracker/releases/download/0.0.1.1/Gods.Unchained.Deck.Tracker.v.0.0.1.zip)

**OR**

The source code (you need to build it first by yourself for e.g. in Visual Studio 2019): [Source code](https://github.com/coin-coop/godsunchained-deck-tracker/archive/0.0.1.zip)

**2. Launch the application by GodsUnchained-Deck-Tracker.exe**

**3. Change the path to your log directory**

![Log directory path setting](/Docs/sc1.png)

It will look similiar to this one:

```
C:\Users\YourUser\AppData\LocalLow\FuelGames\Gods Unchained - Version 0.24.1.544(2020.7.23) - Built at 20_58_23\output_log.txt
```

For now app will ask for this path after each start as game with every new version has a new log path. Later I would like to display this popup only when path could be potentially changed.
 
**4. Wait until application will display all decks which you were using**

![Application view](/Docs/sc2.png)
 
In fact it will display decks played since 29.06. I have constrained to this date as loading all decks which player ever played could take quiet long. Later I'm planning add more infos for easier recognize which deck is displayed (e.g. distribution of core/genesis/welcome set cards). For now you need to recognize by cards displayed.

**Note: First display of the decks can take up to 10 minutes!!! So please be patient on the first run.**
 
**5. Click on the chosen Deck**
 
It will load the application with your deck. The deck wil be displayed in the app and in the separate half transparent window used for tracking drawn cards. At the picture you can see that in last game I have already drawn Tracking Bolt and Battle Aurochs.
 
**6. Play the game**
 
The deck tracker will automatically refresh whenever you start a new game.

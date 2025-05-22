# laberinto
Hello, This mod follows a basic algorithm for generating mazes in Vintage Story. I wanted to leave version 1 here so that you can use it for events or constructions in your corners. With the /maze command, it generates a maze where you are positioned looking, and in the configuration you will see a place that says: "defaultSize": "large", //options->> small medium large mazeWidth = Config.defaultSize switch"small" "=>",25,"medium" "=>", 50,"large" "=>",150,"_" "=>",150
those are the sizes and space dimensions that you will have
I hope you like it and do good tasks, events, or any idea that comes to mind. Currently, I do not intend to continue with the code for a long time, but later maybe I will, so I will try as much as possible to leave the source code on github in case anyone wants to add more things and such is welcome :)
-The only current command in this version:
-/maze //creates a maze as you have it configured, normally it is in "large" format
----------------------------------
---------------------------------Config-------
{
//remember to have the "game:admin" privilege to be able to use the "gamemode" command
"defaultSize": "large", //options->> small medium large mazeWidth = Config.defaultSize switch "small" "=>",25,"medium" "=>",50,"large" "=>",150,"_" "=>",150
"pedestalBlock": "game:mudbrick-light", //the starting block
"mazeHeight": 11, //the height of the maze
"trapChance": 0.25, //chance of there being traps in the maze
"rewardChance": 0.15, //chance of there being rewards in the maze
"mazeDurationSeconds": 1600, //the duration of the maze in seconds
"allowCommandActivation": true, //whether the maze can be activated with a command
"announceCoordinates": true, //whether the maze coordinates are announced
"maxPlayers": 20, // maximum number of players in the maze
"mazeDifficulty": "hard", // maze difficulty
"enableRanking": true, //whether ranking is enabled
"eventIntervalSeconds": 60, // time interval between events
"wallBlockTypes": [ // wall block types
"game:claybricks-uneven-four-soldier-fire",
"game:diamond-stone-clean"
],
"floorBlock": "game:mudbrick-light", // floor block
"rewardBlock": "game:chest-east", // reward block
"trapBlock": "game:trap-spike", // trap block
"startMessage": "The dynamic maze has started!", // start message
"successMessage": "Congratulations, you've completed the maze!", // success message
"failMessage": "You have failed the maze.", // failure message
"possibleRewards": [ // possible rewards
"game:metalbit-steel",
"game:charcoal"
],
"autoRegenerate": true, // whether to automatically regenerate the maze
"regenerateIntervalMinutes": 30, // time interval between regenerations
"allowBlockBreaking": false, // whether to allow block breaking
"allowBlockPlacement": false, // whether to allow block placement
"enableDebugLogging": true, // whether to enable debug logging
"penaltyFreezeSeconds": 15 // freezing penalty in seconds
}

Each time you use /maze
it will generate a different maze.
--Important note: the ranking and other features in this version are not yet supported by commands, but they are ready and tested. it is the /labyrinth

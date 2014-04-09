# ColorPuzzle - an F# teaching example

The object of this lesson is to create a little game, which actually will be able to run on Android. 
The game logic will be isolated and therefore easy to port to other platforms.

To do the exercises you do need some knowledge of F#. A working knowledge of at least the following will
be helpful:

 - records
 - discriminated unions
 - functions and recursive functions
 - lists
 - arrays
 - sets
 - list comprehensions

That being said, there is nothing stopping you from obtaining the knowledge while doing the exercises ;)

The game, when finished, will be a simple board game with x*y tiles. Each tile can have any of 7 (here) colors. 
There is a list of buttons with the corresponding 7 colors. The start position is in the upper-left corner and the game
is brought forward by choosing a color of any of the neighboring tiles. The starting tile and the corresponding 
neighbor tiles are then colored in the new color chosen. When all tiles are in the same color, the game is finished.

## Prerequisites
 - Xamarin Studio
 - F# bindings and stuff for Xamarin
 - This project

The project, as it comes for this lesson, is a skeleton project without much code. It will compile, but will not
run (without crashing). As you're working through the exercises, you'll (hopefully) get more and more working.

Please note that this lesson in no way should be taken as THE way to develop for Android! 

# Exercise 1
First, try running the application and watch it crash. It will stop with an IndexOutOfRangeException.

The culprit is in GameBoard.fs - the createGameBoard function, which just returns an empty array for the part
representing the board - the `tiles` member of the record.

Your task is to mend the function so it returns a `Tile array array` representing the board - it should be 
an array with `sizeY`elements, each having `sizeX` Tile elements, where the color should be random.

Other obvious members - especially `sizeX`, `sizeY` and `numberOfColors` should also be initialized. For now you 
should ignore the `currentTiles` member. It's usage will become clear later.

*Hint*: Use list comprehensions to create the tiles.

Run the application and verify that it works and draws a board with (at least seemingly) random colors.

# Exercise 2

Now a little explanation for the `currentTiles` member of the `gameBoard` record. The game starts in the 
upper left corner and `currentTiles` should contain the set of `Tile`s with have the same color as is
connected to the upper left corner, either directly or indirectly through other `Tile`s with the same color.

This also means that the starting value for `currentTiles` should be a set, which contains the `Tile`
in the upper left corner and all other `Tile`s which happen to be connected to it.

Your - not entirely simple - task for this exercise is to correctly initialize the `currentTiles` member in 
the `createGameBoard` function.

*Hint*: Move the responsibility to a separate function `calculateCurrentTileSet`:

    `calculateCurrentTileSet: (sizeX: int) -> (sizeY: int) -> (tiles : Tile array array) -> Set<Tile>`

You may find it useful to have a local recursive function inside this function. It is also, of course, entirely
possible to implement it in a fully imperative way.

`Set.union` may also come in handy.

*More hints*
The `currentTiles` is a set of connected `Tile`s. This means that for each `Tile` - starting with the upper
left corner - you look at the neigbors (left, right, up, down) and their color. If the color is the same
as on the `Tile` you're currently looking at they should be included in the set.

You may find it helpful to create a function

    `getNeighborTiles: (sizeX: int) -> (sizeY: int) -> (tile: Tile) -> (tiles: Tile array array) -> Tile list`

which given a gameBoard and a tile will return a list of it's neighboring tiles.

*Note*: The reason the two suggested functions do not simply take a `GameBoard` as parameter is that
the functions are used before a `GameBoard` has been created.

*Note*: The reason that the initial Set<Tile> is not simply the Tile from the upper-left corner is that
there may already be neighboring Tile's in the same color (due to the randomness).

GO!

# Exercise 4

We will now implement the function `paintBoardWithColor`, which will be called each time one of the color buttons
is clicked.

Lets assume that the board looks like this (digits corresponding to colors):

    1 1 1 2 3
    1 2 4 3 5
    5 6 1 3 7
    5 3 7 5 6
    1 3 2 5 6

When painting the board with the color `2` the board will look like this:

    2 2 2 2 3
    2 2 4 3 5
    5 6 1 3 7
    5 3 7 5 6
    1 3 2 5 6

All the connected `1`s in the upper left corner are changed to `2`s.

If we, on the other hand had painted with the color `4` the board would look like this:

    4 4 4 2 3
    4 2 4 3 5
    5 6 1 3 7
    5 3 7 5 6
    1 3 2 5 6

All the connected `1`s in the upper left corner are changed to `4`s.

For this exercise we will ignore whether it is actually possible to change to a given color, we will ignore
checking whether the game is finished, we will also ignore scoring.

*Hint*: Use the calculateCurrentTileSet from the last exercise.

Run the game to verify that tiles change color as expected.

# Exercise 5

In this exercise we will add code for counting the number of moves and a check for whether the game is
finished. This entails a few minor changes to `paintBoardWithColor`.

The number of moves should only be incremented if (at least) all of the following is true:

 - The `newColor` is different from the `currentColor` of the board
 - The game is not already finished
 - Painting with the `newColor` is actually changing the number of `currentTiles`

Try to figure out the rules for whether the game is finished and implement that, as well.

# Exercise 6




// In a later exercise...:
*Note:* The calculation could be prone to excessive recursion and unnecessary recalculation of neighbors, so
to ensure that your solutions performs well, you could change the following line in *MainActivity.fs*:

    `this.gameBoard <- createGameBoard 5 5 7`

to e.g.

    `this.gameBoard <- createGameBoard 25 25 7
   
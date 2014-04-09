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

Now we will implement some scoring, so there is some excitement in the game ;)

We will have two ways of obtaining some points:

 - Whenever painting causes us to reach a corner
 - Whenever painting causes the entire surface to be same color (the game is finished)

You can choose yourself what kind of point system you have. The exercise solution has an example of one way
to do it. But ofcourse it would be obvious to have a system which gives more points the sooner (the fewer
number of moves) you reach the goal.

# Exercise 7

Currently the *Next* button doesn't really do much. What we want it to do is to move the game to the next
level. This, of course, should only happen when the game is actually finished.

Clicking the button causes the invocation of the function `moveBoardToNextLevel`, which in it's current state
simply returns the given `gameBoard`. Instead it should return a `gameBoard` which is one bigger on each side, 
and the score should be retained. Obviously `numberOfMoves` should be reset to 0 (zero).

Run the game through an entire level and verify that the button now does what it should.

# Exercise 8

The bulk of the game is now finished. We will now put in some polish here and there. The first place is to
enable the loading and saving of a game, so Android can stop and start the app and we can restore the
state.

This is all initiated in `MainActivity.fs`, in the methods `OnSaveBoard` and `LoadBoard`. They use the
 `Persistence` module in `GameBoard.fs` - especially the functions `loadGameBoard` and `saveGameBoard`.

Your task is now to implement these two functions, so that the state of an entire `GameBoard` can be
saved and loaded.

*Note*: The current implementation in MainActivity.fs does not save the state when the application exits, which
is typically what happens when you press the *Back* key. To test the save of the state you should use the *Home*
key and then start the app again. Then verify that the colors, score, moves, etc. are the same.

# Exercise 9

You may have noticed that the board is not placed in the middle. In this exercise you should correct that and you 
should also make it so that the *Next* button is only visible when it should be, that is, when the game is
in a finished state.

One of these tasks take place in `MainActivity.fs` and the other in `GameView.fs`.

*Hint*: The property to use on the button is called Visibility and should be set to one of:

- ViewStates.Visible
- ViewStates.Invisible

# Exercise 10

We will in this exercise verify how your app performs. Especially the function `calculateCurrentTileSet` can - 
depending on the implementation - be prone to excessive recursion and unnecessary recalculation of neighbors, so
to ensure that your solution performs well, you should change the following line in `MainActivity.fs`:

    `this.gameBoard <- createGameBoard 5 5 7`

to e.g.

    `this.gameBoard <- createGameBoard 25 25 7
   
Then start the game and play a full level. You will soon notice whether your implementation performs
adequately. If not, the task is to optimize it!

*Hint*: If your implementation of `calculateCurrentTileSet` is fully immutable you might want to look into a partly
mutable implementation.

# Exercise 11

The last exercise is to implement the saving of preferences when the Back key is pressed and the app is otherwise
exited. 
[You can see here how to save preferences when the back key is pressed.](http://stackoverflow.com/questions/12171320/save-the-state-when-back-buton-is-pressed)
and take a look [here for how to propery save the preferences](https://forums.xamarin.com/discussion/8199/how-to-save-user-settings).

*Note*: You cannot directly save an array of ints in the shared preferences, so you have to find another way.

*Hint*: You should override the methods:

 - OnBackPressed()
 - OnDestroy()

And it does take some reorganizing of the code in `MainActivity.fs`.



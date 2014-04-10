namespace ColorPuzzle
module GameBoard = 

    type TileColor = TileColor of int
    with member x.value() = 
            match x with
            | TileColor(n) -> n

    type Position = {
        x : int
        y : int
    }

    type Tile = {
        color    : TileColor
        position : Position
    }

    let randomGenerator = System.Random()

    let getRandom n =
        randomGenerator.Next n

    let createRandomColor numberOfColors =
        let colorIndex = getRandom numberOfColors
        TileColor colorIndex
            
    type GameBoard = {
        sizeX         : int
        sizeY         : int  
        numberOfColors: int
        currentColor  : TileColor
        currentTiles  : Set<Tile>
        tiles         : Tile array array
        numberOfMoves : int
        score         : int  
        isFinished    : bool
    }

    let createGameBoard sizeX sizeY numberOfColors =
        { 
            currentColor = TileColor(0)
            sizeX        = sizeX
            sizeY        = sizeY 
            numberOfColors = numberOfColors
            tiles        = Array.empty
            currentTiles = set []
            numberOfMoves = 0
            score         = 0
            isFinished    = false
        }
            
    let moveBoardToNextLevel gameBoard = 
        gameBoard

    let paintBoardWithColor gameBoard newColor =
        gameBoard                   

    module Persistence =

        let tileToList tile =
            [tile.color.value()]
            @[tile.position.x]
            @[tile.position.y]

        let tileRowToList tileRow : int list =
            tileRow 
            |> List.collect(tileToList)

        let tilesToList tiles : int list =
            tiles 
            |> Array.toList
            |> List.collect(fun row -> row |> Array.toList |> tileRowToList)


        let saveGameBoard gameBoard =
            Array.empty<int>

        let loadTile (saved: int[]) position =
            { 
                color = TileColor(saved.[position + 0])
                position = { x = saved.[position + 1]; y = saved.[position + 2]}
            }

        let loadGameBoard (saved: int[]) =
            {
                sizeX = 0
                sizeY = 0
                numberOfColors = 0
                currentColor = TileColor 0
                numberOfMoves = 0
                score = 0
                isFinished = false
                tiles = Array.empty
                currentTiles = Set []
            }


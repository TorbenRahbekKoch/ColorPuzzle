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

    let calculateCurrentTileSet sizeX sizeY (tiles : Tile array array) : Set<Tile> = 
        let westTile tile =
            match tile.position.x, tile.position.y with
            | (x, y) when x > 0 -> Some(tiles.[y].[x-1])
            | _ -> None

        let eastTile tile =
            match tile.position.x, tile.position.y with
            | (x, y) when x < sizeX-1 -> Some(tiles.[y].[x+1])
            | _ -> None
        
        let northTile tile =
            match tile.position.x, tile.position.y with
            | (x, y) when y > 0 -> Some(tiles.[y-1].[x])
            | _ -> None
            
        let southTile tile =
            match tile.position.x, tile.position.y with
            | (x, y) when y < sizeY-1 -> Some(tiles.[y+1].[x])
            | _ -> None

        let rec goWest color tile tiles =
            let w = westTile tile
            match w with
            | Some(west) when west.color = color -> goWest color west (west::tiles)
            | _ -> tiles

        let rec goEast color tile tiles =
            let e = eastTile tile
            match e with
            | Some(east) when east.color = color -> goEast color east (east::tiles)
            | _ -> tiles

        let q = Queue<Tile>()
        let tileSet = List<Tile>()

        let paintedTiles =         
            [| for y in 0..sizeY-1 -> 
                [|for x in 0..sizeX-1 -> 
                    { x = x; y = y; painted=false} |]  |] 

        let hasBeenPainted tile =
            paintedTiles.[tile.position.y].[tile.position.x].painted

        let paintTile tile =
            paintedTiles.[tile.position.y].[tile.position.x] <- 
                { x = tile.position.x; y = tile.position.y; painted = true }

        q.Enqueue tiles.[0].[0]
        let color = tiles.[0].[0].color

        let rec workThroughQueue() =
            match q.Count with
            | 0 -> ()
            | _ ->
                let currentTile = q.Dequeue()
                if currentTile.color = color && not (hasBeenPainted currentTile) then
                    paintTile currentTile

                    let line = List<Tile>()
                    line.Add currentTile
                    line.AddRange(goWest color currentTile [])
                    line.AddRange(goEast color currentTile [])

                    for tile in line do
                        paintTile tile
                        let n = northTile tile
                        match n with
                        | Some(n) when n.color = color -> q.Enqueue n
                        | _ -> ()

                        let s = southTile tile
                        match s with
                        | Some(s) when s.color = color -> q.Enqueue s
                        | _ -> ()
                workThroughQueue()

        workThroughQueue()
        paintedTiles
        |> Array.collect (fun line -> line |> Array.filter (fun tile -> tile.painted))
        |> Array.map(fun tile -> tiles.[tile.y].[tile.x])
        |> Set.ofArray

    let createTiles sizeX sizeY numberOfColors =
        [| for y in 0..sizeY-1 -> 
            [|for x in 0..sizeX-1 -> 
                {color = createRandomColor numberOfColors; position = {x = x; y = y} }|]  |] 

    let createGameBoard sizeX sizeY numberOfColors =
        let tiles = createTiles sizeX sizeY numberOfColors
        { 
            currentColor = tiles.[0].[0].color
            sizeX        = sizeX
            sizeY        = sizeY 
            numberOfColors = numberOfColors
            tiles        = tiles
            currentTiles = calculateCurrentTileSet sizeX sizeY tiles
            numberOfMoves = 0
            score         = 0
            isFinished    = false
        }
            
    let moveBoardToNextLevel gameBoard = 
        if not gameBoard.isFinished then
            gameBoard
        else
            let newGameBoard = createGameBoard (gameBoard.sizeX+1) (gameBoard.sizeY+1) gameBoard.numberOfColors
            { newGameBoard with
                score = gameBoard.score
            }

    let paintBoardWithColor gameBoard newColor =
        if gameBoard.isFinished then
            gameBoard
        elif gameBoard.currentColor = newColor then
            gameBoard
        else
            let sizeX = gameBoard.sizeX
            let sizeY = gameBoard.sizeY

            let scoreDiff = sizeX * sizeY - gameBoard.numberOfMoves

            let tiles = [| for y in 0..sizeY-1 ->
                            [|for x in 0..sizeX-1 ->
                               let tile = gameBoard.tiles.[y].[x]
                               if Set.contains tile gameBoard.currentTiles then
                                   { tile with color = newColor }
                               else
                                   tile
                            |]
                        |]         

            let newCurrentTiles = calculateCurrentTileSet sizeX sizeY tiles
            let isFinished = newCurrentTiles.Count = gameBoard.sizeX * gameBoard.sizeY

            if isFinished then
                { gameBoard with 
                    score = gameBoard.score + scoreDiff
                    tiles = tiles
                    isFinished = true
                    numberOfMoves = gameBoard.numberOfMoves + 1
                 }
            else            
                if newCurrentTiles.Count = gameBoard.currentTiles.Count then
                    gameBoard
                else          
                    let oldTopRightTile = gameBoard.tiles.[0].[sizeX-1]
                    let oldBottomRightTile = gameBoard.tiles.[sizeY-1].[sizeX-1]
                    let oldBottomLeftTile = gameBoard.tiles.[sizeY-1].[0]
                    let newTopRightTile = tiles.[0].[sizeX-1]
                    let newBottomRightTile = tiles.[sizeY-1].[sizeX-1]
                    let newBottomLeftTile = tiles.[sizeY-1].[0]

                    let hasTile newTile oldTile =
                        (Set.contains newTile newCurrentTiles) && not (Set.contains oldTile gameBoard.currentTiles)

                    let newScore = gameBoard.score  
                                 + (if hasTile newTopRightTile oldTopRightTile then scoreDiff else 0)
                                 + (if hasTile newBottomRightTile oldBottomRightTile then scoreDiff else 0)
                                 + (if hasTile newBottomLeftTile oldBottomLeftTile then scoreDiff else 0)

                    { gameBoard with 
                        currentColor = newColor
                        score = newScore
                        tiles = tiles 
                        currentTiles = newCurrentTiles
                        numberOfMoves = gameBoard.numberOfMoves + 1
                     }

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


namespace ColorPuzzle
module GameView =
    
    open Android.Content
    open Android.Graphics
    open Android.Graphics.Drawables
    open Android.Util
    open Android.Views
    open GameBoard

    type GameView(context: Context, attr: IAttributeSet, defStyle: int) =
        inherit View(context, attr, defStyle)

        let paint = new Paint()
        let mutable gameBoard: GameBoard option = None
        let mutable height = 0;
        let mutable width = 0;

        let colors = [Color.Blue;Color.Cyan;Color.Gray;Color.Green;Color.Magenta;Color.Red;Color.Yellow]


        new (context, attrs) = new GameView(context, attrs, 0)

        member this.getColors() =
            colors
                   
        member this.setGameBoard newGameBoard =
            gameBoard <- Some(newGameBoard)
            base.Invalidate()
                     
        override this.OnSizeChanged(w, h, oldw, oldh) =  
            height <- h
            width <- w
            ()       


        override this.OnDraw(canvas) =
            match gameBoard with
            | None -> ()
            | Some(gameBoard) ->
                let tileSize = min (width / gameBoard.sizeX) (height / gameBoard.sizeY)
                let leftMargin = (width - tileSize * gameBoard.sizeX) / 2
                let topMargin = (height - tileSize * gameBoard.sizeY) / 2

                for x in 0..gameBoard.sizeX-1 do
                for y in 0..gameBoard.sizeY-1 do
                    let tile = gameBoard.tiles.[x].[y]

                    let left = single(x * tileSize + leftMargin)
                    let top = single(y * tileSize + topMargin)
                    let right = left + (single tileSize)
                    let bottom = top + (single tileSize)

                    let color = colors.[tile.color.value()]
                    paint.Color <- color
                    canvas.DrawRect(left, top, right, bottom, paint)
            
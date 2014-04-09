namespace ColorPuzzle

open System

open Android.App
open Android.Content
open Android.Graphics
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget

open ColorPuzzle.GameBoard
open ColorPuzzle.GameBoard.Persistence
open ColorPuzzle.GameView

[<Activity (Label = "ColorPuzzle", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)>]
type MainActivity () =
    inherit Activity ()

    [<Literal>]
    let bundleKey = "colorpuzzle"

    [<DefaultValue>]
    val mutable gameView : GameView 

    [<DefaultValue>]
    val mutable colorViewLayout : LinearLayout

    [<DefaultValue>]
    val mutable gameBoard : GameBoard

    [<DefaultValue>]
    val mutable movesText : TextView

    [<DefaultValue>]
    val mutable scoreText : TextView

    [<DefaultValue>]
    val mutable nextLevelButton : Button

    override this.OnCreate (savedInstanceState) =
        base.OnCreate (savedInstanceState)

        // Set our view from the "main" layout resource
        this.SetContentView (Resource_Layout.Main)

        if savedInstanceState = null then
            this.gameBoard <- createGameBoard 5 5 7
            this.gameView <- base.FindViewById<GameView> Resource_Id.gameBoard
            this.colorViewLayout <- base.FindViewById<LinearLayout> Resource_Id.colorViewLayout

            this.movesText <- base.FindViewById<TextView> Resource_Id.textMoves
            this.scoreText <- base.FindViewById<TextView> Resource_Id.textScore

            this.nextLevelButton <- base.FindViewById<Button> Resource_Id.buttonNext
            this.nextLevelButton.Click.Add(fun args -> this.NextLevel())

            this.setColors(this.gameView.getColors())
            this.gameView.setGameBoard this.gameBoard
        else
            this.LoadBoard(savedInstanceState)
        
        this.SetNextLevelButtonVisibility()
                          
    member this.setColors newColors =
        let padding = 5
        let layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent)
        layoutParams.LeftMargin <- 5
        layoutParams.RightMargin <- 5
        let b = newColors
                |> List.mapi(fun index color -> 
                                 let button = new Button(this)
                                 button.SetPadding(padding, 0, padding, 0)
                                 button.SetText(Resource_String.spaces)
                                 button.SetMinimumWidth(button.Width + padding)
                                 button.Click.Add(fun args -> this.OnClick(button, color, index)) 
                                 button.SetBackgroundColor(color)
                                 button.SetTextColor(color)
                                 this.colorViewLayout.AddView(button, layoutParams)
                                 button)        
        ()
    
    member this.NextLevel() =
        let newBoard = moveBoardToNextLevel this.gameBoard
        this.gameBoard <- newBoard
        this.gameView.setGameBoard this.gameBoard
        this.movesText.Text <- this.gameBoard.numberOfMoves.ToString()
        this.scoreText.Text <- this.gameBoard.score.ToString()
        this.SetNextLevelButtonVisibility()

    member this.OnClick((button: Button), color, colorIndex) =
        button.Background.SetColorFilter(color, PorterDuff.Mode.Multiply)
        let newBoard = paintBoardWithColor this.gameBoard (TileColor(colorIndex))
        this.gameBoard <- newBoard
        this.gameView.setGameBoard this.gameBoard
        this.movesText.Text <- this.gameBoard.numberOfMoves.ToString()
        this.scoreText.Text <- this.gameBoard.score.ToString()
        this.SetNextLevelButtonVisibility()

    member this.SetNextLevelButtonVisibility() =
        if this.gameBoard.isFinished then
            this.nextLevelButton.Visibility <- ViewStates.Visible
        else 
            this.nextLevelButton.Visibility <- ViewStates.Invisible

    override this.OnPause() =
        base.OnPause()
          
    override this.OnSaveInstanceState bundle =
        this.SaveBoard(bundle)

    member private this.SaveBoard(bundle) =
        let boardBundle = new Bundle()
        boardBundle.PutIntArray("gameBoard", saveGameBoard this.gameBoard)
        bundle.PutBundle(bundleKey, boardBundle)

    member private this.LoadBoard(bundle) = 
        let boardBundle = bundle.GetBundle bundleKey

        let gameBoardData = boardBundle.GetIntArray bundleKey
        this.gameBoard <- loadGameBoard gameBoardData
        this.gameView.setGameBoard this.gameBoard


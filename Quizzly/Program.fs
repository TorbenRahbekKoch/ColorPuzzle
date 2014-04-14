// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System

let rec GameLoop() =
    printf "Number of players: "
    Console.ReadLine() |> ignore
    //let numberOfPlayers = Console.ReadLine()

    GameLoop()

[<EntryPoint>]
let main argv = 
    GameLoop()
    0 // return an integer exit code


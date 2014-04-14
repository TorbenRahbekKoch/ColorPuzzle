namespace Quizzly
module QuizGame = 
    type Player = Name of string

    type Question = {
        Question : string
        Answers  : string list
        Correct  : int
    }
        
    type Answer = {
        Player     : string
        Question   : string
        Answer     : string
        WasCorrect : bool
        CorrectAnswer : string
    }

    type Game = {
        Players   : Player list
        Questions : Question list
        Answers   : Answer list
        CurrentPlayer : int
    }





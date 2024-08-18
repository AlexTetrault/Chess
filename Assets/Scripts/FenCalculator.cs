using UnityEngine;
using System.Threading.Tasks;

public class FenCalculator : MonoBehaviour
{
    public ChessBoard chessBoard;
    public GameManager gameManager;
    public StockfishInterface stockFish;
    public GameObject[] castles;
    public GameObject blackKing, whiteKing;
    public Canvas whiteWinsCanvas;
    public Canvas blackWinsCanvas;

    public string[] castleLetters;

    [HideInInspector] public int fullMoveNumber = 1;
    [HideInInspector] public int halfMoveNumber = 0;

    [HideInInspector] public string enPassantSquareCode;

    public GameObject squareToMoveTo;
    public GameObject squareMovingFrom;
    public GameObject movingPiece;
    public GameOptions gameOptions;
    public string fenCode;

    private void Start()
    {
        //initialize the fenCode
        fenCode = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        enPassantSquareCode = "-";
        halfMoveNumber = 0;
        fullMoveNumber = 1;
    }

    public async void UpdateFenCode()
    {
        chessBoard.UpdateBoardGrid();


        string newFenCode = "";
        int emptyCount = 0;

        //Generates first half of FEN code
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject piece = chessBoard.boardGrid[x, y];

                if (piece == null)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        newFenCode += emptyCount;
                        emptyCount = 0;
                    }

                    newFenCode += piece.name;
                }
            }

            if (emptyCount > 0)
            {
                newFenCode += emptyCount;
                emptyCount = 0;
            }

            if (y > 0)
            {
                newFenCode += "/";
            }
        }

        //Generates second half of FEN code

        newFenCode += gameManager.isWhitesMove ? " w " : " b ";

        newFenCode += CalculateCastles().Length == 0? "- " : CalculateCastles() + " ";

        newFenCode += enPassantSquareCode + " ";

        newFenCode += $"{halfMoveNumber} {fullMoveNumber}";

        fenCode = newFenCode;

        Debug.Log("Fen Code is: " + newFenCode);

        gameManager.legalMoves = await stockFish.CalculateLegalMoveList(newFenCode);

        if (gameManager.isWhitesMove != gameOptions.isPlayingWhite)
        {
            string suggestedMove = await GetBestMove(newFenCode);
            Debug.Log("Stockfish suggests: " + suggestedMove);

            //it is the bot's move, have the bot wait a second before making move. 
            await Task.Delay(1000);

            //Have the bot make a random move sometimes (to make it perform worse) depending on difficulty setting. Only if there are legal moves available.
            if (gameManager.legalMoves.Count > 0)
            {
                int randomValue = Random.Range(0, 11);
                if (randomValue < gameOptions.randomMoveChance)
                {
                    int randomMove = Random.Range(0, gameManager.legalMoves.Count - 1);
                    suggestedMove = gameManager.legalMoves[randomMove];
                }
            }

            //carry out the move.
            gameManager.moveCode = suggestedMove;
            gameManager.possibleMoves.Clear();
            gameManager.GenerateAIMove(suggestedMove);
        }
        else
        {
            //tell stockfish what the current fen code is to support sync.
            await stockFish.SendCommand($"position fen {newFenCode}");
        }
    }

    string CalculateCastles()
    {
        //initialize the castle code
        string castlesString = string.Empty;

        //There are 4 possible castle options in the game. First 2 letters of the code are for white, last 2 are for black.
        for (int i = 0; i < 4; i++)
        {
            if (i <= 1)
            {
                //to be a valid castle, the rook must be in play and has not moved. The King must not have ever moved aswell.
                if (castles[i].GetComponent<ChessPiece>().hasMoved == false && whiteKing.GetComponent<ChessPiece>().hasMoved == false && castles[i].GetComponent<SpriteRenderer>().enabled == true)
                {
                    castlesString += castleLetters[i];
                }
            }
            else
            {
                //to be a valid castle, the rook must be in play and has not moved. The King must not have ever moved aswell.
                if (castles[i].GetComponent<ChessPiece>().hasMoved == false && blackKing.GetComponent<ChessPiece>().hasMoved == false && castles[i].GetComponent<SpriteRenderer>().enabled == true)
                {
                    castlesString += castleLetters[i];
                }
            }
        }

        return castlesString;

    }

    public async Task <string> GetBestMove(string fen)
    {
        string bestMove = await stockFish.GetBestMove(fen, gameOptions.botDifficulty, gameOptions.botDepth);

        if (bestMove == "(none)")
        {
            string staleorcheck = await stockFish.CheckIfMate(fen);
            return staleorcheck;
        }
        return bestMove;
    }

    void ShowEndGameCanvas()
    {
        if (gameManager.isWhitesMove)
        {
            blackWinsCanvas.enabled = true;
        }
        else
        {
            whiteWinsCanvas.enabled = true;
        }
    }
}

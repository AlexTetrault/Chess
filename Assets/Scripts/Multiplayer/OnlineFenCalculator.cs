using UnityEngine;

public class OnlineFenCalculator : MonoBehaviour
{
    public string fenCode;

    public OnlineChessBoard chessBoard;
    public OnlineGameManager gameManager;
    public OnlineStockfish stockFish;
    public GameObject[] castles;
    public GameObject blackKing, whiteKing;

    public string[] castleLetters;

    [HideInInspector] public int fullMoveNumber = 1;
    [HideInInspector] public int halfMoveNumber = 0;
    [HideInInspector] public string enPassantSquareCode;
    [HideInInspector] public GameObject squareToMoveTo;
    [HideInInspector] public GameObject squareMovingFrom;
    [HideInInspector] public GameObject movingPiece;
    

    private void Start()
    {
        //initialize the fenCode
        enPassantSquareCode = "-";
        halfMoveNumber = 0;
        fullMoveNumber = 1;
    }

    public async void UpdateFenCode()
    {
        chessBoard.UpdateBoardGrid();

        //start with an empty code and build it throughout the function.
        string newFenCode = "";

        //Generates first half of FEN code by looking at the board pieces and their positions.
        int emptyCount = 0;

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

        //add who's turn it is to FEN.
        newFenCode += gameManager.isWhitesMove ? " w " : " b ";

        //add legal castles to FEN.
        newFenCode += CalculateCastles().Length == 0 ? "- " : CalculateCastles() + " ";

        //add en passant code.
        newFenCode += enPassantSquareCode + " ";

        //update half move and full move counter.
        newFenCode += $"{halfMoveNumber} {fullMoveNumber}";

        //update FEN 
        fenCode = newFenCode;

        //generate the list of legal moves the player can make.
        gameManager.legalMoves = await stockFish.CalculateLegalMoveList(newFenCode);

        //tell stockfish what the current fen code is to support sync.
        await stockFish.SendCommand($"position fen {newFenCode}");
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
}

using UnityEngine;
using System.Threading.Tasks;
using System;

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
    public string fenCode;

    private void Start()
    {
        //initialize the fenCode
        fenCode = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
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

        newFenCode += CalculateCastles().Length == 0? CalculateCastles() : CalculateCastles() + " ";


        newFenCode += enPassantSquareCode + " ";

        newFenCode += "0 1"; // Simplified for illustration

        fenCode = newFenCode;

        Debug.Log(newFenCode);

        gameManager.legalMoves = await stockFish.CalculateLegalMoveList(newFenCode);
        string suggestedMove = await GetBestMove(newFenCode);
        Debug.Log(suggestedMove);
    }

    string CalculateCastles()
    {
        string castlesString = string.Empty;
        for (int i = 0; i < 4; i++)
        {
            if (i <= 1)
            {
                if (castles[i].GetComponent<ChessPiece>().hasMoved == false && whiteKing.GetComponent<ChessPiece>().hasMoved == false)
                {
                    castlesString += castleLetters[i];
                }
            }
            else
            {
                if (castles[i].GetComponent<ChessPiece>().hasMoved == false && blackKing.GetComponent<ChessPiece>().hasMoved == false)
                {
                    castlesString += castleLetters[i];
                }
            }
        }

        return castlesString;

    }

    public async Task <string> GetBestMove(string fen)
    {
        string bestMove = await stockFish.GetBestMove(fen, 5);

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

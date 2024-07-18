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

    public void UpdateFenCode()
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

        GetBestMove(newFenCode);
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

    public async void GetBestMove(string fen)
    {
        Debug.Log("GetBestMove called with FEN: " + fen);

        try
        {
            string bestMove = await stockFish.GetBestMove(fen, 15);
            Debug.Log("Best move received: " + bestMove);

            if (bestMove == "(none)")
            {
                ShowEndGameCanvas();
                return;
            }

            string currentSquare = bestMove.Substring(0, 2);
            string destinationSquare = bestMove.Substring(2, 2);

            squareToMoveTo = GameObject.Find(destinationSquare);
            squareMovingFrom = GameObject.Find(currentSquare);

            if (squareMovingFrom != null)
            {
                squareMovingFrom.GetComponent<Collider>().enabled = false;

                RaycastHit hit;
                if (Physics.Raycast(squareMovingFrom.transform.position, Vector3.back, out hit))
                {
                    movingPiece = hit.collider.gameObject;

                    await Task.Delay(200); // Delay before making move

                    if (movingPiece != null && squareToMoveTo != null)
                    {
                        movingPiece.GetComponent<MouseDrag>().AIMove(squareToMoveTo.transform.localPosition);
                    }
                    else
                    {
                        Debug.Log("Didn't have the info required");
                    }
                }

                squareMovingFrom.GetComponent<Collider>().enabled = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error in GetBestMove: " + e.Message);
        }
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

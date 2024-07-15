using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class FenCalc : MonoBehaviour
{
    public string FenCode;
    [SerializeField] GameObject[] squares;
    Vector3 offset = new Vector3(0, 0, -0.5f);
    public string FenCodeOtherHalf;
    GameManager manager;
    public GameObject[] castles;
    public string[] castleLetters;
    public GameObject blackKing, whiteKing;
    public int fullMoveNumber = 1;
    public int halfMoveNumber = 0;
    public string enPassantCode = " - ";

    public StockfishInterface stockFish;

    // Start is called before the first frame update
    void Start()
    {
        //initialize the fen code. 
        FenCode = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        FenCodeOtherHalf = " w KQkq - 0 1";
        manager = GetComponent<GameManager>();
    }

    public void UpdateFenCode()
    {
        int numOfEmpty = 0;
        string newFenCode = "";

        if (manager.isWhitesMove)
        {
            fullMoveNumber++;
        }
        
        for (int i = 0; i < squares.Length; i++)
        {
            RaycastHit hit;
            Vector3 rayOrigin = squares[i].transform.position + offset;

            if (Physics.Raycast(rayOrigin, Vector3.back, out hit, 10))
            {
                if (numOfEmpty != 0)
                {
                    newFenCode += numOfEmpty.ToString();
                    numOfEmpty = 0;
                }
                newFenCode += hit.collider.gameObject.name;
            }
            else
            {
                numOfEmpty++;
            }

            if (numOfEmpty == 8)
            {
                newFenCode += numOfEmpty.ToString();
                numOfEmpty = 0;
            }

            if ((i + 1) % 8 == 0 && i != 63)
            {
                if (numOfEmpty > 0)
                {
                    newFenCode += numOfEmpty.ToString();
                    numOfEmpty = 0;
                }
                newFenCode += "/";
            }
        }

        if (manager.isWhitesMove)
        {
            newFenCode += " w ";
        }
        else
        {
            newFenCode += " b ";
        }

        newFenCode += CalculateCastles();

        newFenCode += enPassantCode;

        newFenCode += halfMoveNumber.ToString() + " ";

        newFenCode += fullMoveNumber.ToString();

        GetBestMove(newFenCode);

    }

    public async void GetBestMove(string fen)
    {
        string bestMove = await stockFish.GetBestMove(fen, 5);
        Debug.Log(bestMove);

        //check for checkmate
        if (bestMove == "(none)")
        {
            if (manager.isWhitesMove)
            {
                Debug.Log("CHECKMATE: BLACK WINS");
            }
            else
            {
                Debug.Log("CHECKMATE: WHITE WINS");
            }
        }
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
}

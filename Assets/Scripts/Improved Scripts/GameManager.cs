using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> blackPieces = new List<GameObject>();
    public List<GameObject> whitePieces = new List<GameObject>();

    public string moveCode;
    public bool isWhitesMove;

    public List<Vector2> possibleMoves = new List<Vector2>();

    public List<GameObject> availSquares;

    public GameObject blackKing;
    public GameObject whiteKing;

    public ChessBoard chessBoard;

    public List<GameObject> pawnsAllowedToEnPassant = new List<GameObject>();

    public List<string> legalMoves = new List<string>();

    public Canvas blackWins;
    public Canvas whiteWins;
    public Canvas staleMate;

    private void Start()
    {
        isWhitesMove = true;
        possibleMoves.Clear();
    }

    public void ChangeTurn()
    {
        isWhitesMove = isWhitesMove? false : true;
    }

    public void AttackEnemyPiece(GameObject activePiece)
    {
        Vector3 activePiecePos = activePiece.transform.position;
        bool pieceIsWhite = activePiece.GetComponent<ChessPiece>().isWhite;

        //if moving piece is white, check to see if its position is the same as any of the black pieces.
        if (pieceIsWhite)
        {
            foreach (GameObject blackPiece in blackPieces)
            {
                Vector3 blackPiecePos = blackPiece.transform.position;
                if (activePiecePos == blackPiecePos)
                {
                    //moving to a black piece, take black piece off of the board. 
                    blackPiece.GetComponent<SpriteRenderer>().enabled = false;
                    blackPiece.GetComponent<Collider>().enabled = false;
                    blackPieces.Remove(blackPiece);
                    chessBoard.chessPieces.Remove(blackPiece);
                    break;
                }
            }
        }

        //if moving piece is black, check to see if its position is the dame as any of the white pieces.
        else
        {
            foreach (GameObject whitePiece in whitePieces)
            {
                Vector3 whitePiecePos = whitePiece.transform.position;
                if (activePiecePos == whitePiecePos)
                {
                    //moving to a white piece, take white piece off of the board. 
                    whitePiece.GetComponent<SpriteRenderer>().enabled = false;
                    whitePiece.GetComponent<Collider>().enabled = false;
                    whitePieces.Remove(whitePiece);
                    chessBoard.chessPieces.Remove(whitePiece);
                    break;
                }
            }
        }

    }

    public void DetermineWinner()
    {
        Canvas winner = isWhitesMove ? blackWins : whiteWins;
        winner.enabled = true;
    }

    public void StaleMate()
    {
        staleMate.enabled = true;
    }
}



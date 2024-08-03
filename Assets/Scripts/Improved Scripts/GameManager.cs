using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public List<GameObject> blackPieces = new List<GameObject>();
    public List<GameObject> whitePieces = new List<GameObject>();

    public string moveCode;
    public bool isWhitesMove;

    public GameObject enPassantVictim;

    public List<Vector2> possibleMoves = new List<Vector2>();

    public List<GameObject> availSquares;

    public GameObject blackKing;
    public GameObject whiteKing;

    public ChessBoard chessBoard;

    public List<string> legalMoves = new List<string>();

    public Canvas blackWins;
    public Canvas whiteWins;
    public Canvas staleMate;

    public GameObject movingSquare;
    public GameObject destinationSquare;

    public bool gameHasStarted;

    AudioManager audioManager;

    private void Start()
    {
        isWhitesMove = true;
        possibleMoves.Clear();
        audioManager = GetComponent<AudioManager>();
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

        //if moving piece is black, check to see if its position is the same as any of the white pieces.
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

    public void GenerateAIMove(string moveCode)
    {
        Debug.Log("AI is going to move now");
        //find the sqaure the AI wants to move from and to.
        movingSquare = GameObject.Find(moveCode.Substring(0, 2));
        destinationSquare = GameObject.Find(moveCode.Substring(2, 2));

        //get reference to the chess piece the AI wants to move and move it to the same x and y position as the destination square.
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(movingSquare.transform.position.x, movingSquare.transform.position.y, -5);

        Vector2 newPos = new Vector2(destinationSquare.transform.localPosition.x, destinationSquare.transform.localPosition.y);

        if (Physics.Raycast(rayOrigin, Vector3.forward, out hit))
        {
            Debug.Log(hit.collider.name);
            hit.collider.GetComponent<MouseDrag>().initialPos = hit.collider.transform.localPosition;
            hit.collider.GetComponent<MouseDrag>().GenerateMove(newPos);
        }
        else
        {
            Debug.Log("Couldn't find chesspiece");
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

    public void PlayRandomPieceMoveSound()
    {
        audioManager.PlayRandomSound();
    }
}



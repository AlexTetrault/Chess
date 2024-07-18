using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> blackPieces = new List<GameObject>();
    public List<GameObject> whitePieces = new List<GameObject>();

    public bool isWhitesMove;

    public List<Vector2> possibleMoves = new List<Vector2>();

    public List<Vector2> whiteControlledSpaces = new List<Vector2>();
    public List<Vector2> blackControlledSpaces = new List<Vector2>();

    public List<GameObject> availSquares;

    public GameObject blackKing;
    public GameObject whiteKing;

    public ChessBoard chessBoard;

    int layerMask;

    private void Start()
    {
        layerMask = LayerMask.GetMask("Square");
        isWhitesMove = true;
        possibleMoves.Clear();
    }

    public void ChangeTurn()
    {
        isWhitesMove = isWhitesMove? false : true;
    }

    public void ShowAvailMoves(Vector2 startPos)
    {
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            Vector2 origin = startPos + possibleMoves[i];
            Vector3 rayOrigin = new Vector3(origin.x, origin.y, -1f);
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, Vector3.forward, out hit, 10, layerMask))
            {
                // If the ray hits an object in the "Square" layer
                availSquares.Add(hit.collider.gameObject);
                hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
    }

    public void HideAvailMoves()
    {
        foreach (GameObject square in availSquares)
        {
            square.GetComponent<BoardSquare>().ResetColor();
        }
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
}



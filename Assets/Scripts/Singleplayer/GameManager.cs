using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> blackPieces = new List<GameObject>();
    public List<GameObject> whitePieces = new List<GameObject>();

    public string moveCode;
    public bool isWhitesMove;

    public bool isPlayersMove;

    public GameObject enPassantVictim;

    public List<Vector2> possibleMoves = new List<Vector2>();

    public List<GameObject> availSquares;

    public GameObject blackKing;
    public GameObject whiteKing;

    public int removeAttempts = 5;

    public ChessBoard chessBoard;

    public List<string> legalMoves = new List<string>();

    public List<GameObject> destinationSquares = new List<GameObject>();

    public Canvas blackWins;
    public Canvas whiteWins;
    public Canvas staleMate;

    public GameObject movingSquare;
    public GameObject destinationSquare;

    public bool gameHasStarted;

    public AudioManager audioManager;

    public GameOptions gameOptions;

    public FenCalculator fenCalculator;

    public Sprite whitePawn, whiteRook, whiteKnight, whiteBishop, whiteQueen;
    public Sprite blackPawn, blackRook, blackKnight, blackBishop, blackQueen;

    int layerMask = 1 << 7;

    private void Start()
    {
        isWhitesMove = true;
        removeAttempts = 5;
        possibleMoves.Clear();
        isPlayersMove = true;
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

                    //attacking an opponents piece resets the half move counter to 0.
                    fenCalculator.halfMoveNumber = 0;

                    blackPieces.Remove(blackPiece);
                    chessBoard.chessPieces.Remove(blackPiece);

                    //game logic for infect-chess.
                    if (!gameOptions.isPlayingInfectChess)
                    {
                        break;
                    }

                    //kings cannot be infected.
                    if (activePiece.tag == "King")
                    {
                        break;
                    }

                    if (blackPiece.tag == "Pawn")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = whitePawn;
                    }
                    if (blackPiece.tag == "Rook")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = whiteRook;
                    }
                    if (blackPiece.tag == "Knight")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = whiteKnight;
                    }
                    if (blackPiece.tag == "Bishop")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = whiteBishop;
                    }
                    if (blackPiece.tag == "Queen")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = whiteQueen;
                    }

                    activePiece.tag = blackPiece.tag;
                    activePiece.name = blackPiece.name.ToUpper();

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

                    //attacking an opponents piece resets the half move counter to 0.
                    fenCalculator.halfMoveNumber = 0;

                    whitePieces.Remove(whitePiece);
                    chessBoard.chessPieces.Remove(whitePiece);

                    //game logic for infect-chess.
                    if (!gameOptions.isPlayingInfectChess)
                    {
                        break;
                    }

                    //kings cannot be infected.
                    if (activePiece.tag == "King")
                    {
                        break;
                    }

                    if (whitePiece.tag == "Pawn")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = blackPawn;
                    }
                    if (whitePiece.tag == "Rook")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = blackRook;
                    }
                    if (whitePiece.tag == "Knight")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = blackKnight;
                    }
                    if (whitePiece.tag == "Bishop")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = blackBishop;
                    }
                    if (whitePiece.tag == "Queen")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = blackQueen;
                    }

                    activePiece.tag = whitePiece.tag;
                    activePiece.name = whitePiece.name.ToLower();

                    break;
                }
            }
        }
    }

    public void GenerateAIMove(string moveCode)
    {
        //find the sqaure the AI wants to move from and to.
        movingSquare = GameObject.Find(moveCode.Substring(0, 2));
        destinationSquare = GameObject.Find(moveCode.Substring(2, 2));

        //get reference to the chess piece the AI wants to move and move it to the same x and y position as the destination square.
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(movingSquare.transform.position.x, movingSquare.transform.position.y, -5);

        Vector2 newPos = new Vector2(destinationSquare.transform.localPosition.x, destinationSquare.transform.localPosition.y);

        if (Physics.Raycast(rayOrigin, Vector3.forward, out hit, Mathf.Infinity, layerMask))
        {
            hit.collider.GetComponent<MouseDrag>().initialPos = hit.collider.transform.localPosition;
            hit.collider.GetComponent<MouseDrag>().GenerateMove(newPos);
            removeAttempts = 5;
            isPlayersMove = true;
        }
        //stockfish provided an illegal move, we will do a max of 5 attempts to retry and get a correct move. 
        else
        {
            if (removeAttempts > 0)
            {
                fenCalculator.UpdateFenCode();
                removeAttempts--;
            }
            else
            {
                Debug.Log("Could not get a move");
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

    public void PlayRandomPieceMoveSound()
    {
        audioManager.PlayRandomSound();
    }
}



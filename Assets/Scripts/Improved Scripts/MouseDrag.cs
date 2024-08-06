using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    Vector2 difference;
    public Vector2 initialPos;

    SpriteRenderer spriteRenderer;
    ChessPiece chessPiece;

    Color opaque = new Color(1f, 1f, 1f, 1f);
    Color translucent = new Color(1f, 1f, 1f, 0.75f);

    public FenCalculator fenCalculator;
    public GameManager gameManager;
    public PawnPromotion pawnPromotion;
    public ChessBoard chessBoard;
    public GameOptions gameOptions;

    bool movingToEnPassantSquare = false;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        chessPiece = GetComponent<ChessPiece>();
    }

    private void OnMouseDown()
    {
        //acts to disable script function when it is not the player's turn or the piece does not belong to the player. Disabling script does not stop OnMouse function. Annoying.
        if (!gameManager.isWhitesMove || chessPiece.isWhite != gameOptions.isPlayingWhite)
        {
            return;
        }

        gameManager.possibleMoves.Clear();
        gameManager.moveCode = string.Empty;

        //create a raycast behind the board and see what square the piece is on.
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y, 10);

        if (Physics.Raycast(rayOrigin, Vector3.back, out hit))
        {
            if (hit.collider.tag == "Square")
            {
                gameManager.moveCode += hit.collider.gameObject.name;
            }
        }

        initialPos = transform.localPosition;
        difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;

        spriteRenderer.color = translucent;
    }

    private void OnMouseDrag()
    {
        //acts to disable script function when it is not the player's turn or the piece does not belong to the player. Disabling script does not stop OnMouse function. Annoying.
        if (!gameManager.isWhitesMove || chessPiece.isWhite != gameOptions.isPlayingWhite)
        {
            return;
        }

        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;
    }

    private void OnMouseUp()
    {
        //acts to disable script function when it is not the player's turn or the piece does not belong to the player. Disabling script does not stop OnMouse function. Annoying.
        if (!gameManager.isWhitesMove || chessPiece.isWhite != gameOptions.isPlayingWhite)
        {
            return;
        }

        //create a raycast behind the board and see what square the piece is moving to.
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y, 10);

        if (Physics.Raycast(rayOrigin, Vector3.back, out hit))
        {
            if (hit.collider.tag == "Square")
            {
                gameManager.moveCode += hit.collider.gameObject.name;

                if (hit.collider.name == fenCalculator.enPassantSquareCode)
                {
                    movingToEnPassantSquare = true;
                }
                else
                {
                    movingToEnPassantSquare = false;
                }
            }
        }

        //check if the generated move is in the list of legal moves. If it isn't, do not complete the move.
        if (!gameManager.legalMoves.Contains(gameManager.moveCode))
        {
            transform.localPosition = initialPos;
            spriteRenderer.color = opaque;
            return;
        }

        //The move is legal, carry out the move and check for special conditions (castling, en passant, attacking)

        Vector2 newPos = new Vector2(hit.transform.position.x, hit.transform.position.y);
        spriteRenderer.color = opaque;
        GenerateMove(newPos);

        gameManager.isPlayersMove = false;
    }

    public void GenerateMove(Vector2 newPos)
    {
        //snap the piece to the centre of the destination square
        transform.localPosition = newPos;

        //every time a piece moves, this is considered a half turn (plie)
        fenCalculator.halfMoveNumber++;

        //whenever we move a pawn, the half number is reset to 0.
        if (tag == "Pawn")
        {
            fenCalculator.halfMoveNumber = 0;
        }

        //check if the piece is moving to a square occupied by a piece of the enemy color.
        gameManager.AttackEnemyPiece(gameObject);

        //check for special conditions.
        CheckIfCastling(initialPos.x, newPos.x);
        CheckIfEnPassanting(initialPos, newPos);
        CheckIfPromotingPawn();

        //this is for kings and rooks when calculating castle abilities.
        chessPiece.hasMoved = true;

        gameManager.PlayRandomPieceMoveSound();

        //if black is playing their move, a full turn has occured.
        if (!gameManager.isWhitesMove)
        {
            fenCalculator.fullMoveNumber++;
        }

        //move is finished. It is now the opponent's turn, notify if they have an opportunity to en passant.
        gameManager.ChangeTurn();
        fenCalculator.enPassantSquareCode = CalculateEnPassantCode(initialPos.y, newPos.y);

        //generate the new fen code so the opponent can perform their move. 
        fenCalculator.UpdateFenCode();
    }

    private void CheckIfPromotingPawn()
    {
        //for now, assume we will always turn our pawn into a queen.

        if (gameObject.tag != "Pawn")
        {
            return;
        }

        if (chessPiece.isWhite)
        {
            if (transform.localPosition.y == 7)
            {
                gameObject.name = "Q";
                spriteRenderer.sprite = pawnPromotion.queenSpriteW;
                gameObject.tag = "Queen";
            }
        }
        else
        {
            if (transform.localPosition.y == 0)
            {
                gameObject.name = "q";
                spriteRenderer.sprite = pawnPromotion.queenSpriteB;
                gameObject.tag = "Queen";
            }
        }
    }

    private void CheckIfEnPassanting(Vector2 initialPos, Vector2 newPos)
    {
        //only pawns can en passant
        if (tag != "Pawn")
        {
            return;
        }

        //there is no en passant victim, therefore en passant is impossible.
        if (gameManager.enPassantVictim == null)
        {
            return;
        }

        //make sure this diagonal pawn move is moving to the en passant sqaure, not just a regular attack.
        if (movingToEnPassantSquare == false)
        {
            return;
        }

        if (Mathf.Abs(initialPos.x - newPos.x) == 1 && Mathf.Abs(initialPos.y - newPos.y) == 1) 
        {
            gameManager.enPassantVictim.GetComponent<SpriteRenderer>().enabled = false;
            gameManager.enPassantVictim.GetComponent<Collider>().enabled = false;
            chessBoard.chessPieces.Remove(gameManager.enPassantVictim);
        }
    }

    private string CalculateEnPassantCode(float initialYPos, float newYPos)
    {
        //pawn did not move this turn, therfore no en passant allowed.
        if (tag != "Pawn")
        {
            if (gameManager.enPassantVictim != null)
            {
                gameManager.enPassantVictim = null;
            }
            return "-";
        }

        //pawn did not move 2 spaces this turn, therefore no en passant allowed.
        if (Mathf.Round(Mathf.Abs(initialYPos - newYPos)) != 2)
        {
            if (gameManager.enPassantVictim != null)
            {
                gameManager.enPassantVictim = null;
            }
            return "-";
        }

        //find the name of the square behind the pawn and add it to the Fen code.
        RaycastHit hit;
        Vector3 behindPawn = chessPiece.isWhite ? new Vector3(0, -1, 0) : new Vector3(0, 1, 0);

        if (Physics.Raycast(transform.position + behindPawn, Vector3.forward, out hit))
        {
            gameManager.enPassantVictim = gameObject;
            return hit.collider.name;
        }

        //catch all, default is setting code to no en passant allowed. 
        if (gameManager.enPassantVictim != null)
        {
            gameManager.enPassantVictim = null;
        }
        return "-";
    }

    private void CheckIfCastling(float initialXPos, float newXPos)
    {
        //if we are not moving a king, we are not castling.
        if (tag != "King")
        {
            return;
        }

        //if the king is not moving 2 spaces to the left or right, we are not castling.
        if (Mathf.Round(Mathf.Abs(initialXPos - newXPos)) != 2)
        {
            return;
        }

        //we are castling.

        RaycastHit hit;

        float direction = newXPos - initialXPos > 0 ? 1 : -1;
        Vector3 rayDirection = new Vector3(direction, 0, 0);

        gameObject.GetComponent<Collider>().enabled = false;

        if (Physics.Raycast(transform.position, rayDirection, out hit))
        {
            hit.transform.position = transform.position - rayDirection;
        }

        gameObject.GetComponent<Collider>().enabled = true;
    }
}

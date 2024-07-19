using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    Vector2 difference;
    Vector2 initialPos;

    SpriteRenderer spriteRenderer;
    ChessPiece chessPiece;

    float minBoardRange, maxBoardRange;

    Color opaque = new Color(1f, 1f, 1f, 1f);
    Color translucent = new Color(1f, 1f, 1f, 0.75f);

    public FenCalculator fenCalculator;
    public PieceBehaviour pieceBehaviour;
    public GameManager gameManager;

    public PawnPromotion pawnPromotion;

    public ChessBoard chessBoard;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        chessPiece = GetComponent<ChessPiece>();
        minBoardRange = -0.5f;
        maxBoardRange = 7.5f;
    }

    public void AIMove(Vector3 newPos)
    {
        gameManager.possibleMoves.Clear();
        pieceBehaviour.activePiece = gameObject;
        pieceBehaviour.LineOfSight();
        initialPos = transform.localPosition;
        transform.localPosition = newPos;
        SnapToClosestSquare();
    }

    private void OnMouseDown()
    {
        //acts to disable script function when it is not the player's turn. Disabling script does not stop OnMouse function. Annoying.
        if (chessPiece.isWhite != gameManager.isWhitesMove)
        {
            return;
        }
        gameManager.possibleMoves.Clear();

        initialPos = transform.localPosition;
        difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        spriteRenderer.color = translucent;

        pieceBehaviour.activePiece = gameObject;
        pieceBehaviour.LineOfSight();
        gameManager.ShowAvailMoves(initialPos);
    }

    private void OnMouseDrag()
    {
        //acts to disable script function when it is not the player's turn. Disabling script does not stop OnMouse function. Annoying.
        if (chessPiece.isWhite != gameManager.isWhitesMove)
        {
            return;
        }

        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;
    }

    private void OnMouseUp()
    {
        //acts to disable script function when it is not the player's turn. Disabling script does not stop OnMouse function. Annoying.
        if (chessPiece.isWhite != gameManager.isWhitesMove)
        {
            return;
        }
        spriteRenderer.color = opaque;
        SnapToClosestSquare();
        gameManager.HideAvailMoves();
    }

    private void SnapToClosestSquare()
    {
        Vector2 currentPos = transform.localPosition;
        Vector2 newPos = new Vector2(Mathf.Round(currentPos.x), Mathf.Round(currentPos.y));

        if (currentPos.x < minBoardRange || currentPos.x > maxBoardRange || currentPos.y < minBoardRange || currentPos.y > maxBoardRange)
        {
            //drag is out of bounds, revert back to inital position
            transform.localPosition = initialPos;
            return;
        }

        transform.localPosition = newPos;

        if (!pieceBehaviour.ValidMove(initialPos, newPos))
        {
            //move was not one of the valid possibilities calculated, revert back to initial position
            transform.localPosition = initialPos;
            return;
        }

        //refresh en passant info before calculating
        if (gameManager.pawnsAllowedToEnPassant.Count > 0)
        {
            foreach (GameObject pawn in gameManager.pawnsAllowedToEnPassant)
            {
                pawn.GetComponent<ChessPiece>().canEnPassant = false;
            }
        }

        gameManager.pawnsAllowedToEnPassant.Clear();

        chessPiece.hasMoved = true;
        gameManager.AttackEnemyPiece(gameObject);

        CheckIfCastling(initialPos.x, newPos.x);
        CheckIfDoingEnPassant(initialPos, newPos);
        NotifyNeighborsOfEnPassant(initialPos.y, newPos.y);
        CheckIPromotingPawn();
        gameManager.ChangeTurn();
        fenCalculator.enPassantSquareCode = CalculateEnPassantCode(initialPos.y, newPos.y);
        fenCalculator.UpdateFenCode();

    }

    public void CheckIPromotingPawn()
    {
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

    void NotifyNeighborsOfEnPassant(float initialPosY, float newPosY)
    {
        if (tag != "Pawn")
        {
            return;
        }

        if (Mathf.Round(Mathf.Abs(initialPosY - newPosY)) != 2)
        {
            return;
        }

        pieceBehaviour.enPassantVictim = gameObject;

        RaycastHit leftHit;
        RaycastHit rightHit;

        gameObject.GetComponent<Collider>().enabled = false;

        if (Physics.Raycast(transform.position, Vector3.left, out leftHit, 1.5f))
        {
            if (leftHit.collider.gameObject.tag != "Pawn")
            {
                gameObject.GetComponent<Collider>().enabled = true;
                return;
            }

            if (leftHit.collider.GetComponent<ChessPiece>().isWhite == chessPiece.isWhite)
            {
                gameObject.GetComponent<Collider>().enabled = true;
                return;
            }

            leftHit.collider.GetComponent<ChessPiece>().canEnPassant = true;
            gameManager.pawnsAllowedToEnPassant.Add(leftHit.collider.gameObject);
        }

        if (Physics.Raycast(transform.position, Vector3.right, out rightHit, 1.5f))
        {
            if (rightHit.collider.gameObject.tag != "Pawn")
            {
                gameObject.GetComponent<Collider>().enabled = true;
                return;
            }

            if (rightHit.collider.GetComponent<ChessPiece>().isWhite == chessPiece.isWhite)
            {
                gameObject.GetComponent<Collider>().enabled = true;
                return;
            }

            rightHit.collider.GetComponent<ChessPiece>().canEnPassant = true;
            gameManager.pawnsAllowedToEnPassant.Add(rightHit.collider.gameObject);
        }

        gameObject.GetComponent<Collider>().enabled = true;
    }

    void CheckIfDoingEnPassant(Vector2 initialPos, Vector2 newPos)
    {
        if (Mathf.Abs(initialPos.x - newPos.x) == 1 && Mathf.Abs(initialPos.y - newPos.y) == 1) 
        {
            pieceBehaviour.enPassantVictim.GetComponent<SpriteRenderer>().enabled = false;
            pieceBehaviour.enPassantVictim.GetComponent<Collider>().enabled = false;
            chessBoard.chessPieces.Remove(pieceBehaviour.enPassantVictim);
        }
    }

    public string CalculateEnPassantCode(float initialYPos, float newYPos)
    {
        //pawn did not move this turn, therfore no en passant allowed.
        if (tag != "Pawn")
        {
            return "-";
        }

        //pawn did not move 2 spaces this turn, therefore no en passant allowed.
        if (Mathf.Round(Mathf.Abs(initialYPos - newYPos)) != 2)
        {
            return "-";
        }

        //find the name of the square behind the pawn and add it to the Fen code.
        RaycastHit hit;
        Vector3 behindPawn = chessPiece.isWhite ? new Vector3(0, -1, 0) : new Vector3(0, 1, 0);

        if (Physics.Raycast(transform.position + behindPawn, Vector3.forward, out hit))
        {
            return hit.collider.name;
        }

        //catch all, default is setting code to no en passant allowed. 
        return "-";
    }

    void CheckIfCastling(float initialXPos, float newXPos)
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

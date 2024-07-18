using System.Collections;
using System.Collections.Generic;
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

        chessPiece.hasMoved = true;
        gameManager.AttackEnemyPiece(gameObject);
        CheckIfCastling(initialPos.x, newPos.x);
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

using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.UI.Image;

public class DragDrop : MonoBehaviour
{
    Vector2 difference;
    SpriteRenderer spriteRenderer;
    Vector2 startPos;
    public GameManager gameManager;
    bool transparent = false;
    float boardMinRange = -3.5f;
    float boardMaxRange = 4.0f;
    ChessPiece piece;
    bool validMove;
    Collider myCollider;
    Vector2 realStartPos;

    public FenCalc fenCalc;

    public PieceBehaviour pieceBehaviour;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        piece = GetComponent<ChessPiece>();
        myCollider = GetComponent<Collider>();
    }

    private void OnMouseDown()
    {
        if (piece.isWhite != gameManager.isWhitesMove)
        {
            return;
        }

        piece.LineOfSight();
        SwitchTransparency();
        startPos = transform.localPosition;
        realStartPos = transform.position;
        difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        gameManager.ShowAvailMoves(realStartPos);

    }

    public void AIMoveInit()
    {
        piece.LineOfSight();
        startPos = transform.localPosition;
        realStartPos = transform.position;
    }

    public void AIMove(Vector3 move)
    {
        transform.position = move;
        SnapToClosestSquare();
        CheckIfMoveIsValid();
        CheckIfCastling();
        CheckForEnPassant();
        EnpassantTake();
        CheckForPawnPromotion();

        gameManager.possibleMoves.Clear();
        gameManager.possibleAttacks.Clear();

        if (piece.isWhite)
        {
            for (int i = 0; i < gameManager.blackPieces.Count; i++)
            {
                if (piece.transform.localPosition == gameManager.blackPieces[i].transform.localPosition)
                {
                    gameManager.blackPieces[i].GetComponent<SpriteRenderer>().enabled = false;
                    gameManager.blackPieces[i].GetComponent<DragDrop>().enabled = false;
                    gameManager.blackPieces[i].GetComponent<Collider>().enabled = false;
                    gameManager.blackPieces.RemoveAt(i);
                    gameManager.ChangeTurn();
                    fenCalc.halfMoveNumber = 0;
                    fenCalc.UpdateFenCode();
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < gameManager.whitePieces.Count; i++)
            {
                if (piece.transform.localPosition == gameManager.whitePieces[i].transform.localPosition)
                {
                    gameManager.whitePieces[i].GetComponent<SpriteRenderer>().enabled = false;
                    gameManager.whitePieces[i].GetComponent<DragDrop>().enabled = false;
                    gameManager.whitePieces[i].GetComponent<Collider>().enabled = false;
                    gameManager.whitePieces.RemoveAt(i);
                    gameManager.ChangeTurn();
                    fenCalc.halfMoveNumber = 0;
                    fenCalc.UpdateFenCode();
                    return;
                }
            }
        }

        if (validMove)
        {
            gameManager.ChangeTurn();
            if (piece.tag == "Pawn")
            {
                fenCalc.halfMoveNumber = 0;
            }
            else
            {
                fenCalc.halfMoveNumber++;
            }
            fenCalc.UpdateFenCode();
        }
    }

    private void OnMouseDrag()
    {
        if (piece.isWhite != gameManager.isWhitesMove)
        {
            return;
        }
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;
    }
    private void OnMouseUp()
    {
        if (piece.isWhite != gameManager.isWhitesMove)
        {
            return;
        }
        SwitchTransparency();
        SnapToClosestSquare();
        CheckDragIsOutsideBoard();
        CheckIfMoveIsValid();
        CheckIfCastling();
        ClearEnPassantPrivilages();
        CheckForEnPassant();
        EnpassantTake();
        CheckForPawnPromotion();
        RevertSelectSquareColor();

        gameManager.possibleMoves.Clear();
        gameManager.possibleAttacks.Clear();

        if (piece.isWhite)
        {
            for (int i = 0; i < gameManager.blackPieces.Count; i++)
            {
                if (piece.transform.localPosition == gameManager.blackPieces[i].transform.localPosition)
                {
                    gameManager.blackPieces[i].GetComponent<SpriteRenderer>().enabled = false;
                    gameManager.blackPieces[i].GetComponent<DragDrop>().enabled = false;
                    gameManager.blackPieces[i].GetComponent<Collider>().enabled = false;
                    gameManager.blackPieces.RemoveAt(i);
                    gameManager.ChangeTurn();
                    fenCalc.halfMoveNumber = 0;
                    fenCalc.UpdateFenCode();
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < gameManager.whitePieces.Count; i++)
            {
                if (piece.transform.localPosition == gameManager.whitePieces[i].transform.localPosition)
                {
                    gameManager.whitePieces[i].GetComponent<SpriteRenderer>().enabled = false;
                    gameManager.whitePieces[i].GetComponent<DragDrop>().enabled = false;
                    gameManager.whitePieces[i].GetComponent<Collider>().enabled = false;
                    gameManager.whitePieces.RemoveAt(i);
                    gameManager.ChangeTurn();
                    fenCalc.halfMoveNumber = 0;
                    fenCalc.UpdateFenCode();
                    return;
                }
            }
        }

        if (validMove)
        {
            gameManager.ChangeTurn();
            if (piece.tag == "Pawn")
            {
                fenCalc.halfMoveNumber = 0;
            }
            else
            {
                fenCalc.halfMoveNumber++;
            }
            fenCalc.UpdateFenCode();
        }
    }

    public void RevertSelectSquareColor()
    {
        for (int i = 0; i < gameManager.availSquares.Count; i++)
        {
            gameManager.availSquares[i].GetComponent<BoardSquare>().ResetColor();
        }

        gameManager.availSquares.Clear();
    }

    public void EnpassantTake()
    {
        if (gameObject.tag != "Pawn")
        {
            return;
        }

        if (pieceBehaviour.enPassantVictim == null)
        {
            return;
        }

        if (Mathf.Abs(startPos.x - transform.localPosition.x) >= 0.8f && Mathf.Abs(startPos.y -  transform.localPosition.y) >= 0.8f)
        {
            pieceBehaviour.enPassantVictim.GetComponent<SpriteRenderer>().enabled = false;
            pieceBehaviour.enPassantVictim.GetComponent<DragDrop>().enabled = false;
            pieceBehaviour.enPassantVictim.GetComponent<Collider>().enabled = false;
            Debug.Log("En passant");
            piece.pieceBehaviour.enPassantVictim = null;
        }
    }

    void SnapToClosestSquare()
    {
        Vector2 position = transform.localPosition;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        transform.localPosition = position;
    }

    public void CheckForPawnPromotion()
    {
        if (!CompareTag("Pawn"))
        {
            return;
        }

        if (transform.localPosition.y == -3 || transform.localPosition.y == 4)
        {
            pieceBehaviour.PromotePawn();
        }
    }

    void CheckIfCastling()
    {
        if (tag == "King")
        {
            myCollider.gameObject.SetActive(false);

            if (Mathf.Abs(transform.localPosition.x - startPos.x) > 1.5f)
            {
                if (transform.localPosition.x - startPos.x > 0)
                {
                    Debug.Log("I am castling to the right");
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, Vector3.right, out hit, 5f)) {
                        if (hit.collider.gameObject.tag == "Rook")
                        {
                            Vector3 moveRook = hit.collider.transform.localPosition - new Vector3(2, 0, 0);
                            hit.transform.localPosition = moveRook;
                        }
                    }
                }
                else
                {
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, Vector3.left, out hit, 5f))
                    {
                        if (hit.collider.gameObject.tag == "Rook")
                        {
                            Vector3 moveRook = hit.collider.transform.localPosition + new Vector3(2, 0, 0);
                            hit.transform.localPosition = moveRook;
                            hit.collider.gameObject.GetComponent<ChessPiece>().hasMoved = true;
                        }
                    }
                }
            }
        }

        myCollider.gameObject.SetActive(true);
    }

    void CheckForEnPassant()
    {
        pieceBehaviour.enPassantVictim = null;
        pieceBehaviour.enPassantSquare = null;

        if (tag == "Pawn")
        {
            float yDifference = startPos.y - transform.localPosition.y;
            float xDifference = startPos.x - transform.localPosition.x;
            float wiggleRoom = 0.1f;

            if (Mathf.Abs(yDifference) > 1f && Mathf.Abs(xDifference) <= wiggleRoom && Mathf.Abs(xDifference) >= 0)
            {
                //en passant allowed
                pieceBehaviour.enPassantVictim = gameObject;
                float direction = yDifference / Mathf.Abs(yDifference);
                Vector3 enPassantRayOrigin = new Vector3(transform.position.x, transform.position.y + direction, transform.position.z);
                pieceBehaviour.enPassantVictim = gameObject;

                RaycastHit hit;
                if (Physics.Raycast(enPassantRayOrigin, Vector3.forward, out hit))
                {
                    fenCalc.enPassantCode = " " + hit.collider.gameObject.name + " ";
                    pieceBehaviour.enPassantSquare = hit.collider.gameObject;

                    myCollider.enabled = false;
                }

                RaycastHit leftHit;
                if (Physics.Raycast(transform.position, Vector3.left, out leftHit, 1.5f))
                {
                    if (leftHit.collider.gameObject.tag == "Pawn")
                    {
                        leftHit.collider.gameObject.GetComponent<ChessPiece>().canEnPassant = true;
                        pieceBehaviour.enPassantPawns.Add(leftHit.collider.gameObject);
                    }
                }

                RaycastHit rightHit;
                if (Physics.Raycast(transform.position, Vector3.right, out rightHit, 1.5f))
                {
                    if (rightHit.collider.gameObject.tag == "Pawn")
                    {
                        rightHit.collider.gameObject.GetComponent<ChessPiece>().canEnPassant = true;
                        pieceBehaviour.enPassantPawns.Add(rightHit.collider.gameObject);
                    }
                }
            }
            else
            {
                fenCalc.enPassantCode = " - ";
            }
        }
        else
        {
            fenCalc.enPassantCode = " - ";
        }

        myCollider.enabled = true;

    }

    void ClearEnPassantPrivilages()
    {
        for (int i = 0; i < pieceBehaviour.enPassantPawns.Count; i++)
        {
            pieceBehaviour.enPassantPawns[i].GetComponent<ChessPiece>().canEnPassant = false;
        }
    }

    void SwitchTransparency()
    {
        if (!transparent)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.75f);
            transparent = true;
        }
        else
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            transparent = false;
        }
    }

    void CheckDragIsOutsideBoard()
    {
        if (transform.localPosition.x < boardMinRange || transform.localPosition.x > boardMaxRange)
        {
            transform.localPosition = startPos;
        }

        if (transform.localPosition.y < boardMinRange || transform.localPosition.y > boardMaxRange)
        {
            transform.localPosition = startPos;
        }
    }

    void CheckIfMoveIsValid()
    {
        validMove = false;

        Vector2 currentPos = transform.localPosition;

        CheckForCheck();

        if (!piece.myKing.isInCheck)
        {
            for (int i = 0; i < gameManager.possibleMoves.Count; i++)
            {
                if (currentPos - startPos == gameManager.possibleMoves[i])
                {
                    piece.hasMoved = true;
                    validMove = true;
                    return;
                }
            }
        }

        transform.localPosition = startPos;
    }

    void CheckForCheck()
    {
        GameObject enemyKing;
        GameObject yourKing;

        Vector3 upright = Vector3.up + Vector3.right;
        Vector3 downright = Vector3.down + Vector3.right;
        Vector3 upleft = Vector3.up + Vector3.left;
        Vector3 downleft = Vector3.down + Vector3.left;

        if (piece.isWhite)
        {
            enemyKing = gameManager.blackKing;
            yourKing = gameManager.whiteKing;

            enemyKing.GetComponent<King>().isInCheck = false;
            yourKing.GetComponent<King>().isInCheck = false;

            LookForPawns(yourKing, upleft, 1.5f);
            LookForPawns(yourKing, upright, 1.5f);
            LookForPawns(enemyKing, downleft, 1.5f);
            LookForPawns(enemyKing, downleft, 1.5f);

        }
        else
        {
            enemyKing = gameManager.whiteKing;
            yourKing = gameManager.blackKing;

            enemyKing.GetComponent<King>().isInCheck = false;
            yourKing.GetComponent<King>().isInCheck = false;

            LookForPawns(enemyKing, upleft, 1.5f);
            LookForPawns(enemyKing, upright, 1.5f);
            LookForPawns(yourKing, downleft, 1.5f);
            LookForPawns(yourKing, downleft, 1.5f);
        }



        LookForLRUD(enemyKing, Vector3.left, 10f);
        LookForLRUD(enemyKing, Vector3.right, 10f);
        LookForLRUD(enemyKing, Vector3.up, 10f);
        LookForLRUD(enemyKing, Vector3.down, 10f);

        LookForDiagonal(enemyKing, upright, 10f);
        LookForDiagonal(enemyKing, downright, 10f);
        LookForDiagonal(enemyKing, upleft, 10f);
        LookForDiagonal(enemyKing, downleft, 10f);

        LookForKnights(enemyKing);

        LookForLRUD(yourKing, Vector3.left, 10f);
        LookForLRUD(yourKing, Vector3.right, 10f);
        LookForLRUD(yourKing, Vector3.up, 10f);
        LookForLRUD(yourKing, Vector3.down, 10f);

        LookForDiagonal(yourKing, upright, 10f);
        LookForDiagonal(yourKing, downright, 10f);
        LookForDiagonal(yourKing, upleft, 10f);
        LookForDiagonal(yourKing, downleft, 10f);

        LookForOtherKing(yourKing, Vector3.up, 1.5f);
        LookForOtherKing(yourKing, Vector3.down, 1.5f);
        LookForOtherKing(yourKing, Vector3.left, 1.5f);
        LookForOtherKing(yourKing, Vector3.right, 1.5f);
        LookForOtherKing(yourKing, upleft, 1.5f);
        LookForOtherKing(yourKing, upright, 1.5f);
        LookForOtherKing(yourKing, downleft, 1.5f);
        LookForOtherKing(yourKing, downright, 1.5f);

        LookForOtherKing(enemyKing, Vector3.up, 1.5f);
        LookForOtherKing(enemyKing, Vector3.down, 1.5f);
        LookForOtherKing(enemyKing, Vector3.left, 1.5f);
        LookForOtherKing(enemyKing, Vector3.right, 1.5f);
        LookForOtherKing(enemyKing, upleft, 1.5f);
        LookForOtherKing(enemyKing, upright, 1.5f);
        LookForOtherKing(enemyKing, downleft, 1.5f);
        LookForOtherKing(enemyKing, downright, 1.5f);

        LookForKnights(yourKing);
    }

    void LookForPawns(GameObject king, Vector3 direction, float rayDistance)
    {
        Ray ray = new Ray(king.transform.position, direction);
        RaycastHit hit;

        king.GetComponent<Collider>().enabled = false;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                if (hit.collider.gameObject.GetComponent<ChessPiece>().isWhite != king.gameObject.GetComponent<ChessPiece>().isWhite)
                {
                    if (hit.collider.gameObject.tag == "Pawn")
                    {
                        Debug.Log(king.name + " is in check");
                        king.GetComponent<King>().isInCheck = true;
                        king.GetComponent<Collider>().enabled = true;
                        return;
                    }
                }
            }

        }

        king.GetComponent<Collider>().enabled = true;
    }

    void LookForLRUD(GameObject king, Vector3 direction, float rayDistance)
    {
        Ray ray = new Ray(king.transform.position, direction);
        RaycastHit hit;

        king.GetComponent<Collider>().enabled = false;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                if (hit.collider.gameObject.GetComponent<ChessPiece>().isWhite != king.gameObject.GetComponent<ChessPiece>().isWhite)
                {
                    if (hit.collider.gameObject.tag == "Queen" || hit.collider.gameObject.tag == "Rook")
                    {
                        //Debug.Log(king.name + " is in check");
                        king.GetComponent<King>().isInCheck = true;
                        king.GetComponent<Collider>().enabled = true;
                        return;
                    }
                }
            }

        }
        king.GetComponent<Collider>().enabled = true;
    }

    void LookForOtherKing(GameObject king, Vector3 direction, float rayDistance)
    {
        Ray ray = new Ray(king.transform.position, direction);
        RaycastHit hit;

        king.GetComponent<Collider>().enabled = false;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                if (hit.collider.gameObject.GetComponent<ChessPiece>().isWhite != king.gameObject.GetComponent<ChessPiece>().isWhite)
                {
                    if (hit.collider.gameObject.tag == "King")
                    {
                        //Debug.Log(king.name + " is in check");
                        king.GetComponent<King>().isInCheck = true;
                        king.GetComponent<Collider>().enabled = true;
                        return;
                    }
                }
            }

        }
        king.GetComponent<Collider>().enabled = true;
    }

    void LookForDiagonal(GameObject king, Vector3 direction, float rayDistance)
    {
        Ray ray = new Ray(king.transform.position, direction);
        RaycastHit hit;

        king.GetComponent<Collider>().enabled = false;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                if (hit.collider.gameObject.GetComponent<ChessPiece>().isWhite != king.gameObject.GetComponent<ChessPiece>().isWhite)
                {
                    if (hit.collider.gameObject.tag == "Queen" || hit.collider.gameObject.tag == "Bishop")
                    {
                        //Debug.Log(king.name + " is in check");
                        king.GetComponent<King>().isInCheck = true;
                        king.GetComponent<Collider>().enabled = true;
                        return;
                    }
                }
            }

        }
        king.GetComponent<Collider>().enabled = true;
    }

    void LookForKnights(GameObject king)
    {
        Vector2[] possibleEnemyKnightLocations = new Vector2[]
        {
        new Vector2(2, 1),
        new Vector2(2, -1),
        new Vector2(-2, 1),
        new Vector2(-2, -1),
        new Vector2(1, 2),
        new Vector2(1, -2),
        new Vector2(-1, 2),
        new Vector2(-1, -2)
        };

        foreach (Vector2 move in possibleEnemyKnightLocations)
        {
            Vector3 testPos = king.transform.position + new Vector3(move.x, move.y, -10f);
            RaycastHit hit;

            if (Physics.Raycast(testPos, Vector3.forward, out hit, 30f))
            {
                if (hit.collider.gameObject.tag == "Knight")
                {
                    if (hit.collider.GetComponent<ChessPiece>().isWhite != king.gameObject.GetComponent<ChessPiece>().isWhite) 
                    {
                        //Debug.Log(king.name + " is in check");
                        king.GetComponent<King>().isInCheck = true;
                        king.GetComponent<Collider>().enabled = true;
                        return;
                    }
                }
            }
        }
    }
}



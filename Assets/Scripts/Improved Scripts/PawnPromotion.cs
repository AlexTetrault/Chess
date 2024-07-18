using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPromotion : MonoBehaviour
{
    public FenCalc fenCal;

    public GameObject promotingPawn;
    public Sprite rookSpriteW;
    public Sprite queenSpriteW;
    public Sprite bishopSpriteW;
    public Sprite knightSpriteW;
    public Sprite rookSpriteB;
    public Sprite queenSpriteB;
    public Sprite bishopSpriteB;
    public Sprite knightSpriteB;
    public Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public void RookPromotion()
    {
        if (promotingPawn.GetComponent<ChessPiece>().isWhite)
        {
            promotingPawn.GetComponent<SpriteRenderer>().sprite = rookSpriteW;
            promotingPawn.tag = "Rook";
            promotingPawn.name = "R";
        }
        else
        {
            promotingPawn.GetComponent<SpriteRenderer>().sprite = rookSpriteB;
            promotingPawn.tag = "Rook";
            promotingPawn.name = "r";
        }

        fenCal.UpdateFenCode();
        canvas.enabled = false;
    }

    public void QueenPromotion()
    {
        if (promotingPawn.GetComponent<ChessPiece>().isWhite)
        {
            promotingPawn.GetComponent<SpriteRenderer>().sprite = queenSpriteW;
            promotingPawn.tag = "Queen";
            promotingPawn.name = "Q";
        }
        else
        {
            promotingPawn.GetComponent<SpriteRenderer>().sprite = queenSpriteB;
            promotingPawn.tag = "Queen";
            promotingPawn.name = "q";
            canvas.enabled = false;
        }

        fenCal.UpdateFenCode();
        canvas.enabled = false;
    }
    public void BishopPromotion()
    {
        if (promotingPawn.GetComponent<ChessPiece>().isWhite)
        {
            promotingPawn.GetComponent<SpriteRenderer>().sprite = bishopSpriteW;
            promotingPawn.tag = "Bishop";
            promotingPawn.name = "B";
        }
        else
        {
            promotingPawn.GetComponent<SpriteRenderer>().sprite = bishopSpriteB;
            promotingPawn.tag = "Bishop";
            promotingPawn.name = "b";
            canvas.enabled = false;
        }

        fenCal.UpdateFenCode();
        canvas.enabled = false;
    }
    public void KnightPromotion()
    {
        if (promotingPawn.GetComponent<ChessPiece>().isWhite)
        {
            promotingPawn.GetComponent<SpriteRenderer>().sprite = knightSpriteW;
            promotingPawn.tag = "Knight";
            promotingPawn.name = "N";
        }
        else
        {
            promotingPawn.GetComponent<SpriteRenderer>().sprite = knightSpriteB;
            promotingPawn.tag = "Knight";
            promotingPawn.name = "n";
        }

        fenCal.UpdateFenCode();
        canvas.enabled = false;
    }
}

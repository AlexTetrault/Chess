using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineChessBoard : MonoBehaviour
{
    public OnlineFenCalculator fenCalculator;

    public GameObject[,] boardGrid = new GameObject[8, 8];
    public List<GameObject> chessPieces;

    public void UpdateBoardGrid()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                boardGrid[x, y] = null;
            }
        }

        foreach (GameObject chessPiece in chessPieces)
        {
            Vector3 position = chessPiece.transform.localPosition;
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);

            boardGrid[x, y] = chessPiece;
        }
    }
}

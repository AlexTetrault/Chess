using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public List<GameObject> blackPieces = new List<GameObject>();
    public List<GameObject> whitePieces = new List<GameObject>();

    public bool isWhitesMove = true;

    public List<Vector2> possibleMoves = new List<Vector2>();
    public List<GameObject> possibleAttacks = new List<GameObject>();

    public List<Vector2> whiteControlledSpaces = new List<Vector2>();
    public List<Vector2> blackControlledSpaces = new List<Vector2>();

    public List<GameObject> availSquares;

    public GameObject blackKing;
    public GameObject whiteKing;

    int layerMask;

    private void Start()
    {
        layerMask = LayerMask.GetMask("Square");
    }

    public void ChangeTurn()
    {
        if (isWhitesMove)
        {
            isWhitesMove=false;
        }
        else
        {
            isWhitesMove = true;
        }
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
}



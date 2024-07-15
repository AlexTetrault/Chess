using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    Color squareColor;

    // Start is called before the first frame update
    void Start()
    {
        squareColor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = squareColor;
    }
}

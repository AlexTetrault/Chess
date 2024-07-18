using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    Color squareColor;

    // Start is called before the first frame update
    void Start()
    {
        squareColor = GetComponent<SpriteRenderer>().color;
    }

    public void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = squareColor;
    }
}

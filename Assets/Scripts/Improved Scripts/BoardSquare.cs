using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    Color squareColor;
    SpriteRenderer spriteRenderer;
    Color highLightColor;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        squareColor = spriteRenderer.color;

        float blueShiftAmount = 0.2f;
        highLightColor = new Color(
            Mathf.Clamp01(squareColor.r - blueShiftAmount),
            Mathf.Clamp01(squareColor.g - blueShiftAmount),
            Mathf.Clamp01(squareColor.b + blueShiftAmount));
    }

    public void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = squareColor;
    }

    public void LegalMoveColor()
    {
        spriteRenderer.color = highLightColor;
    }
}

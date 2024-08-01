using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOptions : MonoBehaviour
{
    public bool isPlayingWhite;

    public int botDifficulty = 1;
    public int botDepth = 1;

    public Camera mainCamera;
    public GameObject[] pieces;

    public TMP_Dropdown dropDown;
    public TMP_Dropdown colourDropDown;
    public Canvas optionCanvas;

    public FenCalculator fenCalculator;

    Vector3 upSideDown;
    Vector3 rightSideUp;

    public void SetDifficulty(string difficulty)
    {
        if (difficulty == "B")
        {
            botDifficulty = 1;
            botDepth = 1;
        }

        if (difficulty == "N")
        {
            botDifficulty = 2;
            botDepth = 3;
        }

        if (difficulty == "I")
        {
            botDifficulty = 4;
            botDepth = 6;
        }

        if (difficulty == "A")
        {
            botDifficulty = 6;
            botDepth = 10;
        }

        if (difficulty == "P")
        {
            botDifficulty = 8;
            botDepth = 14;
        }

        if (difficulty == "G")
        {
            botDifficulty = 10;
            botDepth = 20;
        }
    }

    private void Start()
    {
        SetDifficulty("B");
        isPlayingWhite = true;

        upSideDown = new Vector3(0, 0, 180);
        rightSideUp = new Vector3(0, 0, 0);
    }

    public void difficultyOption()
    {
        int index = dropDown.value;

        switch (index) 
        {
            case 0: SetDifficulty("B"); break;
            case 1: SetDifficulty("N"); break;
            case 2: SetDifficulty("I"); break;
            case 3: SetDifficulty("A"); break;
            case 4: SetDifficulty("P"); break;
            case 5: SetDifficulty("G"); break;
        }

    }

    public void chooseColour()
    {
        int index = colourDropDown.value;

        switch (index)
        {
            case 0: isPlayingWhite = true; RotateBoardToWhite(); break;
            case 1: isPlayingWhite = false; RotateBoardToBlack(); break;
        }
    }

    public void StartGame()
    {
        fenCalculator.UpdateFenCode();
        optionCanvas.enabled = false;
    }

    private void RotateBoardToBlack()
    {
        mainCamera.transform.eulerAngles = upSideDown;

        foreach (GameObject piece in pieces)
        {
            piece.transform.eulerAngles = upSideDown;
        }
    }

    private void RotateBoardToWhite()
    {
        mainCamera.transform.eulerAngles = rightSideUp;

        foreach (GameObject piece in pieces)
        {
            piece.transform.eulerAngles = rightSideUp;
        }
    }
}

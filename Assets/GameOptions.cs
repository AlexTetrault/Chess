using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOptions : MonoBehaviour
{
    public bool isPlayingWhite;

    public int botDifficulty = 1;
    public int botDepth = 1;
    public int moveTime = 100;

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
            botDifficulty = 0;
            botDepth = 1;
            moveTime = 50;
        }

        if (difficulty == "N")
        {
            botDifficulty = 1;
            botDepth = 1;
            moveTime = 75;
        }

        if (difficulty == "I")
        {
            botDifficulty = 3;
            botDepth = 2;
            moveTime = 100;
        }

        if (difficulty == "A")
        {
            botDifficulty = 5;
            botDepth = 4;
            moveTime = 250;
        }

        if (difficulty == "P")
        {
            botDifficulty = 8;
            botDepth = 6;
            moveTime = 500;
        }

        if (difficulty == "G")
        {
            botDifficulty = 10;
            botDepth = 8;
            moveTime = 1200;
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

    public void ReloadScene()
    {
        // Get the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

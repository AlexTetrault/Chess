using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOptions : MonoBehaviour
{
    public bool isPlayingWhite;

    public int botDifficulty = 1;
    public int botDepth = 1;

    public int randomMoveChance = 9;

    public Camera mainCamera;
    public GameObject[] pieces;

    public TMP_Dropdown dropDown;
    public TMP_Dropdown colourDropDown;
    public Canvas optionCanvas;

    public FenCalculator fenCalculator;
    public GameManager gameManager;

    public Toggle showMovesToggle;
    public bool showingMoves;

    public bool isPlayingInfectChess;

    Vector3 upSideDown;
    Vector3 rightSideUp;

    public void SetDifficulty(string difficulty)
    {
        if (difficulty == "B")
        {
            botDifficulty = 0;
            botDepth = 6;
            randomMoveChance = 8;
        }

        if (difficulty == "N")
        {
            botDifficulty = 1;
            botDepth = 6;
            randomMoveChance = 7;
        }

        if (difficulty == "I")
        {
            botDifficulty = 3;
            botDepth = 6;
            randomMoveChance = 5;
        }

        if (difficulty == "A")
        {
            botDifficulty = 5;
            botDepth = 6;
            randomMoveChance = 3;
        }

        if (difficulty == "P")
        {
            botDifficulty = 8;
            botDepth = 8;
            randomMoveChance = 1;
        }

        if (difficulty == "G")
        {
            botDifficulty = 10;
            botDepth = 10;
            randomMoveChance = 0;
        }
    }

    private void Start()
    {
        SetDifficulty("B");
        isPlayingWhite = true;

        isPlayingInfectChess = true;

        showingMoves = showMovesToggle.isOn;

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
            case 0: isPlayingWhite = true; RotateBoardToWhite(); gameManager.isPlayersMove = true; break;
            case 1: isPlayingWhite = false; RotateBoardToBlack(); gameManager.isPlayersMove = false; break;
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

    public void ShowMovesToggle()
    {
        showingMoves = showMovesToggle.isOn ? true : false;
    }

    public void ReloadScene()
    {
        // Get the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);
    }

    public void BackButton(Canvas canvas)
    {
        canvas.enabled = false;
    }

    public void SelectButton(Canvas canvas)
    {
        canvas.enabled = true;
    }

    public void InfectChessOption(bool playingInfectChess)
    {
        if (playingInfectChess == false)
        {
            isPlayingInfectChess = false;
        }
        else
        {
            isPlayingInfectChess = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

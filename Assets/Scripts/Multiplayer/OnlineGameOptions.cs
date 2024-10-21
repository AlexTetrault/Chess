using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class OnlineGameOptions : MonoBehaviour
{
    public OnlineFenCalculator fenCalculator;
    public OnlineGameManager gameManager;

    public bool isInfect;
    public Sprite whitePawn, whiteRook, whiteKnight, whiteBishop, whiteQueen;
    public Sprite blackPawn, blackRook, blackKnight, blackBishop, blackQueen;

    public TMP_Dropdown gameModeDropdown;

    public void StartGame()
    {
        fenCalculator.UpdateFenCode();
        gameManager.photonView.RPC("CloseClientWaitingScreen", RpcTarget.Others);

        if (isInfect)
        {
            gameManager.photonView.RPC("SetGameToInfect", RpcTarget.All);
        }

        gameManager.masterWaitingScreen.SetActive(false);
    }

    public void SelectGameMode()
    {
        int index = gameModeDropdown.value;

        switch (index)
        {
            case 0: isInfect = false; break;
            case 1: isInfect = true; break;
        }
    }
}

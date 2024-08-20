using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class OnlineGameOptions : MonoBehaviour
{
    public OnlineFenCalculator fenCalculator;
    public OnlineGameManager gameManager;

    public void StartGame()
    {
        fenCalculator.UpdateFenCode();
        gameManager.photonView.RPC("CloseClientWaitingScreen", RpcTarget.Others);
        gameManager.masterWaitingScreen.SetActive(false);
    }
}

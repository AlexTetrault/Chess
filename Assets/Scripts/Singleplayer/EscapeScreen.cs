using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class EscapeScreen : MonoBehaviourPunCallbacks
{
    Canvas canvas;
    public Canvas settingsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public void CloseCanvas()
    {
        canvas.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.enabled = !canvas.enabled;
        }
    }

    public void LoadTitleScreen()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CloseWindow()
    {
        canvas.enabled = false;
    }

    public void OpenSettings()
    {
        settingsCanvas.enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnlineCamera : MonoBehaviour
{
    public GameObject[] pieces;

    Vector3 upSideDown = new Vector3(0, 0, 180);

    // Start is called before the first frame update
    void Start()
    {
        //if joining a game, you play as black
        if (!PhotonNetwork.IsMasterClient)
        {
            RotateBoardToBlack();
        }
    }

    private void RotateBoardToBlack()
    {
        transform.eulerAngles = upSideDown;

        foreach (GameObject piece in pieces)
        {
            piece.transform.eulerAngles = upSideDown;
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public Text messageAreaText { get; set; } // LobbyController
 
    private string roomName;
    void Start()
    {
        roomName = transform.GetChild(0).GetComponent<Text>().text;
        GetComponent<Button>().onClick.AddListener(JoinRoom);
    }

    private void JoinRoom()
    {
        LobbyController.instance.ButtonJoinRoom(roomName);
    }

}

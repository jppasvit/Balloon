using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomButton : MonoBehaviourPunCallbacks
{
    public Text messageAreaText { get; set; } // LobbyController
    public int multiplayerRoomSceneIndex { get; set; } // LobbyController

    private string roomName;
    void Start()
    {
        roomName = transform.GetChild(0).GetComponent<Text>().text;
        GetComponent<Button>().onClick.AddListener(JoinRoom);
    }

    private void JoinRoom()
    {
        Debug.Log("Join roomName button: " + roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed on join to room: " + roomName + " Message: " + message);
        messageAreaText.text = "Failed on join to room" + roomName + ". Try again.";
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + roomName);
        SceneManager.LoadScene(multiplayerRoomSceneIndex);
    }

}

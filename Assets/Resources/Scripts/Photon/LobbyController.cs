using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Button createRoomButton;

    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Text messageArea;

    [SerializeField]
    private int RoomSize = 2;

    [SerializeField]
    private int multiplayerRoomSceneIndex = 1;

    [SerializeField]
    private GameObject roomButtonPrefab;
    [SerializeField]
    private GameObject placeOfRooms;

    private static List<RoomInfo> roomInfos = new List<RoomInfo>();

    void Awake()
    {
        if (createRoomButton != null )
        {
            createRoomButton.onClick.AddListener(CreateRoom);
        }

    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinLobby();
        Debug.Log("JoinLobby.");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        roomInfos.Clear();
        foreach ( RoomInfo info in roomList)
        {
            if (!info.RemovedFromList)
            {
                roomInfos.Add(info);
                Debug.Log("Room Name Added: " + info.Name);
            }
            Debug.Log("Room Name: " + info.Name);

        }
        UpdateMenuRoomList();
        Debug.Log("Into OnRoomListUpdate");
        Debug.Log("Rooms: " + roomList.Count);
    }

    private void UpdateMenuRoomList() {
        int children = placeOfRooms.transform.childCount;
        int rooms = roomInfos.Count;

        int i = 0;
        foreach (RoomInfo info in roomInfos)
        {
            if ( i <= children && children > 0)
            {
                GameObject roomButton = placeOfRooms.transform.GetChild(i).gameObject;
                SetRoomButtonAttributes(ref roomButton, info);
                roomButton.gameObject.SetActive(true);
            }
            else
            {
                GameObject roomButton = Instantiate(roomButtonPrefab, placeOfRooms.transform);
                SetRoomButtonAttributes(ref roomButton, info);
            }
            i++;
        }

        if (children > rooms)
        {
            for (int j = i; j < children; j++)
            {
                placeOfRooms.transform.GetChild(j).gameObject.SetActive(false);
            }
        }
    }

    private void SetRoomButtonAttributes(ref GameObject roomButton, RoomInfo info)
    {
        roomButton.transform.GetChild(0).GetComponent<Text>().text = info.Name;
        RoomButton roomButtonComp = roomButton.GetComponent<RoomButton>();
        roomButtonComp.multiplayerRoomSceneIndex = multiplayerRoomSceneIndex;
        roomButtonComp.messageAreaText = messageArea;
    }

    private bool IsRoomOnRoomList(string roomName)
    {
        foreach ( RoomInfo info in roomInfos)
        {
            if ( roomName.Equals(info.Name) )
            {
                return true;
            }
        }

        return false;
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize };
        if (string.IsNullOrEmpty(inputField.text) || string.IsNullOrWhiteSpace(inputField.text))
        {
            messageArea.text = "Room name is not valid.";
        }
        else if ( IsRoomOnRoomList(inputField.text) )
        {
            messageArea.text = "Room name already exists.";
        }
        else
        {
            Debug.Log("Creating room with name: " + inputField.text);
            messageArea.text = "";
            PhotonNetwork.CreateRoom(inputField.text, roomOptions);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed on join to room: " + message);
        messageArea.text = "Failed on join to room. Try again.";
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Falied to create room: " + message);
        messageArea.text = "Falied to create room. Try again.";
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Join Room");
        PhotonNetwork.JoinRoom(inputField.text);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        SceneManager.LoadScene(multiplayerRoomSceneIndex);
    }

}

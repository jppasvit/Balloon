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
    private GameObject createRoomButton;
    private Button _createRoomButton;

    [SerializeField]
    private GameObject inputField;
    private InputField _inputField;

    [SerializeField]
    private GameObject messageArea;
    private Text _messageArea;

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
        if ( createRoomButton != null )
        {
            _createRoomButton = createRoomButton.GetComponent<Button>();
            _createRoomButton.onClick.AddListener(CreateRoom);
        }

        if (inputField != null )
        {
            _inputField = inputField.GetComponent<InputField>();
        }

        if (messageArea != null)
        {
            _messageArea = messageArea.GetComponent<Text>();
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
        roomButtonComp.messageAreaText = _messageArea;
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
        if (string.IsNullOrEmpty(_inputField.text) || string.IsNullOrWhiteSpace(_inputField.text))
        {
            _messageArea.text = "Room name is not valid.";
        }
        else if ( IsRoomOnRoomList(_inputField.text) )
        {
            _messageArea.text = "Room name already exists.";
        }
        else
        {
            Debug.Log("Creating room with name: " + _inputField.text);
            _messageArea.text = "";
            PhotonNetwork.CreateRoom(_inputField.text, roomOptions);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed on join to room: " + message);
        _messageArea.text = "Failed on join to room. Try again.";
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Falied to create room: " + message);
        _messageArea.text = "Falied to create room. Try again.";
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Join Room");
        PhotonNetwork.JoinRoom(_inputField.text);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        SceneManager.LoadScene(multiplayerRoomSceneIndex);
    }

}

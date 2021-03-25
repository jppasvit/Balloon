using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonSynchronizer : MonoBehaviour
{
    public static BalloonSynchronizer instance { get; private set; }

    public int balloonViewId = 0;
    public bool itsBalloonIsInstantiated = false;
    public string cloudAnchorId;
    private PhotonView photonView;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public GameObject InstantiateAndAllocateViewId(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        GameObject balloon = Instantiate(gameObject, position, rotation);
        PhotonView balloonPhotonView = balloon.GetComponent<PhotonView>();
        Debug.LogAssertion("INTO InstantiateAndAllocateViewId");
        if (balloonViewId != 0)
        {
            Debug.LogAssertion("Set ViewID MANUALLY");
            balloonPhotonView.ViewID = balloonViewId;
        }
        else
        {
            if (PhotonNetwork.AllocateViewID(balloonPhotonView))
            {
                Debug.LogAssertion("AllocateViewID <-");
                photonView.RPC("RPC_SetBalloonViewId", RpcTarget.AllBuffered, balloonPhotonView.ViewID);
            }
            else
            {
                Destroy(balloon);
                return null;
            }
        }
        photonView.RPC("RPC_NotifyBalloonIsInstantiated", RpcTarget.OthersBuffered, true);

        return balloon;
    }

    [PunRPC]
    public void RPC_NotifyBalloonIsInstantiated(bool value)
    {
        Debug.LogAssertion("RPC_NotifyBalloonIsInstantiated <-");
        itsBalloonIsInstantiated = value;
    }

    [PunRPC]
    public void RPC_SetBalloonViewId(int id)
    {
        Debug.LogAssertion("RPC_SetBalloonViewId <-");
        balloonViewId = id;
    }

}
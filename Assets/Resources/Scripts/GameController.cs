using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }
    public GameObject balloon { get; private set; }

    // Game flow
    private bool startGame = true;
    private bool myTurn = false;

    // PhotonView
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

    void Update()
    {
        if (balloon == null)
        {
            FindBalloon();
        }

        // Game flow
        if ( BalloonSynchronizer.instance.AreBallonsSynchronized() )
        {
            if (startGame)
            {
                MessageArea.instance.SuccessMessage("GAME STARTS!!!");
                balloon.GetPhotonView().RPC("RPC_Gravity", RpcTarget.All);
                startGame = false;
            }

            if ( myTurn )
            {
                MessageArea.instance.SuccessMessage("IS YOUR TURN\nTap the balloon.");
            }
            else
            {
                MessageArea.instance.ErrorMessage("IS NOT YOUR TURN\nWait your turn.");
            }
        }
    }

    private void FindBalloon ()
    {
        if ( balloon == null ) { 
            GameObject[] balloons = GameObject.FindGameObjectsWithTag("Balloon");
            if ( balloons.Length > 0 )
            {
                balloon = balloons[0]; // Only one balloon
            }
        }
    }

    public PhotonView GetBalloonPhotonView()
    {
        if ( balloon != null )
        {
            return balloon.GetPhotonView();
        }
        return null;
    }

    public bool IsBalloonTouched(Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit hitObject;

                if (Physics.SphereCast(ray, (float)0.1, out hitObject))
                {
                    var balloonMovement = hitObject.transform.GetComponent<BalloonMovement>();

                    if (balloonMovement != null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Give me first turn
    public void TakeFirstTurn()
    {
        myTurn = true;
        photonView.RPC("RPC_TakeTurn", RpcTarget.Others, false);
    }

    [PunRPC]
    public void RPC_TakeTurn(bool value)
    {
        myTurn = value;
    }

    public void TapOnBalloon()
    {
        if (myTurn)
        {
            balloon.GetPhotonView().RPC("RPC_TapOnBalloon", RpcTarget.All);
            myTurn = false;
            photonView.RPC("RPC_TakeTurn", RpcTarget.Others, true);
            
        }
    }

}

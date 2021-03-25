using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }
    public GameObject balloon { get; private set; }

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

    void Update()
    {
        FindBalloon();
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
}

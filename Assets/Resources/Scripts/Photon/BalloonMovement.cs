using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ConstantForce))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class BalloonMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private float gravityForce = 0.15F;
    [SerializeField]
    private float tapForce = 20;
    [SerializeField]
    private float upVector = 20;
    private Rigidbody rigidbody;
    private ConstantForce constantForce;
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        constantForce = GetComponent<ConstantForce>();
    }

    [PunRPC]
    public void RPC_SetActiveGravity(bool value)
    {
        if (value)
        {
            constantForce.force = new Vector3(0, -gravityForce, 0);
        }
        else
        {
            constantForce.force = new Vector3(0, 0, 0);
        }
        constantForce.enabled = value;
    }

    [PunRPC]
    public void RPC_TapOnBalloon()
    {
        rigidbody.AddForce(transform.up * tapForce);
    }

    [PunRPC]
    public void RPC_TapOnAndPushBalloon(Vector2 touchPosition)
    {
        Vector3 touchPositionAtScreen = Camera.main.ScreenToWorldPoint(touchPosition);
        Vector3 direction = (transform.up * upVector - (touchPositionAtScreen - rigidbody.transform.position) );
        rigidbody.AddForce(direction.normalized * tapForce);
    }

    [PunRPC]
    public void RPC_RaisePosition(float y)
    {
        transform.Translate(transform.up * y, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ARPlane plane = collision.gameObject.GetComponent<ARPlane>();
        if ( plane != null  )
        {
            if ( GameController.instance.planeClassificationEnabled )
            {
                if ( PlaneClassificationManager.instance.IsFloor(plane) )
                {
                    GameController.instance.GameOver(true);
                }
            }
            else
            {
                GameController.instance.GameOver(true);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.LogError("-> IsWriting <-");
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)
        {
            Debug.Log("-> IsReading <-");
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

}

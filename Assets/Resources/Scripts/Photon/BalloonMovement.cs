using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ConstantForce))]
[RequireComponent(typeof(Rigidbody))]
public class BalloonMovement : MonoBehaviour
{
    [SerializeField]
    private float gravityForce = 0.15F;
    [SerializeField]
    private float tapForce = 15;
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
    public void RPC_RaisePosition(float y)
    {
        transform.Translate(transform.up * y, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        ARPlane plane = collision.gameObject.GetComponent<ARPlane>();
        if ( plane != null  )
        {
            Debug.Log("OnCollisionEnter its a plane");
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

}

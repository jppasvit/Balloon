using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    [SerializeField]
    private float gravityForce = 2;
    private Rigidbody rigidbody;
    private PhotonView photonView;
    
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rigidbody = GetComponent<Rigidbody>();
        
    }

    [PunRPC]
    public void RPC_Gravity()
    {
        rigidbody.AddForce(transform.up * -gravityForce);
    }

    [PunRPC]
    public void RPC_TapOnBalloon()
    {
        rigidbody.AddForce(transform.up * gravityForce);
    }

}

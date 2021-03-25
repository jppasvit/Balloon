using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    [SerializeField]
    private float tapForce = 2;
    private Rigidbody rigidbody;
    private PhotonView photonView;
    
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rigidbody = GetComponent<Rigidbody>();
        
    }

    [PunRPC]
    public void RPC_TapOnBalloon()
    {
        rigidbody.AddForce(transform.up * tapForce);
    }

}

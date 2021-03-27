using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBalloon : MonoBehaviour
{
    [SerializeField]
    private float tapForce = 50;

    [SerializeField]
    private float balloonElevation = 4;

    private void Start()
    {
        transform.Translate(transform.up * balloonElevation);
    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            GetComponent<Rigidbody>().AddForce(transform.up * tapForce);
        }
    }
}

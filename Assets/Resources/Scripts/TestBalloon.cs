using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBalloon : MonoBehaviour
{
    [SerializeField]
    private float tapForce = 50;

    [SerializeField]
    private float balloonElevation = 4;

    private Rigidbody rigidbody;

    private void Start()
    {
        //transform.Translate(transform.up * balloonElevation);
        rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Vector3 touchPositionAtScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (transform.up * 10 - rigidbody.position - touchPositionAtScreen);
            rigidbody.AddForce(direction.normalized * tapForce);
        }
    }
}

using Google.XR.ARCoreExtensions.Samples.CloudAnchors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour
{

    private GameObject _balloonUpMesh;
    private GameObject _threadMesh;

    /// <summary>
    /// The Cloud Anchors example controller.
    /// </summary>
    private CloudAnchorsExampleController _cloudAnchorsExampleController;

    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake()
    {
        _cloudAnchorsExampleController =
            GameObject.Find("CloudAnchorsExampleController")
            .GetComponent<CloudAnchorsExampleController>();
        _balloonUpMesh = transform.Find("BalloonUp").gameObject;
        _threadMesh = transform.Find("Thread").gameObject;
        _balloonUpMesh.SetActive(false);
        _threadMesh.SetActive(false);
    }

    /// <summary>
    /// Set the parent for this star and activate it directly.
    /// </summary>
    public void SetParentToWorldOrigin()
    {
        transform.SetParent(_cloudAnchorsExampleController.WorldOrigin);
        _balloonUpMesh.SetActive(true);
        _threadMesh.SetActive(true);
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        if (_balloonUpMesh.activeSelf && _threadMesh.activeSelf)
        {
            return;
        }

        // Only sets the Star object's mesh after the origin is placed to avoid being placed
        // at identity pose.
        if (!_cloudAnchorsExampleController.IsOriginPlaced)
        {
            return;
        }

        transform.SetParent(_cloudAnchorsExampleController.WorldOrigin, false);
        Transform origin = _cloudAnchorsExampleController.WorldOrigin;
        _balloonUpMesh.SetActive(true);
        _threadMesh.SetActive(true);
    }
}


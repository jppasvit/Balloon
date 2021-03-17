using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ResolveButton : MonoBehaviour
{
    [SerializeField]
    private ARSessionOrigin arSessionOrigin;
    private CloudAnchorManager cloudAnchorManager;
    private MessageArea messageArea;

    void Awake()
    {
        cloudAnchorManager = arSessionOrigin.GetComponent<CloudAnchorManager>();
        messageArea = arSessionOrigin.GetComponent<MessageArea>();
    }

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(EmulateResolve);
    }

    private void EmulateResolve()
    {
        cloudAnchorManager.emulateResolve = true;
        gameObject.SetActive(false);
        messageArea.InfoMessage("Tap on a plane and a balloon will be instantiated.");
    }
}

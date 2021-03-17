using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public enum Emulation
{
    Host = 0,
    Resolve = 1
}
public class EmulateButton : MonoBehaviour
{
    [SerializeField]
    private ARSessionOrigin arSessionOrigin;
    private CloudAnchorManager cloudAnchorManager;
    private MessageArea messageArea;

    public Emulation emulation;

    void Awake()
    {
        cloudAnchorManager = arSessionOrigin.GetComponent<CloudAnchorManager>();
        messageArea = arSessionOrigin.GetComponent<MessageArea>();
    }
    void Start()
    {
       GetComponent<Button>().onClick.AddListener(Emulate);
    }

    private void Emulate()
    {
        if (emulation == Emulation.Host)
        {
            cloudAnchorManager.emulateHost = true;
        }
        else if (emulation == Emulation.Resolve)
        {
            cloudAnchorManager.emulateResolve = true;
        }
        gameObject.SetActive(false);
        messageArea.InfoMessage("Tap on a plane and a balloon will be instantiated.");
    }
}

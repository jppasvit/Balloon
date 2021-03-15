using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ClearButton : MonoBehaviour
{
    [SerializeField]
    private ARSessionOrigin arSessionOrigin;
    private CloudAnchorManager cloudAnchorManager;
    void Awake()
    {
        cloudAnchorManager = arSessionOrigin.GetComponent<CloudAnchorManager>();
    }
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Clear);
    }

    private void Clear()
    {
        cloudAnchorManager.Clear();
        gameObject.SetActive(false);
    }

}

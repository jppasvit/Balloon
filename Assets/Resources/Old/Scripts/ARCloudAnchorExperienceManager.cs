using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(ARPointCloudManager))]
public class ARCloudAnchorExperienceManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnInitialized = null;

    [SerializeField]
    private UnityEvent OnRetarted = null;

    private ARPlaneManager arPlaneManager = null;

    private ARPointCloudManager arPointCloudManager = null;

    private bool Initialized { get; set; }

    private bool AllowCloudAnchorDelay { get; set; }
    
    private float timePassedAfterPlanesDetected = 0;

    [SerializeField]
    private float maxScanningAreaTime = 30;

    void Awake() 
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPointCloudManager = GetComponent<ARPointCloudManager>();

        arPlaneManager.planesChanged += PlanesChanged;

        #if UNITY_EDITOR
            OnInitialized?.Invoke();
            Initialized = true;
            AllowCloudAnchorDelay = false;
            arPlaneManager.enabled = false;
            arPointCloudManager.enabled = false;
        #endif
    }


    void Update() 
    {
        if(AllowCloudAnchorDelay)
        {
            if(timePassedAfterPlanesDetected <= maxScanningAreaTime)
            {
                timePassedAfterPlanesDetected += Time.deltaTime * 1.0f;
                GetComponent<ARDebugManager>().LogInfo($"Experience starts in {maxScanningAreaTime-timePassedAfterPlanesDetected} sec(s)");
            }
            else
            {
                timePassedAfterPlanesDetected = maxScanningAreaTime;
                Activate();
            }
        }
    }

    void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if(!Initialized)
        {
            AllowCloudAnchorDelay = true;
        }
    }

    private void Activate()
    {
        GetComponent<ARDebugManager>().LogInfo("Activate AR Cloud Anchor Experience");
        OnInitialized?.Invoke();
        Initialized = true;
        AllowCloudAnchorDelay = false;
        arPlaneManager.enabled = false;
        arPointCloudManager.enabled = false;
    }

    public void Restart()
    {
        GetComponent<ARDebugManager>().LogInfo("Restart AR Cloud Anchor Experience");
        OnRetarted?.Invoke();
        Initialized = false;
        AllowCloudAnchorDelay = true;
        arPlaneManager.enabled = true;
        arPointCloudManager.enabled = true;
    }
}

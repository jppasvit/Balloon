using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class UnityEventResolver : UnityEvent<Transform>{}

public class ARCloudAnchorManager : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera = null;

    [SerializeField]
    private float resolveAnchorPassedTimeout = 10.0f;

    private ARAnchorManager arAnchorManager = null;

    private ARAnchor pendingHostAnchor = null;

    private ARCloudAnchor cloudAnchor = null;

    private string anchorToResolve;

    private bool anchorUpdateInProgress = false;

    private bool anchorResolveInProgress = false;
    
    private float safeToResolvePassed = 0;

    private UnityEventResolver resolver = null;

    private void Awake() 
    {
        resolver = new UnityEventResolver();   
        resolver.AddListener((t) => gameObject.GetComponent<ARPlacementManager>().ReCreatePlacement(t));
    }

    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position,
            arCamera.transform.rotation);
    }

#region Anchor Cycle

    public void QueueAnchor(ARAnchor arAnchor)
    {
        pendingHostAnchor = arAnchor;
    }

    public void HostAnchor()
    {
        gameObject.GetComponent<ARDebugManager>().LogInfo($"HostAnchor executing");

        FeatureMapQuality quality =
            arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());

        cloudAnchor = arAnchorManager.HostCloudAnchor(pendingHostAnchor, 1);
    
        if(cloudAnchor == null)
        {
            gameObject.GetComponent<ARDebugManager>().LogError("Unable to host cloud anchor");
        }
        else
        {
            anchorUpdateInProgress = true;
        }
    }
    
    public void Resolve()
    {
        gameObject.GetComponent<ARDebugManager>().LogInfo("Resolve executing");

        cloudAnchor = arAnchorManager.ResolveCloudAnchorId(anchorToResolve);

        if(cloudAnchor == null)
        {
            gameObject.GetComponent<ARDebugManager>().LogError($"Failed to resolve cloud achor id {cloudAnchor.cloudAnchorId}");
        }
        else
        {
            anchorResolveInProgress = true;
        }
    }

    private void CheckHostingProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        if(cloudAnchorState == CloudAnchorState.Success)
        {
            gameObject.GetComponent<ARDebugManager>().LogError("Anchor successfully hosted");
            
            anchorUpdateInProgress = false;

            // keep track of cloud anchors added
            anchorToResolve = cloudAnchor.cloudAnchorId;
        }
        else if(cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            gameObject.GetComponent<ARDebugManager>().LogError($"Fail to host anchor with state: {cloudAnchorState}");
            anchorUpdateInProgress = false;
        }
    }

    private void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;

        gameObject.GetComponent<ARDebugManager>().LogInfo($"ResolveCloudAnchor state {cloudAnchorState}");

        if (cloudAnchorState == CloudAnchorState.Success)
        {
            gameObject.GetComponent<ARDebugManager>().LogInfo($"CloudAnchorId: {cloudAnchor.cloudAnchorId} resolved");

            resolver.Invoke(cloudAnchor.transform);

            anchorResolveInProgress = false;
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            gameObject.GetComponent<ARDebugManager>().LogError($"Fail to resolve Cloud Anchor with state: {cloudAnchorState}");

            anchorResolveInProgress = false;
        }
    }

#endregion

    void Update()
    {
        // check progress of new anchors created
        if(anchorUpdateInProgress)
        {
            CheckHostingProgress();
            return;
        }

        if(anchorResolveInProgress && safeToResolvePassed <= 0)
        {
            // check evey (resolveAnchorPassedTimeout)
            safeToResolvePassed = resolveAnchorPassedTimeout;

            if(!string.IsNullOrEmpty(anchorToResolve))
            {
                gameObject.GetComponent<ARDebugManager>().LogInfo($"Resolving AnchorId: {anchorToResolve}");
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }
}

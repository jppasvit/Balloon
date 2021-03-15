using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CloudAnchorManager : MonoBehaviour
{
    // Host
    public ARCloudAnchor hostCloudAnchor { get; set; }
    public GameObject hostAnchorBasePrefab;
    public GameObject spawnHostAnchorBase { get; set; }
    private string cloudAnchorId;
    private bool checkHost = false;
    private bool anchorHosted = false;
    private int hostErrors = 0;
    private string hostWarningMessage = "";

    // Resolve
    public ARCloudAnchor resolveCloudAnchor { get; set; }
    public GameObject resolveAnchorPrefab;
    public GameObject spawnResolveAnchor { get; set; }
    private bool checkResolve = false;
    private bool anchorResolved = false;
    private int resolveErrors = 0;
    private string resolveWarningMessage = "";

    private MessageArea messageArea;
    private ARAnchorManager anchorManager;
    public bool onlyGoodOrSufficientFeatureMapQuality = false;
    
    [SerializeField]
    private int admittedErrors = 30;
    public GameObject clearButton;


    private void Awake()
    {
        anchorManager = GetComponent<ARAnchorManager>();
        messageArea = GetComponent<MessageArea>();
        hostWarningMessage = WarningMessage("Hosting", false);
        resolveWarningMessage = WarningMessage("Resolving", false);
    }

    private void Update()
    {
        if ( checkHost )
        {
            var check = CheckHostOrResolve(hostCloudAnchor);
            if (check == 0)
            {
                Debug.LogError("HOST ERROR");
                if (CheckErrorsHostOrResolve(0))
                {
                    messageArea.ErrorMessage("Host error, try again.");
                    Clear();                  
                }
            }
            else if (check == 1)
            {
                messageArea.SuccessMessage("Host success");
                checkHost = false;
                anchorHosted = true;
                checkResolve = true; // Automatic resolve when hosting is successful
                clearButton.SetActive(false);
                Resolve();
            }
            else if (check == 2)
            {
                Debug.LogWarning("HOST NOT READY");
                messageArea.WarningMessage(hostWarningMessage);
                if (CheckErrorsHostOrResolve(0))
                {
                    hostWarningMessage = WarningMessage("Hosting", true);
                    clearButton.SetActive(true);
                }
            }
        }

        if ( checkResolve )
        {
            var check = CheckHostOrResolve(resolveCloudAnchor);
            if (check == 0)
            {
                Debug.LogError("RESOLVE ERROR");
                if (CheckErrorsHostOrResolve(1))
                {
                    messageArea.ErrorMessage("Resolve error, try again.");
                    Clear();
                }
            }
            else if (check == 1)
            {
                Debug.Log("RESOLVE SUCCESS");
                checkResolve = false;
                anchorResolved = true;
                if (spawnResolveAnchor != null )
                {
                    spawnResolveAnchor = Instantiate(resolveAnchorPrefab, resolveCloudAnchor.transform.position, resolveCloudAnchor.transform.rotation);
                }
                //spawnResolveAnchor.transform.SetParent(resolveCloudAnchor.transform, false);
            }
            else if (check == 2)
            {
                Debug.LogWarning("RESOLVE NOT READY");
                messageArea.WarningMessage(resolveWarningMessage);
                if (CheckErrorsHostOrResolve(0))
                {
                    resolveWarningMessage = WarningMessage("Resolving", true);
                    clearButton.SetActive(true);
                }
            }
        }
    }

    public void InstantiateAndHostCloudAnchor(Pose pose)
    {
        FeatureMapQuality quality = anchorManager.EstimateFeatureMapQualityForHosting(pose);

        bool pass = true;
        if (onlyGoodOrSufficientFeatureMapQuality)
        {
            pass = quality == FeatureMapQuality.Good || quality == FeatureMapQuality.Sufficient;
        }

        if (pass)
        {
            ARAnchor localAnchor = anchorManager.AddAnchor(pose);
            hostCloudAnchor = anchorManager.HostCloudAnchor(localAnchor, 1);
            spawnHostAnchorBase = Instantiate(hostAnchorBasePrefab, pose.position, pose.rotation);
            //spawnHostAnchorBase.transform.SetParent(hostCloudAnchor.transform, false);
            checkHost = true;
        }
    }

    public void ResolveCloudAnchor(string cloudAnchorId)
    {
        resolveCloudAnchor = anchorManager.ResolveCloudAnchorId(cloudAnchorId);
        checkResolve = true;
    }

    public void Resolve()
    {
        if ( string.IsNullOrEmpty(cloudAnchorId) || string.IsNullOrWhiteSpace(cloudAnchorId) )
        {
            cloudAnchorId = hostCloudAnchor.cloudAnchorId;
            if (!string.IsNullOrEmpty(cloudAnchorId) && !string.IsNullOrWhiteSpace(cloudAnchorId))
            {
                ResolveCloudAnchor(cloudAnchorId);
            }
        }
        
    }

    private void ChangeOpacityBalloon (float opacity)
    {
        GameObject cylinder = spawnHostAnchorBase.transform.GetChild(0).gameObject;
        GameObject sphere = spawnHostAnchorBase.transform.GetChild(1).gameObject;

        var color = cylinder.GetComponent<MeshRenderer>().material.color;
        cylinder.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, opacity);

        color = sphere.GetComponent<MeshRenderer>().material.color;
        sphere.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, opacity);
    }

    /*
     * 0 CloudAnchorState.ERROOOOR
     * 1 CloudAnchorState.Success
     * 2 CloudAnchorState.TaskInProgress
     * 3 None
     */
    public int CheckHostOrResolve(ARCloudAnchor cloudAnchor)
    {
        int check = 3;
        if (cloudAnchor)
        {
            if (cloudAnchor.cloudAnchorState == CloudAnchorState.Success)
            {
                check = 1;
            }
            else if (cloudAnchor.cloudAnchorState == CloudAnchorState.TaskInProgress)
            {
                check = 2;
            }
            else
            {
                check = 0;
            }
        }
        return check;
    }

    public void Clear()
    {
        checkHost = false;
        anchorHosted = false;
        checkResolve = false;
        anchorResolved = false;
        Destroy(spawnHostAnchorBase);
        Destroy(spawnResolveAnchor);
        hostCloudAnchor = null;
        resolveCloudAnchor = null;
        cloudAnchorId = "";
        hostWarningMessage = WarningMessage("Hosting", false);
        resolveWarningMessage = WarningMessage("Resolving", false);
        clearButton.SetActive(false);
    }

    public bool AnchorIsHosted ( )
    {
        return anchorHosted;
    }

    public bool AnchorIsResolved()
    {
        return anchorResolved;
    }

    /*
     * hostOrResolve == 0 Check host errors, check resolve errors otherwise
     * true if Errors == admittedErrors, false otherwise
     */
    private bool CheckErrorsHostOrResolve(int hostOrResolve)
    {
        if (hostOrResolve == 0)
        {
            if (hostErrors == admittedErrors)
            {
                hostErrors = 0;
                return true;
            }
            else
            {
                hostErrors++;
            }
        }
        else
        {
            if (resolveErrors == admittedErrors)
            {
                resolveErrors = 0;
                return true;
            }
            else
            {
                resolveErrors++;
            }

        }
        return false;
    }

    private string WarningMessage (string hostOrResolve, bool large)
    {
        var sentence = hostOrResolve + " is not ready yet, please wait...";
        if (large)
            sentence += "\nIf " + hostOrResolve.ToLower() + " does not finish, you can click 'Clear' button to try to host from first step.";
        return sentence;
    }

}

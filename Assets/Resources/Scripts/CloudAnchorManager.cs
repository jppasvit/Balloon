using Google.XR.ARCoreExtensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CloudAnchorManager : MonoBehaviour
{
    // Enum
    private enum CloudAnchorPhase
    {
        Host = 0,
        Resolve = 1
    }

    // Prefabs
    [Header("Prefabs")]
    public GameObject hostAnchorBasePrefab;
    public GameObject resolveAnchorPrefab;

    // Feature Map Quality
    [Header("Feature Map Quality")]
    public bool onlyGoodOrSufficientFeatureMapQuality = false;

    // Emulation
    [Header("Emulation")]
    public bool emulateHost = false;
    public bool emulateResolve = false;
    private bool emulatedResolvePlaced = false;

    // Wait to resolve
    [Header("Wait to resolve")]
    [SerializeField]
    private bool needWaitForResolve = false;
    [SerializeField]
    private float waitSecondsForResolve = 30;
    private float secondsWaited = 0;

    // Errors
    [Header("Errors for hosting or resolving")]
    [SerializeField]
    private int admittedErrors = 30;
    private int hostErrors = 0;
    private int resolveErrors = 0;

    // Check flags
    private bool checkResolve = false;
    private bool checkHost = false;

    // Completed tasks
    private bool anchorHosted = false;
    private bool anchorResolved = false;

    // ARCloudAnchors
    public ARCloudAnchor hostCloudAnchor { get; set; }
    public ARCloudAnchor resolveCloudAnchor { get; set; }

    // Spawn anchors
    public GameObject spawnHostAnchorBase { get; set; }
    public GameObject spawnResolveAnchor { get; set; }

    // Messages
    private static string hostWarningMessage = "Hosting is not ready yet, please wait...";
    private static string hostLargeWarningMessage = hostWarningMessage + "\nIf hosting does not finish, you can click 'Manual Host' to instantiate the balloon manually or 'Clear' to try again";
    private static string hostMessage = hostWarningMessage;
    private static string hostErrorMessage = "Host error. if hosting does not finish, you can click 'Manual Host' to instantiate the balloon manually or 'Clear' to try again";
    private static string resolveWarningMessage = "Resolving is not ready yet, please wait...";
    private static string resolveLargeWarningMessage = resolveWarningMessage + "\nIf resolving does not finish, you can click 'Manual Resolve' to instantiate the balloon manually";
    private static string resolveErrorMessage = "Resolve error. If resolving does not finish, you can click 'Manual Resolve' to instantiate the balloon manually";
    private static string resolveMessage = resolveWarningMessage;
    private static string firstInstructionMessage = "Please tap on the plane where you want the balloon to be created.";

    // CloudAnchorId
    private string cloudAnchorId;

    // Managers
    [Header("Managers")]
    [SerializeField]
    private ARAnchorManager anchorManager;
    [SerializeField]
    private TaskButtonController taskButtonController;

    // PhotonView
    private PhotonView photonView;

    private void Awake()
    {
        if ( anchorManager == null )
        {
            anchorManager = GetComponent<ARAnchorManager>();
        }

        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if ( checkHost )
        {
            if ( emulateHost )
            {
                OnEmulateHost();
            }
            else
            {
                if ( hostCloudAnchor.cloudAnchorState == CloudAnchorState.Success )
                {
                    OnHostSuccess();
                }
                else if ( hostCloudAnchor.cloudAnchorState == CloudAnchorState.TaskInProgress )
                {
                    MessageArea.instance.WarningMessage(hostMessage);
                    if ( CheckErrorsHostOrResolve(CloudAnchorPhase.Host) )
                    {
                        OnHostNotReady();
                        hostMessage = hostLargeWarningMessage;
                    }
                }
                else
                {
                    if ( CheckErrorsHostOrResolve(CloudAnchorPhase.Host) )
                    {
                        OnHostError();
                    }
                }
            }
        }

        if ( checkResolve )
        {
            if ( emulateResolve )
            {
                OnEmulateResolve();
                if ( emulatedResolvePlaced )
                {
                    OnEmulatedResolveSuccess();
                }
            }
            else
            {
                // Wait To Resolve
                if (needWaitForResolve)
                {
                    if (WaitToResolve())
                    {
                        Resolve();
                        needWaitForResolve = false;
                    }
                    else
                    {
                        return;
                    }
                }
                // Check resolve
                if ( resolveCloudAnchor.cloudAnchorState == CloudAnchorState.Success )
                {
                    OnResolveCloudAnchorSuccess();
                }
                else if ( resolveCloudAnchor.cloudAnchorState == CloudAnchorState.TaskInProgress )
                {
                    MessageArea.instance.WarningMessage(resolveMessage);
                    if ( CheckErrorsHostOrResolve(CloudAnchorPhase.Resolve) )
                    {
                        OnResolveNotReady();
                        resolveMessage = resolveLargeWarningMessage;
                    }
                }
                else
                {
                    if ( CheckErrorsHostOrResolve(CloudAnchorPhase.Resolve) )
                    {
                        OnResolveError();
                    }
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
            photonView.RPC("RPC_Resolve", RpcTarget.AllBuffered, hostCloudAnchor.cloudAnchorId);
        }
        
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

        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Resolve, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Host, false);

        secondsWaited = 0;
        needWaitForResolve = false;

        emulateHost = false;
        emulateResolve = false;

        MessageArea.instance.InfoMessage(firstInstructionMessage);
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
    private bool CheckErrorsHostOrResolve(CloudAnchorPhase hostOrResolve)
    {
        if ( hostOrResolve == CloudAnchorPhase.Host )
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
        else if ( hostOrResolve == CloudAnchorPhase.Resolve )
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

    private bool WaitToResolve()
    {
        if (secondsWaited < waitSecondsForResolve)
        {
            secondsWaited += Time.deltaTime;
            return false;
        }
        
        return true;
    }

    public void LocateEmulatedResolve(Pose pose)
    {
        if (spawnResolveAnchor == null)
        {
            spawnResolveAnchor = BalloonSynchronizer.instance.InstantiateAndAllocateViewId(resolveAnchorPrefab, pose.position, pose.rotation);
        }
        else
        {
            spawnResolveAnchor.transform.position = pose.position;
        }
        emulatedResolvePlaced = true;
    }

    public bool IsResolveEmulation()
    {
        return emulateResolve;
    }

    private void OnResolveCloudAnchorSuccess()
    {
        checkResolve = false;
        anchorResolved = true;
        if (spawnResolveAnchor == null)
        {
            spawnResolveAnchor = BalloonSynchronizer.instance.InstantiateAndAllocateViewId(resolveAnchorPrefab, resolveCloudAnchor.transform.position, resolveCloudAnchor.transform.rotation);
        }

        if (spawnHostAnchorBase != null)
        {
            Destroy(spawnHostAnchorBase);
        }

        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Resolve, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Host, false);

        MessageArea.instance.InfoMessage("Wait until the another player's balloon is synchronized.\nThe game starts when balloons are synchronized.");
    }

    private void OnEmulatedResolveSuccess()
    {
        checkResolve = false;
        anchorResolved = true;

        if (spawnHostAnchorBase != null)
        {
            Destroy(spawnHostAnchorBase);
        }

        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Resolve, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Host, false);

        MessageArea.instance.InfoMessage("Wait until the another player's balloon is synchronized.\nThe game starts when balloons are synchronized.");
    }

    private void OnHostSuccess()
    {
        MessageArea.instance.SuccessMessage("Host success");
        checkHost = false;
        anchorHosted = true;
        // Automatic resolve when hosting is successful
        checkResolve = true;
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Resolve, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Host, false);
        if (!needWaitForResolve)
        {
            Resolve();
        }
        //Emulate resolve
        if ( emulateResolve )
        {
            MessageArea.instance.InfoMessage(firstInstructionMessage);
        }
    }

    private void OnEmulateHost()
    {
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Resolve, false);
        taskButtonController.SetActiveButton(TaskButton.TaskButtonType.Host, false);
        checkHost = false;
        anchorHosted = true;
        checkResolve = true;
        emulateResolve = true;
    }

    private void OnHostError()
    {
        MessageArea.instance.ErrorMessage(hostErrorMessage);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, true);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Resolve, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Host, true);
    }

    private void OnHostNotReady()
    {
        MessageArea.instance.WarningMessage(hostLargeWarningMessage);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, true);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Resolve, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Host, true);
    }

    private void OnResolveError()
    {
        MessageArea.instance.ErrorMessage(resolveErrorMessage);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Resolve, true);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Host, false);
    }

    private void OnResolveNotReady()
    {
        MessageArea.instance.WarningMessage(resolveLargeWarningMessage);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Resolve, true);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Host, false);
    }

    private void OnEmulateResolve()
    {
        taskButtonController.SetActiveButton(TaskButton.TaskButtonType.Resolve, false);
        taskButtonController.SetActiveTaskButton(TaskButton.TaskButtonType.Clear, false);
    }

    [PunRPC]
    public void RPC_Resolve(string id)
    {
        Debug.LogAssertion("RPC_Resolve <- " + id);
        cloudAnchorId = id;
        checkHost = false; // For the others
        anchorHosted = true; // For the others
        if (!string.IsNullOrEmpty(cloudAnchorId) && !string.IsNullOrWhiteSpace(cloudAnchorId))
        {
            ResolveCloudAnchor(cloudAnchorId);
        }
        MessageArea.instance.InfoMessage("Resolving balloon wait...");
    }
}

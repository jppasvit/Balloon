using Google.XR.ARCoreExtensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceObject : MonoBehaviour
{
    private Vector2 touchPosition;
    private ARRaycastManager raycastManager;
    private CloudAnchorManager managerCloudAnchor;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        managerCloudAnchor = GetComponent<CloudAnchorManager>();
    }

    void Update()
    {
        if (!TryGetTouchPosition(out touchPosition))
        {
            return;
        }

        if ( managerCloudAnchor.AnchorIsHosted() && managerCloudAnchor.AnchorIsResolved() )
        {
            PhotonView photonView = GameController.instance.GetBalloonPhotonView();
            if (GameController.instance.IsBalloonTouched(touchPosition))
            {
                Debug.Log("GameController Balloon TOUCHED");
                Debug.LogError("BalloonSynchronizer itsBalloonIsInstantiated: " + BalloonSynchronizer.instance.itsBalloonIsInstantiated);
                Debug.LogAssertion("BalloonSynchronizer balloonViewId: " + BalloonSynchronizer.instance.balloonViewId);
                Debug.Log("photonView IS MINE: " + photonView.IsMine);
            }
            if ( photonView != null && GameController.instance.IsBalloonTouched(touchPosition) && BalloonSynchronizer.instance.itsBalloonIsInstantiated )
            {
                photonView.RPC("RPC_TapOnBalloon", RpcTarget.AllBuffered);
                Debug.Log("Hosted resolved - in TapOnBalloon");
            }
        }
        else
        {
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                if (managerCloudAnchor.AnchorIsHosted())
                {
                    if (!managerCloudAnchor.AnchorIsResolved())
                    {
                        if (managerCloudAnchor.IsResolveEmulation())
                        {
                            managerCloudAnchor.LocateEmulatedResolve(hitPose);
                        }
                    }
                }
                else
                {
                    if (managerCloudAnchor.spawnHostAnchorBase == null)
                    {
                        managerCloudAnchor.InstantiateAndHostCloudAnchor(hitPose);
                    }
                }
            }
        }
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            if (BlockUI.IsPointOverUIObject(touchPosition))
            {
                return false;
            }
            return true;
        }

        touchPosition = default;
        return false;
    }

}

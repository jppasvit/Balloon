using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceObject : MonoBehaviour
{
    public GameObject gameObjectToInstantiate;
    public GameObject spawnedGameObject { get; set; }
    private Vector2 touchPosition;
    private ARRaycastManager raycastManager;
    private CloudAnchorManager managerCloudAnchor;
    public ARCloudAnchor spawnedCloudAnchor { get; set; }


    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

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

        //SelectSpawn(touchPosition);

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            if ( managerCloudAnchor.AnchorIsHosted() )
            {
                if( !managerCloudAnchor.AnchorIsResolved() )
                {
                    if ( managerCloudAnchor.IsResolveEmulation() )
                    {
                        managerCloudAnchor.LocateEmulatedResolve(hitPose);
                    }
                }
                else
                {
                    //SpawnOrLocateGameObject(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
                }
            }
            else
            {
                if ( managerCloudAnchor.spawnHostAnchorBase == null )
                {
                    managerCloudAnchor.InstantiateAndHostCloudAnchor(hitPose);
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

    void SpawnOrLocateGameObject(GameObject gameObjectToInstantiate, Vector3 position, Quaternion rotation)
    {
        if (spawnedGameObject == null)
        {
            spawnedGameObject = Instantiate(gameObjectToInstantiate, position, rotation);
        }
        else
        {
            spawnedGameObject.transform.position = position;
        }
    }

    void SelectSpawn(Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit hitObject;

                if (Physics.SphereCast(ray, (float)0.1, out hitObject))
                {
                    spawnedGameObject = hitObject.transform.gameObject;
                }
            }
        }
    }

}

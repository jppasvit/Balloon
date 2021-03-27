using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class PlaneClassificationManager : MonoBehaviour
{
    public static PlaneClassificationManager instance { get; private set; }
    private ARPlaneManager planeManager;
    private ARRaycastManager raycastManager;
    private float floorApproximation = float.MaxValue;
    private TrackableId trackableId = TrackableId.invalidId;
    public float floorApproximationRange;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        DetectFloor();
    }

    void DetectFloor()
    {
        List<ARRaycastHit> hitsForFloor = new List<ARRaycastHit>();
        Vector2 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        if (raycastManager.Raycast(screenCenter, hitsForFloor, TrackableType.PlaneWithinPolygon))
        {
            ARPlane plane = planeManager.GetPlane(hitsForFloor[0].trackableId);

            if (plane.alignment.IsHorizontal() && plane.alignment != PlaneAlignment.HorizontalDown)
            {
                var floorTrackableId = planeManager.GetPlane(trackableId);
                if (floorTrackableId == null)
                {
                    trackableId = TrackableId.invalidId;
                }
                if (floorApproximation == default || plane.center.y < floorApproximation || trackableId == TrackableId.invalidId)
                {
                    floorApproximation = plane.center.y;
                    trackableId = hitsForFloor[0].trackableId;
                }
            }

        }
    }

    public bool IsFloor(ARPlane plane)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (plane.alignment.IsHorizontal() && plane.alignment != PlaneAlignment.HorizontalDown
             && ApproximatelyWithRange(plane.center.y, floorApproximation, floorApproximationRange))
            {
                return true;
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (plane.classification == PlaneClassification.Floor)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsTable(ARPlane plane)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (plane.alignment.IsHorizontal() && plane.alignment != PlaneAlignment.HorizontalDown && !IsFloor(plane))
            {
                return true;
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (plane.classification == PlaneClassification.Table)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsVertical(ARPlane plane)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (plane.alignment.IsVertical())
            {
                return true;
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (plane.classification == PlaneClassification.Wall)
            {
                return true;
            }
        }

        return false;
    }


    public static bool ApproximatelyWithRange(float f1, float f2, float range)
    {
        float f1Abs = Mathf.Abs(f1);
        float f2Abs = Mathf.Abs(f2);
        float diff = Mathf.Abs(f1 - f2);

        if (Mathf.Approximately(f1, f2))
        {
            return true;
        }
        else if (Mathf.Approximately(f1, 0) || Mathf.Approximately(f2, 0) || diff < float.MinValue)
        {
            return diff < (range * float.MinValue);
        }
        else
        {
            return (diff / (f1Abs + f2Abs)) < range;
        }
    }

}

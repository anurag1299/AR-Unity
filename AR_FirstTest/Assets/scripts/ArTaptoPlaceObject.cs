using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class ArTaptoPlaceObject : MonoBehaviour
{
    public GameObject placementIndicator;
    public GameObject []objectToPlace;
    public int selected = 0;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager aRRaycastManager;
    private Pose placementObject;
    private bool placementPoseIsValid = false;
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        if(placementPoseIsValid && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && !IsPointerOverUIObject())
        {
            placeObject();
        }
    }

    private void placeObject()
    {
        Instantiate(objectToPlace[selected], placementObject.position, placementObject.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementObject.position, placementObject.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }  
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f,0.5f,0));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementObject = hits[0].pose;
            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementObject.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    public void changeSelected(int direction)
    {
        if(direction > 0)
        {
            if(selected < objectToPlace.Length-1)
            {
                selected++;
            }
        }
        else if (direction < 0)
        {
            if(selected > 0)
            {
                selected--;
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

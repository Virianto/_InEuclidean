using System;
using System.Collections.Generic;
using UnityEngine;

public class BMazePortals : MonoBehaviour
{
    #region ATTRIBUTES
        
    [Serializable]
    struct SinglePortalPoints
    {
        /// <summary>
        /// Space where the PORTAL is placed (not destiny point)
        /// </summary>
        public NonEuclideanSpaces originSpace;

        /// <summary>
        /// Space where player will be teleported after touching the portal
        /// </summary>
        public NonEuclideanSpaces destinySpace;
        
        /// <summary>
        /// Quad with collider in scene. Twin camera will render on it
        /// </summary>
        public Transform portalFrame;

        /// <summary>
        /// Twin camera will take this point as reference to render
        /// </summary>
        public Transform destinyPoint;

        /// <summary>
        /// When player is closer to this point, the portal will stay active
        /// </summary>
        public Transform activationPoint;
    }
        
    [Header("ACTORS")]

    [SerializeField] GameObject twinCamera;

    [Header("REF POINTS")]

    [SerializeField] List<SinglePortalPoints> allPortals = new List<SinglePortalPoints>();

    Dictionary<Transform, SinglePortalPoints> activationPoints_Portals = new Dictionary<Transform, SinglePortalPoints>();    

    SinglePortalPoints currentActivePortal;

    List<Transform> activationPointsInCurrentSpace = new List<Transform>();
    
    Transform mainCameraTransform;

    Transform mainRef;
    Transform twinRef;

    Vector3 relativeMainCamPosition;

    NonEuclideanSpaces _currentSpace;

    public NonEuclideanSpaces CurrentSpace
    {
        private get
        {
            return _currentSpace;
        }

        set
        {            
            _currentSpace = value;

            for (int a = 0; a < allPortals.Count; ++a)
            {
                if (allPortals[a].originSpace == _currentSpace)
                {
                    activationPointsInCurrentSpace.Add(allPortals[a].activationPoint);
                }
            }            
        }
    }

    bool playerIsCrossing = false;

    #endregion

    #region METHODS

    void Awake()
    {
        for (int a = 0; a < allPortals.Count; ++a)
        {
            activationPoints_Portals.Add(allPortals[a].activationPoint, allPortals[a]);

            SinglePortalPoints x = allPortals[a];

            CNormalizedTrigger nTrigger = allPortals[a].portalFrame.GetComponentInChildren<CNormalizedTrigger>();

            nTrigger.OnColliderDetectedFromForward += (Transform t) =>
            {
                playerIsCrossing = !playerIsCrossing;
            };

            nTrigger.OnColliderDetectedFromBack += (Transform transformDetected) =>
            {
                if (!playerIsCrossing)
                {
                    playerIsCrossing = true;
                    SwitchElements(x, transformDetected);
                }
                else
                {
                    playerIsCrossing = false;
                }
            };
        }

        mainCameraTransform = Camera.main.transform;
    }

    void Start()
    {
        CurrentSpace = NonEuclideanSpaces.First;     
    }

    void LateUpdate()
    {
        // Get closest portal inside current Non Euclidean Space (Box)
        activationPoints_Portals.TryGetValue(GetClosestActivationPoint(activationPointsInCurrentSpace.ToArray()), out currentActivePortal);

        // Set references to start working

        mainRef = currentActivePortal.portalFrame;
        twinRef = currentActivePortal.destinyPoint;

        // MOVE TWIN CAMERA CONSIDERING REFERENCES

        relativeMainCamPosition = mainRef.InverseTransformPoint(mainCameraTransform.position);
        twinCamera.transform.localPosition = twinRef.TransformPoint(relativeMainCamPosition);

        // ROTATE TWIN CAMERA CONSIDERING REFERENCES

        twinCamera.transform.rotation = Quaternion.Inverse(mainRef.rotation) * mainCameraTransform.rotation;
        //twinCamera.transform.localRotation = Quaternion.Inverse(mainRef.localRotation) * mainCameraTransform.localRotation;
    }

    Transform GetClosestActivationPoint(Transform[] activationPoints)
    {
        float mDistance = Mathf.Infinity;
        int closestPointIndex = -1;

        for(int a = 0; a < activationPoints.Length; ++a)
        {
            float d = Vector3.Distance(mainCameraTransform.position, activationPoints[a].position);
            if(d < mDistance)
            {
                mDistance = d;
                closestPointIndex = a;
            }
        }

        return activationPoints[closestPointIndex];
    }

    void SwitchElements(SinglePortalPoints portalTouched, Transform elementToSwitch)
    {        
        CurrentSpace = portalTouched.destinySpace;

        Vector3 twinCamPos = twinCamera.transform.position;

        Vector3 r = mainCameraTransform.InverseTransformPoint(elementToSwitch.position);

        elementToSwitch.position = twinCamPos + r;
        elementToSwitch.localRotation *=  Quaternion.Inverse(mainRef.rotation) * twinRef.rotation;
    }    

    #endregion
}
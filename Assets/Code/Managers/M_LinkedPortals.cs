using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public enum InEuclideanSpaces : byte
{
    Zero,
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    Sixth,
    Seventh,
    Eighth
}

public class M_LinkedPortals : MonoBehaviour
{
    #region ATTRIBUTES

    [Serializable]
    struct SinglePortalPoints
    {
        /// <summary>
        /// Space where the PORTAL is placed (not destiny point)
        /// </summary>
        public InEuclideanSpaces originSpace;

        /// <summary>
        /// Space where player will be teleported after touching the portal
        /// </summary>
        public InEuclideanSpaces destinySpace;

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

    [SerializeField] CinemachineCamera cinemachineCamera;

    [Header("PORTALS AND REFERENCES")]

    [SerializeField] List<SinglePortalPoints> allPortals = new List<SinglePortalPoints>();

    Dictionary<Transform, SinglePortalPoints> activationPoints_Portals = new Dictionary<Transform, SinglePortalPoints>();

    SinglePortalPoints currentActivePortal;

    List<Transform> activationPointsInCurrentSpace = new List<Transform>();

    Transform mainCameraTransform;

    Transform mainRef;
    Transform twinRef;

    Vector3 relativeMainCamPosition;

    InEuclideanSpaces _currentSpace;

    public InEuclideanSpaces CurrentSpace
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

    CinemachinePanTilt myPanTilt;

    bool playerIsCrossing = false;

    #endregion

    #region METHODS

    void Awake()
    {
        // Since this class and the triggers share the same lifetime in scene, there's no unsubscription managed
        // for any event and all subscriptions are made in 'Awake'. If you plan to make any complex use that
        // involves Enabling and Disabling the whole system, consider managing this differently (OnEnable / OnDisable)

        
        for (int a = 0; a < allPortals.Count; ++a)
        {
            activationPoints_Portals.Add(allPortals[a].activationPoint, allPortals[a]);

            SinglePortalPoints x = allPortals[a];

            C_NormalizedTrigger nTrigger = allPortals[a].portalFrame.GetComponentInChildren<C_NormalizedTrigger>();

            nTrigger.OnFrontalTriggerEnter += (Transform t) =>
            {
                playerIsCrossing = !playerIsCrossing;
            };

            nTrigger.OnRearTriggerEnter += (Transform transformDetected) =>
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
        myPanTilt = cinemachineCamera.GetCinemachineComponent<CinemachinePanTilt>();
        //myPOV = cinemachineCamera.GetCinemachineComponent<CinemachinePOV>();       
    }

    void Start()
    {
        CurrentSpace = InEuclideanSpaces.First;
    }

    void LateUpdate()
    {
        // Get closest portal inside current Non Euclidean Space

        activationPoints_Portals.TryGetValue(GetClosestActivationPoint(activationPointsInCurrentSpace.ToArray()), out currentActivePortal);

        // Set references to start working

        mainRef = currentActivePortal.portalFrame;
        twinRef = currentActivePortal.destinyPoint;

        // MOVE TWIN CAMERA CONSIDERING REFERENCES

        relativeMainCamPosition = mainRef.InverseTransformPoint(mainCameraTransform.position);
        twinCamera.transform.localPosition = twinRef.TransformPoint(relativeMainCamPosition);

        // ROTATE TWIN CAMERA CONSIDERING REFERENCES

        twinCamera.transform.localRotation = Quaternion.Inverse(mainRef.localRotation) * mainCameraTransform.localRotation;
        //twinCamera.transform.rotation = mainCameraTransform.rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activationPoints"></param>
    /// <returns>Closest activation point to main camera</returns>
    Transform GetClosestActivationPoint(Transform[] activationPoints)
    {
        float mDistance = Mathf.Infinity;
        int closestPointIndex = -1;

        for (int a = 0; a < activationPoints.Length; ++a)
        {
            float d = Vector3.Distance(mainCameraTransform.position, activationPoints[a].position);
            if (d < mDistance)
            {
                mDistance = d;
                closestPointIndex = a;
            }
        }

        return activationPoints[closestPointIndex];
    }

    /// <summary>
    /// Teleports the element detected to the linked position
    /// </summary>
    /// <param name="portalTouched">Trigger touched by element</param>
    /// <param name="elementToSwitch">Transform to teleport</param>
    void SwitchElements(SinglePortalPoints portalTouched, Transform elementToSwitch)
    {
        CurrentSpace = portalTouched.destinySpace;

        Vector3 twinCamPos = twinCamera.transform.position;

        Vector3 r = mainCameraTransform.InverseTransformPoint(elementToSwitch.position);

        elementToSwitch.position = twinCamPos + r;

        Vector3 twinEulerRotation = twinCamera.transform.rotation.eulerAngles;

        Quaternion dstRotation = twinRef.rotation * Quaternion.Inverse(mainCameraTransform.rotation) * mainRef.rotation;
        Vector3 dstEuler = Quaternion.ToEulerAngles(dstRotation);

        myPanTilt.PanAxis.Value = dstEuler.x;
        myPanTilt.TiltAxis.Value = dstEuler.y;
        //myPanTilt.m_Pitch.Value = dstEuler.x;
        //myPanTilt.m_Yaw.Value = dstEuler.y;

        //mainCameraTransform.rotation *= Quaternion.Inverse(twinCamera.transform.rotation) * twinRef.rotation;
        //elementToSwitch.localRotation *= Quaternion.Inverse(mainRef.rotation) * twinRef.rotation;
    }

    #endregion
}
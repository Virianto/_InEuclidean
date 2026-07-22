using System;
using System.Collections.Generic;
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
        [Tooltip("Space where the PORTAL is placed (not destiny point)")]
        public InEuclideanSpaces originSpace;

        /// <summary>
        /// Space where player will be teleported after touching the portal
        /// </summary>
        [Tooltip("Space where player will be teleported after touching the portal")]
        public InEuclideanSpaces destinySpace;

        /// <summary>
        /// Quad with collider in scene. Twin camera will render on it
        /// </summary>
        [Tooltip("Quad with collider in scene. Twin camera will render on it")]
        public Transform portalFrame;

        /// <summary>
        /// Twin camera will take this point as reference to render
        /// </summary>
        [Tooltip("Twin camera will take this point as reference to render")]
        public Transform destinyPoint;

        /// <summary>
        /// When player is closer to this point, the portal will stay active
        /// </summary>
        [Tooltip("When player is closer to this point, the portal will stay active")]
        public Transform activationPoint;
    }

    [Header("ACTORS")]

    [SerializeField] GameObject twinCamera;

    [SerializeField] Camera mainCamera;

    [Header("PORTALS AND REFERENCES")]

    [SerializeField] List<SinglePortalPoints> allPortals = new();

    Dictionary<Transform, SinglePortalPoints> activationPoints_Portals = new();

    SinglePortalPoints _currentActivePortal;

    List<Transform> _activationPointsInCurrentSpace = new();

    Transform _mainCameraTransform;

    Transform _mainRef;
    Transform _twinRef;

    Vector3 _relativeMainCamPos;
    Quaternion _relativeMainCamRot;

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
                    _activationPointsInCurrentSpace.Add(allPortals[a].activationPoint);
                }
            }
        }
    }

    bool _playerIsCrossing = false;

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

            nTrigger.OnFrontalTriggerEnter += (t) =>
            {
                _playerIsCrossing = !_playerIsCrossing;
            };

            nTrigger.OnRearTriggerEnter += (transformDetected) =>
            {
                if (!_playerIsCrossing)
                {
                    _playerIsCrossing = true;
                    SwitchElements(x, transformDetected);
                }
                else
                {
                    _playerIsCrossing = false;
                }
            };
        }

        _mainCameraTransform = Camera.main.transform;    
    }

    void Start()
    {
        CurrentSpace = InEuclideanSpaces.Zero;
    }

    void LateUpdate()
    {
        // Get closest portal inside current Non Euclidean Space

        activationPoints_Portals.TryGetValue(GetClosestActivationPoint(_activationPointsInCurrentSpace.ToArray()), out _currentActivePortal);

        // Set references to start working

        _mainRef = _currentActivePortal.portalFrame;
        _twinRef = _currentActivePortal.destinyPoint;

        // MOVE TWIN CAMERA CONSIDERING REFERENCES

        _relativeMainCamPos = _mainRef.InverseTransformPoint(_mainCameraTransform.position);
        twinCamera.transform.localPosition = _twinRef.TransformPoint(_relativeMainCamPos);

        // ROTATE TWIN CAMERA CONSIDERING REFERENCES
        
        _relativeMainCamRot = Quaternion.FromToRotation(_mainRef.forward, _mainCameraTransform.forward);
        
        twinCamera.transform.rotation = _relativeMainCamRot * _twinRef.rotation;
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
            float d = Vector3.Distance(_mainCameraTransform.position, activationPoints[a].position);
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

        // MOVE MAIN VIEW POSITION TO TWIN CAMERA
        
        elementToSwitch.position = twinCamera.transform.position;

        // CHANGE MAIN VIEW ROTATION TO MIMIC TWIN CAMERA
        
        _mainCameraTransform.rotation = twinCamera.transform.localRotation;
    }

    #endregion
}
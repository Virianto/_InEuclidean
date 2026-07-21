using UnityEngine;

public class M_SimplePortal : MonoBehaviour
{
    #region ATTRIBUTES

    enum PlayerInSpace : byte
    {
        Alpha,
        Omega
    }

    struct PairedNormalizedTriggers
    {
        public C_NormalizedTrigger alphaTrigger;
        public C_NormalizedTrigger omegaTrigger;
    }

    Transform _mainCameraTransform;

    [Header("ACTORS")]

    [SerializeField] GameObject twinCamera;

    [Header("REF POINTS")]

    [SerializeField] Transform alphaEntry;
    [SerializeField] Transform omegaEntry;
    [SerializeField] Transform alphaExit;
    [SerializeField] Transform omegaExit;

    Transform _mainRef;
    Transform _twinRef;

    Vector3 _mainRefToMainCameraPosition;
    Quaternion _mainRefToMainCameraRotation;   

    PlayerInSpace _currentSpace = PlayerInSpace.Alpha;

    bool _playerIsCrossing = false;    

    #endregion

    #region METHODS

    void Awake()
    {
        _mainCameraTransform = Camera.main.transform;
        _mainRef = alphaEntry;
        _twinRef = omegaEntry;
    }

    void Start()
    {
        // Since this class and the triggers share the same lifetime in scene, there's no unsubscription managed
        // for any event and all subscriptions are made in 'Start'. If you plan to make any complex use that
        // involves Enabling and Disabling the whole system, consider managing this differently (OnEnable / OnDisable)

        
        C_NormalizedTrigger[] allTriggers = GetComponentsInChildren<C_NormalizedTrigger>();

        for (byte a = 0; a < allTriggers.Length; ++a)
        {
            allTriggers[a].OnFrontalTriggerEnter += (Transform t) => 
            {                
                _playerIsCrossing = !_playerIsCrossing;
            };

            allTriggers[a].OnRearTriggerEnter += (Transform t) =>
            {                                                
                if (!_playerIsCrossing)
                {
                    _playerIsCrossing = true;
                    SwitchElements(t);
                }
                else
                {
                    _playerIsCrossing = false;
                }
            };
        }
    }

    void LateUpdate()
    {
        if(_currentSpace == PlayerInSpace.Alpha)
        {
            float distanceToAlphaEntry = Vector3.Distance(_mainCameraTransform.position, alphaEntry.position);
            float distanceToAlphaExit = Vector3.Distance(_mainCameraTransform.position, alphaExit.position);
            
            _mainRef = distanceToAlphaEntry < distanceToAlphaExit ? alphaExit : alphaEntry;
            _twinRef = distanceToAlphaEntry < distanceToAlphaExit ? omegaExit : omegaEntry;
        }
        else
        {
            float distanceToOmegaEntry = Vector3.Distance(_mainCameraTransform.position, omegaEntry.position);
            float distanceToOmegaExit = Vector3.Distance(_mainCameraTransform.position, omegaExit.position);

            _mainRef = distanceToOmegaEntry < distanceToOmegaExit ? omegaEntry : omegaExit;
            _twinRef = distanceToOmegaEntry < distanceToOmegaExit ? alphaEntry : alphaExit;
        }

        _mainRefToMainCameraPosition = _mainRef.InverseTransformPoint(_mainCameraTransform.position);
        twinCamera.transform.localPosition = _twinRef.TransformPoint(_mainRefToMainCameraPosition);
        
        _mainRefToMainCameraRotation = Quaternion.FromToRotation(_mainCameraTransform.forward, _mainRef.forward);

        twinCamera.transform.rotation = Quaternion.Inverse(_mainRefToMainCameraRotation);
    }

    public void SwitchElements(Transform elementToSwitch)
    {
        if (_playerIsCrossing)
        {
            _currentSpace = _currentSpace == PlayerInSpace.Alpha ? PlayerInSpace.Omega : PlayerInSpace.Alpha;            

            Vector3 twinCamPos = twinCamera.transform.position;

            elementToSwitch.position = twinCamPos;
            elementToSwitch.rotation *= Quaternion.Inverse(_mainRef.rotation) * _twinRef.rotation;
        }        
    }

    #endregion
}
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

    Transform mainCameraTransform;

    [Header("ACTORS")]

    [SerializeField] GameObject twinCamera;

    [Header("REF POINTS")]

    [SerializeField] Transform alphaEntry;
    [SerializeField] Transform omegaEntry;
    [SerializeField] Transform alphaExit;
    [SerializeField] Transform omegaExit;

    Transform mainRef;
    Transform twinRef;

    Vector3 mainRefToMainCamera;

    PlayerInSpace currentSpace = PlayerInSpace.Alpha;

    bool playerIsCrossing = false;    

    #endregion

    #region METHODS

    void Awake()
    {
        mainCameraTransform = Camera.main.transform;
        mainRef = alphaEntry;
        twinRef = omegaEntry;
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
                playerIsCrossing = !playerIsCrossing;
            };

            allTriggers[a].OnRearTriggerEnter += (Transform t) =>
            {                                                
                if (!playerIsCrossing)
                {
                    playerIsCrossing = true;
                    SwitchElements(t);
                }
                else
                {
                    playerIsCrossing = false;
                }
            };
        }
    }

    void LateUpdate()
    {
        if(currentSpace == PlayerInSpace.Alpha)
        {
            float distanceToAlphaEntry = Vector3.Distance(mainCameraTransform.position, alphaEntry.position);
            float distanceToAlphaExit = Vector3.Distance(mainCameraTransform.position, alphaExit.position);
            
            mainRef = distanceToAlphaEntry < distanceToAlphaExit ? alphaExit : alphaEntry;
            twinRef = distanceToAlphaEntry < distanceToAlphaExit ? omegaExit : omegaEntry;
        }
        else
        {
            float distanceToOmegaEntry = Vector3.Distance(mainCameraTransform.position, omegaEntry.position);
            float distanceToOmegaExit = Vector3.Distance(mainCameraTransform.position, omegaExit.position);

            mainRef = distanceToOmegaEntry < distanceToOmegaExit ? omegaEntry : omegaExit;
            twinRef = distanceToOmegaEntry < distanceToOmegaExit ? alphaEntry : alphaExit;
        }

        mainRefToMainCamera = mainRef.InverseTransformPoint(mainCameraTransform.position);
        twinCamera.transform.localPosition = twinRef.TransformPoint(mainRefToMainCamera);

        twinCamera.transform.rotation = Quaternion.Inverse(mainRef.rotation) * mainCameraTransform.rotation;
    }

    public void SwitchElements(Transform elementToSwitch)
    {
        if (playerIsCrossing)
        {
            currentSpace = currentSpace == PlayerInSpace.Alpha ? PlayerInSpace.Omega : PlayerInSpace.Alpha;            

            Vector3 twinCamPos = twinCamera.transform.position;

            elementToSwitch.position = twinCamPos;
            elementToSwitch.rotation *= Quaternion.Inverse(mainRef.rotation) * twinRef.rotation;
        }        
    }

    #endregion
}
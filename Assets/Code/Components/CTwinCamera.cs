using UnityEngine;

public class CTwinCamera : MonoBehaviour
{
    #region ATTRIBUTES

    public Material targetMat;

    Camera selfCam;

    #endregion

    #region METHODS

    void Awake()
    {
        selfCam = GetComponent<Camera>();
    }

    void Start()
    {
        selfCam.targetTexture?.Release();
        selfCam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        targetMat.mainTexture = selfCam.targetTexture;
    }

    #endregion
}
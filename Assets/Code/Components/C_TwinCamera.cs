using UnityEngine;

public class C_TwinCamera : MonoBehaviour
{
    #region ATTRIBUTES

    [SerializeField]
    Material targetMat;

    Camera _selfCam;

    #endregion

    #region METHODS

    void Awake()
    {
        _selfCam = GetComponent<Camera>();
    }

    void Start()
    {
        _selfCam.targetTexture?.Release();
        _selfCam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        targetMat.mainTexture = _selfCam.targetTexture;
    }

    #endregion
}
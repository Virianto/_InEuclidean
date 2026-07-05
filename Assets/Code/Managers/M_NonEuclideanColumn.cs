using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_NonEuclideanColumn : MonoBehaviour
{
    #region ATTRIBTUES

    const string stencilReference = "_StencilRef";

    [Header("General references")]

    [Space(10)]

    [SerializeField] C_NormalizedTrigger columnNormalizedTrigger;

    [SerializeField] RectTransform cameraMaskRect;

    [Space(10)]

    [SerializeField] List<Transform> allMaskGroupsToUse = new List<Transform>();    

    [SerializeField] List<Transform> allItemGroupsToHide = new List<Transform>();
        
    Material cameraMaskMat;

    int maxMaskRef;

    int currentMaskRef = 0;

    #endregion

    #region METHODS

    void Awake()
    {
        cameraMaskRect.GetComponent<Image>().material = Instantiate(cameraMaskRect.GetComponent<Image>().material);
        cameraMaskMat = cameraMaskRect.GetComponent<Image>().material;

        int a;
        maxMaskRef = allItemGroupsToHide.Count - 1;

        for (a = 0; a < allMaskGroupsToUse.Count; ++a)
        {
            // Set all masks stencilReference to the same value by group

            SetAllItemsInGroupStencilRef(allMaskGroupsToUse[a], a);            
        }

        for (a = 0; a < allItemGroupsToHide.Count; ++a)
        {
            // Set all objects stencilReference to the same value by group

            SetAllItemsInGroupStencilRef(allItemGroupsToHide[a], a);
        }
    }

    void Start()
    {
        columnNormalizedTrigger.OnFrontalTriggerExit += OnFrontalExit;
        columnNormalizedTrigger.OnRearTriggerExit += OnRearExit;

        for(int a = 0; a < allMaskGroupsToUse.Count; ++a)
        {
            int index = a;

            C_SimpleTrigger[] allSimpleTriggersInMaskGroup = allMaskGroupsToUse[index].GetComponentsInChildren<C_SimpleTrigger>();

            for(int x = 0; x < allSimpleTriggersInMaskGroup.Length; ++x)
            {
                allSimpleTriggersInMaskGroup[x].OnSimpleTriggerEnter += () =>
                {
                    //Debug.Log("Entered a simple trigger");
                    currentMaskRef = allMaskGroupsToUse[index].GetComponentInChildren<Renderer>().material.GetInt(stencilReference);
                    cameraMaskMat.SetInt(stencilReference, currentMaskRef);
                };
            }
        }

        cameraMaskMat.SetInt(stencilReference, currentMaskRef);
    }

    
    /// <summary>
    /// This method acts as an intermediary to discard an unnecessary parameter while,
    /// at the same time, allow to un/subscribe without null references
    /// </summary>
    /// <param name="t">Unused parameter</param>
    void OnRearExit(Transform t)
    {
        ToggleMasksReferences(false);
    }

    /// <summary>
    /// This method acts as an intermediary to discard an unnecessary parameter while,
    /// at the same time, allow to un/subscribe without null references
    /// </summary>
    /// <param name="t">Unused parameter</param>
    void OnFrontalExit(Transform t)
    {
        ToggleMasksReferences(true);
    }

    void OnDisable()
    {
        columnNormalizedTrigger.OnFrontalTriggerExit -= OnFrontalExit;
        columnNormalizedTrigger.OnRearTriggerExit -= OnRearExit;
        
        for (int a = 0; a < allMaskGroupsToUse.Count; ++a)
        {
            int index = a;

            C_SimpleTrigger[] allSimpleTriggersInMaskGroup = allMaskGroupsToUse[index].GetComponentsInChildren<C_SimpleTrigger>();

            for (int x = 0; x < allSimpleTriggersInMaskGroup.Length; ++x)
            {
                allSimpleTriggersInMaskGroup[x].OnSimpleTriggerEnter -= () =>
                {
                    //Debug.Log("Entered a simple trigger");
                    currentMaskRef = allMaskGroupsToUse[index].GetComponentInChildren<Renderer>().material.GetInt(stencilReference);
                    cameraMaskMat.SetInt(stencilReference, currentMaskRef);
                };
            }
        }
    }    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="goForward"></param>
    void ToggleMasksReferences(bool goForward)
    {
        int maskGroupRefToPreserveIndex = goForward ? 1 : 0;
        int maskGroupRefToModifyIndex = goForward ? 0 : 1;

        int newStencilRef = (currentMaskRef + (goForward ? 1 : maxMaskRef)) % (maxMaskRef + 1);

        SetAllItemsInGroupStencilRef(allMaskGroupsToUse[maskGroupRefToPreserveIndex], currentMaskRef);                

        SetAllItemsInGroupStencilRef(allMaskGroupsToUse[maskGroupRefToModifyIndex], newStencilRef);
    }

    void SetAllItemsInGroupStencilRef(Transform itemsGroup, int newStencilRefValue)
    {
        Renderer[] masksRenderers = itemsGroup.GetComponentsInChildren<Renderer>();

        for (int m = 0; m < masksRenderers.Length; ++m)
        {
            masksRenderers[m].material.SetInt(stencilReference, newStencilRefValue);
        }
    }

    #endregion
}
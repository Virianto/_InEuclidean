using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class C_NormalizedTrigger : MonoBehaviour
{
    #region ATTRIBUTES

    public delegate void FrontalTriggerEnter(Transform crossingObject);
    public FrontalTriggerEnter OnFrontalTriggerEnter;

    public delegate void RearTriggerEnter(Transform crossingObject);
    public RearTriggerEnter OnRearTriggerEnter;

    public delegate void FrontalTriggerExit(Transform crossingObject);
    public FrontalTriggerExit OnFrontalTriggerExit;

    public delegate void RearTriggerExit(Transform crossingObject);
    public RearTriggerExit OnRearTriggerExit;
    
    public delegate void RearToFrontTriggerExit(Transform crossingObject);
    public RearToFrontTriggerExit OnRearToFrontTriggerExit;
    
    public delegate void FrontToRearTriggerExit(Transform crossingObject);
    public FrontToRearTriggerExit OnFrontToRearTriggerExit;

    BoxCollider _myCollider;

    Vector3 _triggerCenterPos;

    #endregion

    #region METHODS

    void OnEnable()
    {
        _myCollider = GetComponent<BoxCollider>();
        _myCollider.isTrigger = true;
    }

    void CheckRelativeToTriggerDirection(Transform otherObjectTrasform, bool objectIsEntering)
    {
        Vector3 posDif = otherObjectTrasform.position - _triggerCenterPos;
        Vector3 relativePos = transform.TransformPoint(otherObjectTrasform.position);

        float accurate = Vector3.Dot(transform.forward, posDif);

        if (objectIsEntering)
        {
            if (accurate < 0)
            {
                OnRearTriggerEnter?.Invoke(otherObjectTrasform);
            }
            else if (accurate > 0)
            {             
                OnFrontalTriggerEnter?.Invoke(otherObjectTrasform);
            }
        }
        else
        {
            if (accurate < 0)
            {
                Debug.Log("BACK");
                OnRearTriggerExit?.Invoke(otherObjectTrasform);
            }
            else if (accurate > 0)
            {
                Debug.Log("FORWARD");
                OnFrontalTriggerExit?.Invoke(otherObjectTrasform);
            }
        }        
    }

    void OnTriggerEnter(Collider other)
    {
        _triggerCenterPos = transform.TransformPoint(_myCollider.center);
        CheckRelativeToTriggerDirection(other.transform, true);
    }

    void OnTriggerExit(Collider other)
    {
        _triggerCenterPos = transform.TransformPoint(_myCollider.center);
        CheckRelativeToTriggerDirection(other.transform, false);
    }

    #endregion
}
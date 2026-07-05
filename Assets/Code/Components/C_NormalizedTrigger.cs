using UnityEngine;

/// <summary>
/// Distinguishes between frontal and rear triggering interactions
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

    BoxCollider _myCollider;

    Vector3 _triggerCenterPos;

    #endregion

    #region METHODS

    void OnEnable()
    {
        _myCollider = GetComponent<BoxCollider>();
        _myCollider.isTrigger = true;
    }

    /// <summary>
    /// Compares the relative position of an external collider to the trigger's center
    /// and then checks how it's aligned with the trigger's forward direction
    /// </summary>
    /// <param name="otherObjectTrasform">External collider's transform</param>
    /// <param name="objectIsEntering">Is the external collider entering or leaving?</param>
    void CheckRelativeToTriggerDirection(Transform otherObjectTrasform, bool objectIsEntering)
    {
        // _triggerCenterPos is converted to world space here so it works even if the trigger is moving
        _triggerCenterPos = transform.TransformPoint(_myCollider.center);
        
        Vector3 posDif = otherObjectTrasform.position - _triggerCenterPos;

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
        CheckRelativeToTriggerDirection(other.transform, true);
    }

    void OnTriggerExit(Collider other)
    {
        CheckRelativeToTriggerDirection(other.transform, false);
    }

    #endregion
}
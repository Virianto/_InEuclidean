using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CNormalizedTrigger : MonoBehaviour
{
    #region ATTRIBUTES

    public delegate void ColliderDetectedFromForward(Transform crossingObject);
    public ColliderDetectedFromForward OnColliderDetectedFromForward;

    public delegate void ColliderDetectedFromBack(Transform crossingObject);
    public ColliderDetectedFromBack OnColliderDetectedFromBack;

    public delegate void ColliderLeftForward(Transform crossingObject);
    public ColliderLeftForward OnColliderLeftForward;

    public delegate void ColliderLeftBack(Transform crossingObject);
    public ColliderLeftBack OnColliderLeftBack;

    BoxCollider myCollider;

    Vector3 triggerCenterPos;

    #endregion

    #region METHODS

    void OnEnable()
    {
        myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
    }

    void CheckRelativeToTriggerDirection(Transform otherObjectTrasform, bool objectIsEntering)
    {
        Vector3 posDif = otherObjectTrasform.position - triggerCenterPos;
        Vector3 relativePos = transform.TransformPoint(otherObjectTrasform.position);

        float accurate = Vector3.Dot(transform.forward, posDif);

        if (objectIsEntering)
        {
            if (accurate < 0)
            {
                OnColliderDetectedFromBack?.Invoke(otherObjectTrasform);
            }
            else if (accurate > 0)
            {             
                OnColliderDetectedFromForward?.Invoke(otherObjectTrasform);
            }
        }
        else
        {
            if (accurate < 0)
            {
                Debug.Log("BACK");
                OnColliderLeftBack?.Invoke(otherObjectTrasform);
            }
            else if (accurate > 0)
            {
                Debug.Log("FORWARD");
                OnColliderLeftForward?.Invoke(otherObjectTrasform);
            }
        }        
    }

    void OnTriggerEnter(Collider other)
    {
        triggerCenterPos = transform.TransformPoint(myCollider.center);
        CheckRelativeToTriggerDirection(other.transform, true);
    }

    void OnTriggerExit(Collider other)
    {
        triggerCenterPos = transform.TransformPoint(myCollider.center);
        CheckRelativeToTriggerDirection(other.transform, false);
    }

    #endregion
}
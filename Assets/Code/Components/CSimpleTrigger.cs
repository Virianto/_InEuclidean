using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class CSimpleTrigger : MonoBehaviour
{
    #region ATTRIBUTES

    public UnityEvent TriggerEnteredEvent;
    public UnityEvent TriggerStayEvent;
    public UnityEvent TriggerLeftEvent;

    public delegate void SimpleTriggerEntered();
    public SimpleTriggerEntered OnSimpleTriggerEntered;

    public delegate void SimpleTriggerStay();
    public SimpleTriggerStay OnSimpleTriggerStay;

    public delegate void SimpleTriggerLeft();
    public SimpleTriggerLeft OnSimpleTriggerLeft;
        
    #endregion

    #region METHODS

    void OnEnable()
    {
        Collider[] allMyColliders = GetComponents<Collider>();

        for(int a = 0; a < allMyColliders.Length; ++a)
        {
            allMyColliders[a].isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        OnSimpleTriggerEntered?.Invoke();
        TriggerEnteredEvent?.Invoke();
    }

    void OnTriggerStay(Collider other)
    {
        OnSimpleTriggerStay?.Invoke();
        TriggerStayEvent?.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        OnSimpleTriggerLeft?.Invoke();
        TriggerLeftEvent?.Invoke();
    }

    #endregion
}
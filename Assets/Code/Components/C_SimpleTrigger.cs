using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows developers to easily manage triggers' events from editor
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class C_SimpleTrigger : MonoBehaviour
{
    #region ATTRIBUTES

    public UnityEvent TriggerEnteredEvent;
    public UnityEvent TriggerStayEvent;
    public UnityEvent TriggerLeftEvent;

    
    public event Action OnSimpleTriggerEntered;

    public event Action OnSimpleTriggerStay;

    public event Action OnSimpleTriggerLeft;
        
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
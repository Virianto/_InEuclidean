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

    public UnityEvent TriggerEnterEvent;
    public UnityEvent TriggerStayEvent;
    public UnityEvent TriggerExitEvent;
    
    public event Action OnSimpleTriggerEnter;

    public event Action OnSimpleTriggerStay;

    public event Action OnSimpleTriggerExit;
        
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
        OnSimpleTriggerEnter?.Invoke();
        TriggerEnterEvent?.Invoke();
    }

    void OnTriggerStay(Collider other)
    {
        OnSimpleTriggerStay?.Invoke();
        TriggerStayEvent?.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        OnSimpleTriggerExit?.Invoke();
        TriggerExitEvent?.Invoke();
    }

    #endregion
}
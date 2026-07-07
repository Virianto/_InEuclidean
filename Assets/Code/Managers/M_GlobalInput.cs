using System;
using Code.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Global Input Manager
/// </summary>
public class M_GlobalInput : Singleton<M_GlobalInput>
{
    #region ATTRIBUTES

         

    #region Events

    // 'event' declaration removes control from other classes (they can not assign or override)
    public event Action<InputActionMap> OnInputActionMapChanged;

    #endregion

    public GlobalInputActions globalInputActions;

    public InputControlScheme currentInputControlScheme;

    #endregion
    
    #region METHODS

    void Awake()
    {
        globalInputActions = new GlobalInputActions();
    }

    void Start()
    {
        globalInputActions.Enable();
        currentInputControlScheme = globalInputActions.KeyboardMouseScheme;
    }

    /// <summary>
    /// Modifies current Action Map active and notifies it to other parts of the code
    /// </summary>
    /// <param name="newInputActionMap">Mapa de acciones que pasará a estar activo si no lo estaba ya</param>
    public void SwitchCurrentActionMap(InputActionMap newInputActionMap)
    {
        if (!newInputActionMap.enabled)
        {
            // With this line we don't disable the entire asset, but all Maps and Actions at once
            globalInputActions.Disable();

            // Guarantee that only this Action Map will be active
            newInputActionMap.Enable();

            // Notify Action Map change to other parts of the code
            OnInputActionMapChanged?.Invoke(newInputActionMap);

            Debug.Log("Current Action Map " + newInputActionMap);
        }
    }

    #endregion
}

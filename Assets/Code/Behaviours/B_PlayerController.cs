using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class makes use of Global Input Manager to handle player input without caring
/// about device type
/// </summary>
public class B_PlayerController : MonoBehaviour
{
    #region ATTRIBUTES

    [Header("Editable values")]
    
    [SerializeField] float _speed = 4;
    
    /// <summary>
    /// This shall help shortening the code
    /// </summary>
    GlobalInputActions.TestingMapActions _testingMapActions;
    
    Vector3 _moveDirection;
    
    #endregion
    
    #region METHODS

    void Start()
    {
        _testingMapActions = M_GlobalInput.Instance.globalInputActions.TestingMap;
        
        _testingMapActions.MainInteraction.performed += MainInteraction;
        _testingMapActions.Move.performed += (c)=>
        {
            Vector2 newDirection = c.ReadValue<Vector2>();
            _moveDirection = new Vector3(newDirection.x, 0, newDirection.y);
        };
    }
    
    void Update()
    {
        transform.Translate(_moveDirection * Time.deltaTime * _speed);
        
        Vector2 rotation = _testingMapActions.Look.ReadValue<Vector2>();
        transform.Rotate(0, rotation.x, 0);       
    }
    
    /// <summary>
    /// This method will be called whenever player presses the Main Interaction button from any
    /// input device (not only keyboard)
    /// </summary>
    /// <param name="c">Contains all info about the input action</param>
    void MainInteraction(InputAction.CallbackContext c)
    {
        Debug.Log("Main Interaction");
    }

    #endregion
}

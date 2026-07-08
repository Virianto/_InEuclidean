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

    /*void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // This code will execute ONLY whenever player presses the 'E' key on Keyboard
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E key pressed");
        }

        // This code will execute ONLY whenever player holds the 'W' key on Keyboard
        if (Keyboard.current.wKey.isPressed)
        {
            Debug.Log("W key is being pressed");           
        }
    }*/

    #endregion
}

using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)] // 가장 먼저 실행을 보장합니다.
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;


    private TouchControls playerControls;

    private void Awake()
    {
        playerControls = new TouchControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    private void Start()
    {
        playerControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        playerControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    }
    private void StartTouch(InputAction.CallbackContext context)
    {
        Debug.Log("Touch Started" + playerControls.Touch.TouchPosition.ReadValue<Vector2>());
        if (OnStartTouch != null) OnStartTouch(playerControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null) OnEndTouch(playerControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
    }


}

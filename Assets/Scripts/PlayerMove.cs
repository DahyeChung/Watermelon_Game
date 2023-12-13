using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Camera cameraMain;

    private void Awake()
    {
        cameraMain = Camera.main;

    }
    private void OnEnable()
    {
        InputManager.Instance.OnStartTouch += Move;
    }
    private void OnDisable()
    {
        InputManager.Instance.OnEndTouch -= Move;
    }

    public void Move(Vector2 screenPosition, float time)
    {
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, cameraMain.nearClipPlane);
        Vector3 worldCoordinates = cameraMain.ScreenToWorldPoint(screenCoordinates);
        worldCoordinates.z = 0;
        transform.position = worldCoordinates;
    }
}

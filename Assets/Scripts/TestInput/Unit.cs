using UnityEngine;

public class Unit : MonoBehaviour
{
    private float speed = 4f;
    [SerializeField]
    private float dropSpeed = -1;
    [SerializeField]
    private LayerMask layerMaskHit;
    [SerializeField]
    private float impactField;
    [SerializeField]
    private float impactForce;

    new Rigidbody2D rigidbody;
    CircleCollider2D circleCollider;
    SpriteRenderer spriteRenderer;
    Touch touch;

    public bool isMerged;
    private bool isTouchStarted = false;
    private bool isMovable = false;
    private bool isDragging = false;
    private float deadTime;

    void Awake()
    {
        this.isMovable = false;
        this.isMerged = false;
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.rigidbody.simulated = false;
        this.rigidbody.velocity = new Vector3(0, dropSpeed, 0);
        this.circleCollider = GetComponent<CircleCollider2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(this);
    }

    private void Update()
    {
        horizontalMove();
    }

    private void horizontalMove()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            Debug.Log(touch);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isTouchStarted = true;
                    break;

                case TouchPhase.Moved:
                    if (isTouchStarted)
                    {
                        this.transform.position = touchPosition;
                    }
                    break;

                case TouchPhase.Ended:
                    if (isTouchStarted)
                    {
                        Drop();
                    }
                    isTouchStarted = false;
                    break;
            }
        }
    }

    private void Drop()
    {
        this.isMovable = false;
        this.rigidbody.simulated = true;
        Debug.Log("드롭발동");
    }
}

using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private float dropSpeed = -1;
    [SerializeField] private float impactField;
    [SerializeField] private float impactForce;
    [SerializeField] private LayerMask layerMaskHit;

    private new Rigidbody2D rigidbody;
    public UnitLevel Level;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private Touch touch;

    public bool isMerged;
    private bool isTouchStarted = false;
    private bool isMovable = false;
    private bool isNext = false;
    public bool IsInit = false;

    private float deadTime;
    private Sprite originalSprite;


    private void Awake()
    {
        isMovable = false;
        isMerged = false;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.simulated = false;
        rigidbody.velocity = new Vector3(0, dropSpeed, 0);
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    public void InitDropUnit(UnitLevel unitLevel)
    {
        Level = unitLevel;
        rigidbody.simulated = false;
        isMovable = true;
        IsInit = true;
        UnitManager.Instance.EnableDropLine();
    }

    public void InitNextUnit(UnitLevel unitLevel)
    {
        this.isNext = true;
        Level = unitLevel;
        this.isMovable = false;
        IsInit = true;
        rigidbody.simulated = false;
    }
    public void InitMergedUnit(UnitLevel unitLevel)
    {
        Level = unitLevel;
        this.isMovable = false;
        IsInit = true;
        rigidbody.simulated = true;
    }

    private void FixedUpdate()
    {
        if (!isNext)
        {
            HorizontalMove();
        }
    }

    private void HorizontalMove()
    {
        if (!isMovable || GameManager_DH.Instance.IsGameOver)
            return;

        if (Input.touchCount <= 0 && this.isTouchStarted == true)
        {
            Drop();
            isTouchStarted = false;
            UnitManager.Instance.DisableDropLine();
        }

        if (Input.touchCount <= 0)
            return;

        touch = Input.GetTouch(0);
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

        // Limit touch top in order to click setting menu
        if (touchPosition.y > 3.5)
            return;

        touchPosition.z = 0;
        touchPosition.y = this.transform.position.y;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                isTouchStarted = true;
                transform.position = touchPosition;
                this.transform.position = new Vector2(touchPosition.x, UnitManager.Instance.dropPosition.position.y);
                break;

            case TouchPhase.Moved:
                if (!isTouchStarted)
                {
                    isTouchStarted = true;
                    transform.position = touchPosition;
                    this.transform.position = new Vector2(touchPosition.x, UnitManager.Instance.dropPosition.position.y);
                    isTouchStarted = true;
                    return;
                }

                transform.position = touchPosition;
                this.transform.position = new Vector2(touchPosition.x, UnitManager.Instance.dropPosition.position.y);
                UnitManager.Instance.MovingDropLine(this.transform);

                break;

            case TouchPhase.Ended:
                Drop();

                isTouchStarted = false;
                UnitManager.Instance.DisableDropLine();
                break;
            default:
                break;
        }

    }

    private void Drop()
    {
        isMovable = false;
        rigidbody.simulated = true;
        UnitManager.Instance.DropComplete();
        if (MenuManager.Instance.canPlaySFX)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        }
        ChangeSprite();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.isNext)
            return;

        if (!collision.collider.CompareTag("Unit"))
            return;
        if (collision.collider.CompareTag("Unit"))
            ChangeSprite();

        Unit otherUnit = collision.gameObject.GetComponent<Unit>();
        if (otherUnit == null)
        {
            Debug.Log("Collision null unit");
            return;
        }

        if (otherUnit.Level != this.Level)
            return;

        if ((!this.IsInit || !otherUnit.IsInit) && this.Level == UnitLevel.Level0)
            return;

        float meX = transform.position.x;
        float meY = transform.position.y;
        float otherX = otherUnit.transform.position.x;
        float otherY = otherUnit.transform.position.y;

        if (!isMerged && !otherUnit.isMerged && Level < UnitLevel.Level10)
        {
            if (meY < otherY || (meY == otherY && meX > otherX))
            {
                Vector2 contactPos = collision.GetContact(0).point;
                Explosion();
                Hide(otherUnit.transform.position);
                otherUnit.Hide(transform.position);
                GenerateNextLevelUnit(contactPos);
            }
        }
    }

    public void Hide(Vector3 targetPos)
    {
        isMerged = true;
        rigidbody.simulated = false;
        circleCollider.enabled = false;
        StartCoroutine(HideRoutine(targetPos));
    }

    private IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
        float mergeForce = 0.1f;

        while (frameCount < 3)
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targetPos, mergeForce);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void GenerateNextLevelUnit(Vector2 contactPos)
    {
        StartCoroutine(LevelUpRoutine(contactPos));
    }

    private IEnumerator LevelUpRoutine(Vector2 contactPos)
    {
        yield return new WaitForSeconds(0.005f);
        UnitManager.Instance.MergeComplete(Level, new Vector3(contactPos.x, contactPos.y, 0));
        if (MenuManager.Instance.canPlaySFX)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.MergeSfx);
        }
    }

    private void Explosion()
    {
        Collider2D[] unitObjs = Physics2D.OverlapCircleAll(transform.position, impactField, layerMaskHit);
        foreach (Collider2D obj in unitObjs)
        {
            Vector2 dir = obj.transform.position - transform.position;
            obj.GetComponent<Rigidbody2D>().AddForce(dir * impactForce * 5);
            Debug.Log("Explode!" + impactForce);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, impactField);
    }

    //-------------------------------------------------------------------------------------------------
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            deadTime += Time.deltaTime;
            if (deadTime > 2)
            {
                spriteRenderer.color = Color.red;
                ChangeSprite();

            }
            if (deadTime > 2.5)
            {
                Destroy(gameObject);
                GameManager_DH.Instance.GameOver();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }
    private void ChangeSprite()
    {
        if (UnitManager.Instance.unitSO == null || (int)Level >= UnitManager.Instance.unitSO.Length || UnitManager.Instance.unitSO[(int)Level].spriteAnimation == null)
        {
            Debug.LogError("Invalid unitSO array or Level." + (int)Level + "Length is " + UnitManager.Instance.unitSO.Length);
            return;
        }

        originalSprite = spriteRenderer.sprite;

        // Change the sprite
        spriteRenderer.sprite = UnitManager.Instance.unitSO[(int)Level].spriteAnimation;
        ;

        // Start a coroutine to revert the sprite after 1 second
        StartCoroutine(RevertSprite(originalSprite, 1.0f));
    }


    private IEnumerator RevertSprite(Sprite originalSprite, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Revert the sprite back to the original sprite
        spriteRenderer.sprite = originalSprite;
    }

}

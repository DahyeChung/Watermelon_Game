using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private float dropSpeed = -1;
    [SerializeField] private float impactField;
    [SerializeField] private float impactForce;
    [SerializeField] private LayerMask layerMaskHit;

    private new Rigidbody2D rigidbody;
    private UnitLevel Level;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private Touch touch;

    public bool isMerged;
    private bool isTouchStarted = false;
    private bool isMovable = false;
    private float deadTime;

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
    public void Init(UnitLevel unitLevel, bool isMerged)
    {
        Level = unitLevel;
        this.isMovable = false;

        if (isMerged)
        {
            rigidbody.simulated = true;
            isMovable = false;
            Debug.Log("create merged Unit");
        }
        else
        {
            rigidbody.simulated = false;
            isMovable = true;
            Debug.Log("create new unit");
        }
    }

    private void FixedUpdate()
    {
        HorizontalMove();
    }

    private void HorizontalMove()
    {
        if (!this.isMovable)
            return;
        if (GameManager.Instance.IsGameOver)
            return;

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;
            touchPosition.y = UnitManager.Instance.unitGroups.transform.position.y;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isTouchStarted = true;
                    break;

                case TouchPhase.Moved:
                    if (isTouchStarted)
                    {
                        transform.position = touchPosition;
                        UnitManager.Instance.dropLine.transform.position = new Vector2(touchPosition.x, 0);
                        //UnitManager.Instance.dropLine.transform.localPosition = new Vector3(0, -4, 0);

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
        isMovable = false;
        rigidbody.simulated = true;
        UnitManager.Instance.dropLine.SetActive(false);
        UnitManager.Instance.DropComplete();
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Unit"))
            return;

        Unit otherUnit = collision.gameObject.GetComponent<Unit>();
        if (otherUnit == null)
        {
            Debug.Log("Collision null unit");
            return;
        }

        if (otherUnit.Level != Level)
            return;

        float meX = transform.position.x;
        float meY = transform.position.y;
        float otherX = otherUnit.transform.position.x;
        float otherY = otherUnit.transform.position.y;

        if (!isMerged && !otherUnit.isMerged && (int)Level < 10)
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

        yield return new WaitForSeconds(0.01f);
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
        UnitManager.Instance.maxLevel = Mathf.Max((int)Level, UnitManager.Instance.maxLevel);
        UnitManager.Instance.MergeComplete(Level + 1, new Vector3(contactPos.x, contactPos.y, 0));
        SoundManager.Instance.PlaySFX(SoundManager.Instance.MergeSfx);
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            Debug.Log("Line Touched");
            deadTime += Time.deltaTime;
            if (deadTime > .5)
            {
                spriteRenderer.color = Color.red;
            }
            if (deadTime > 1)
            {
                Destroy(gameObject);
                GameManager.Instance.GameOver();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }
}

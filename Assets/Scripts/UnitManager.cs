using System.Collections;
using UnityEngine;

public class UnitManager : SingletonMonoBehaviour<UnitManager>
{
    private int MaxLevel = 0;

    public Transform dropPosition;

    [SerializeField] private Transform nextPosition;

    [SerializeField] private UnitScriptableObject[] unitSO;

    [SerializeField] private UIanim uiAnimation;

    public GameObject dropLine;

    private Unit nextUnit;

    private Unit dropUnit;

    private UnitLevel nextUnitLevel = UnitLevel.Level0;

    private bool isDropped = true;

    private void Start()
    {
        if (GameManager.Instance.IsGameOver)
        {
            Debug.Log("game is over");
            return;
        }

        this.DisableDropLine();

        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM);

        var prefab = GetUnitPrefab((int)this.nextUnitLevel);
        if (prefab == null)
        {
            Debug.Log("Start() get unit prefab failed.");
        }

        this.nextUnit = Instantiate(prefab, this.nextPosition).GetComponent<Unit>();
        this.nextUnit.InitNextUnit(this.nextUnitLevel);

        CreateUnit();
    }

    public void MergeComplete(UnitLevel unitLevel, Vector3 position)
    {
        Debug.Log("MergeComplete() " + unitLevel + " " + this.MaxLevel);
        unitLevel += 1;
        this.MaxLevel = Mathf.Max((int)unitLevel, this.MaxLevel);

        Debug.Log("MergeComplete() " + unitLevel + " " + this.MaxLevel);

        var nextLevelPrefab = GetLevelPrefab(unitLevel);
        if (nextLevelPrefab == null)
        {
            Debug.Log("Next level prefab is null");
            return;
        }

        var nextLevelUnit = Instantiate(nextLevelPrefab, position, Quaternion.identity).GetComponent<Unit>();
        if (nextLevelUnit == null)
        {
            Debug.Log("Level up routine next null");
            return;
        }

        nextLevelUnit.InitMergedUnit(unitLevel);

        GameManager.Instance.AddScore(GetLevelScore(unitLevel));
    }

    public void DropComplete()
    {
        // this.DisableDropLine();
        StartCoroutine(PauseFunctionForSeconds(0.5f));
    }

    private IEnumerator PauseFunctionForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isDropped = true;
    }

    private GameObject GetLevelPrefab(UnitLevel level)
    {
        return level > UnitLevel.Level10 ? unitSO[(int)UnitLevel.Level10].UnitPrefabs : unitSO[(int)level].UnitPrefabs;
    }

    private int GetLevelScore(UnitLevel level)
    {
        return unitSO[(int)level].Score;
    }

    private UnitLevel GetNextUnitLevelIndex()
    {
        return (UnitLevel)UnityEngine.Random.Range(0, Mathf.Min(this.MaxLevel, 5));
    }

    private GameObject GetUnitPrefab(int index)
    {
        Debug.Log("get unit prefab : " + index);
        return unitSO[index].UnitPrefabs;
    }

    private void CreateDropUnit()
    {
        var prefab = GetUnitPrefab((int)this.nextUnitLevel);
        if (prefab == null)
        {
            Debug.Log("CreateDropUnit : not found prefab");
            return;
        }

        var unit = Instantiate(prefab, this.dropPosition).GetComponent<Unit>();
        if (unit == null)
        {
            Debug.Log("CreateDropUnit : unit instantiate failed.");
            return;
        }

        unit.InitDropUnit(this.nextUnitLevel);

        // Debug.Log("enable drop line");
        // this.EnableDropLine(unit.transform);

        if (uiAnimation == null)
            Debug.Log("UnitManager.uiAnimation is null");

        uiAnimation.ScaleAnim(unit.gameObject, 1.5f);
        this.dropLine.transform.localScale = Vector3.one;

        Debug.Log("created drop unit");
    }

    private void CreateNextUnit()
    {
        this.nextUnitLevel = GetNextUnitLevelIndex();
        Debug.Log("CreateNextUnit().this.nextUnitLevel : " + this.nextUnitLevel);

        Destroy(this.nextUnit.gameObject);

        var prefab = GetUnitPrefab((int)this.nextUnitLevel);
        if (prefab == null)
        {
            Debug.Log("CreateNextUnit : not found prefab");
            return;
        }
        Debug.Log("CreateNextUnit() 2");
        this.nextUnit = Instantiate(prefab, this.nextPosition).GetComponent<Unit>();
        if (this.nextUnit == null)
        {
            Debug.Log("CreateNextUnit : unit instantiate failed.");
            return;
        }
        Debug.Log("CreateNextUnit() 3");
        this.nextUnit.InitNextUnit(this.nextUnitLevel);
        Debug.Log("CreateNextUnit() 4");
        Debug.Log("created next unit");
    }

    private void CreateUnit()
    {
        this.CreateDropUnit();
        this.CreateNextUnit();
        this.isDropped = false;
        StartCoroutine(WaitNext());
    }

    private IEnumerator WaitNext()
    {
        while (!isDropped)
            yield return new WaitForSeconds(0.5f);

        CreateUnit();
    }

    public void EnableDropLine()
    {
        this.dropLine.SetActive(true);
        this.dropLine.transform.position = this.dropPosition.position; // new Vector2(position.position.x, this.dropPosition.position.y);
        // this.dropLine.transform.localPosition = new Vector3(0, -3, 0);
    }

    public void MovingDropLine(Transform position)
    {
        this.dropLine.transform.position = new Vector2(position.position.x, this.dropPosition.position.y);
        // this.dropLine.transform.localPosition = new Vector3(0, -3, 0);
    }

    public void DisableDropLine()
    {
        this.dropLine.transform.SetParent(this.dropPosition);
        this.dropLine.SetActive(false);
    }
}

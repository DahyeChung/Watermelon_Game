using System.Collections;
using UnityEngine;

public class UnitManager : SingletonMonoBehaviour<UnitManager>
{
    private int MaxLevel = 0;

    public Transform dropPosition;

    [SerializeField] private Transform nextPosition;

    [SerializeField] private UnitScriptableObject[] unitSO;

    [SerializeField] private MenuManager Menu;

    public GameObject dropLine;

    private Unit nextUnit;

    private UnitLevel nextUnitLevel = UnitLevel.Level0;

    private bool isDropped = true;

    private void Start()
    {
        if (GameManager_DH.Instance.IsGameOver)
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
        unitLevel += 1;
        this.MaxLevel = Mathf.Max((int)unitLevel, this.MaxLevel);
        //Max Level
        //if (this.MaxLevel < 3)
        //    this.MaxLevel = Mathf.Max((int)unitLevel, this.MaxLevel);

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
        if (unitLevel != this.unitSO[(int)unitLevel].UnitLevel)
        {
            Debug.Log("error");
        }

        GameManager_DH.Instance.AddScore(GetLevelScore(unitLevel));
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
        return (UnitLevel)UnityEngine.Random.Range(0, 5);
        //return (UnitLevel)UnityEngine.Random.Range(0, this.MaxLevel + 2);
    }

    private GameObject GetUnitPrefab(int index)
    {
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

        if (Menu == null)
            Debug.Log("UnitManager.uiAnimation is null");

        Menu.ScaleAnim(unit.gameObject, 1.5f);
        this.dropLine.transform.localScale = Vector3.one;
    }

    private void CreateNextUnit()
    {
        this.nextUnitLevel = GetNextUnitLevelIndex();

        Destroy(this.nextUnit.gameObject);

        var prefab = GetUnitPrefab((int)this.nextUnitLevel);
        if (prefab == null)
        {
            Debug.Log("CreateNextUnit : not found prefab");
            return;
        }

        this.nextUnit = Instantiate(prefab, this.nextPosition).GetComponent<Unit>();
        if (this.nextUnit == null)
        {
            Debug.Log("CreateNextUnit : unit instantiate failed.");
            return;
        }

        this.nextUnit.InitNextUnit(this.nextUnitLevel);

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
        this.dropLine.transform.position = new Vector2(this.dropPosition.position.x, this.dropPosition.position.y - 3);
    }
    public void MovingDropLine(Transform position)
    {
        this.dropLine.transform.position = new Vector2(position.position.x, this.dropPosition.position.y - 3);
    }

    public void DisableDropLine()
    {
        this.dropLine.transform.SetParent(this.dropPosition);
        this.dropLine.SetActive(false);
    }
}

using System.Collections;
using UnityEngine;

public class UnitManager : SingletonMonoBehaviour<UnitManager>
{
    [SerializeField] private Transform unitGroups;
    [SerializeField] private Unit myUnit;
    [SerializeField] private UnitScriptableObject[] mySO;

    public int maxLevel;
    private bool isDropped = true;

    private void Start()
    {
        if (!GameManager.Instance.IsGameOver)
            NextUnit();
    }

    public void MergeComplete(UnitLevel unitLevel, Vector3 position)
    {
        var nextLevelPrefab = LevelPrefab(unitLevel);
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

        nextLevelUnit.Init(unitLevel, true);

        GameManager.Instance.AddScore((int)Mathf.Pow(LevelScore(unitLevel), 1));
    }

    public void DropComplete()
    {
        StartCoroutine(PauseFunctionForSeconds(0.5f));
    }

    private IEnumerator PauseFunctionForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isDropped = true;
    }

    //----------------Unit Random Calculator---------------------------------
    public GameObject LevelPrefab(UnitLevel level)
    {
        return level > UnitLevel.Level10 ?
               mySO[(int)UnitLevel.Level10].UnitPrefabs : mySO[(int)level].UnitPrefabs;
    }

    private int LevelScore(UnitLevel level)
    {
        return mySO[(int)level].Score;
    }

    private int GetNextUnitLevelIndex()
    {
        return UnityEngine.Random.Range(0, Mathf.Min(maxLevel, 5));
    }

    private GameObject GetUnitPrefab(int index)
    {
        return mySO[index].UnitPrefabs;
    }

    //----------------Unit Generator-----------------------------------------
    private Unit GetUnit()
    {
        int unitRandomIndex = GetNextUnitLevelIndex();
        if (unitRandomIndex == -1)
            return null;

        isDropped = false;

        var unitPrefabs = GetUnitPrefab(unitRandomIndex);

        var unitInstance = Instantiate(unitPrefabs, unitGroups).GetComponent<Unit>();
        if (unitInstance == null)
            return null;

        unitInstance.Init(mySO[unitRandomIndex].UnitLevel, false);
        return unitInstance;
    }

    private void NextUnit()
    {
        Unit unit = GetUnit();

        StartCoroutine(WaitNext());
    }

    private IEnumerator WaitNext()
    {
        Debug.Log("WaitNext: " + isDropped);
        while (!isDropped)
            yield return new WaitForSeconds(0.5f);

        NextUnit();
    }

}

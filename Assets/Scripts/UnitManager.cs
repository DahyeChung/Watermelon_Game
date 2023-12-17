using System.Collections;
using UnityEngine;

public class UnitManager : SingletonMonoBehaviour<UnitManager>
{
    public Transform unitGroups;
    public GameObject prefabDisplay;
    [SerializeField] private Unit myUnit;
    [SerializeField] private UnitScriptableObject[] mySO;

    public GameObject dropLine;
    public UIanim UIanim;
    public int maxLevel;

    private int nextRandomIndex = +1;
    private bool isDropped = true;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM);
        dropLine.SetActive(false);
        if (!GameManager.Instance.IsGameOver)
        {
            NextUnit();
            SetupPrefabDisplay();
        }

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
        int newRandomIndex = GetNextUnitLevelIndex();
        if (newRandomIndex >= 0 && newRandomIndex < mySO.Length) // 유효한 범위 내의 인덱스인지 검사
        {
            nextRandomIndex = newRandomIndex; // 현재 랜덤 인덱스를 이전에 사용한 랜덤 인덱스로 저장
            SetupPrefabDisplay(); // UI 디스플레이 업데이트
        }
        else
        {
            Debug.LogError("Invalid random index: " + newRandomIndex);
        }
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

        //var unitInstance = Instantiate(unitPrefabs, prefabDisplay);
        var unitInstance = Instantiate(unitPrefabs, unitGroups).GetComponent<Unit>();
        if (unitInstance == null)
            return null;

        unitInstance.Init(mySO[unitRandomIndex].UnitLevel, false);
        //NextPrefabDisplay(unitRandomIndex);

        DropLineActive();
        UIanim.ScaleAnim(unitInstance.gameObject, 1.5f);

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

    private void DropLineActive()
    {
        dropLine.SetActive(true);
        dropLine.transform.SetParent(unitGroups.transform);
        dropLine.transform.localPosition = new Vector3(0, -3, 0);
    }
    private void SetupPrefabDisplay()
    {
        if (prefabDisplay != null)
        {
            var unitPrefab = GetUnitPrefab(nextRandomIndex); // 이전에 사용한 랜덤 인덱스에 해당하는 프리팹을 가져옴
            if (unitPrefab != null)
            {
                Instantiate(unitPrefab, prefabDisplay.transform); // 프리팹을 prefabDisplay의 자식으로 생성
            }
        }
    }
}

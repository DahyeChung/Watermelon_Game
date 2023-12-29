using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : SingletonMonoBehaviour<UnitManager>
{
    public UnitScriptableObject[] unitSO;
    public GameObject dropLine;
    public Transform dropPosition;

    [SerializeField] private Transform previewPosition;
    [SerializeField] private MenuManager Menu;
    private Dictionary<int, UnitLevel> unitRandom = new();
    private Unit previewUnit;
    private UnitLevel previewUnitLevel = UnitLevel.Level0;
    private bool isDropped = true;

    private void Start()
    {
        Application.targetFrameRate = 60;
        MenuManager.Instance.canPlaySFX = true;


        if (GameManager_DH.Instance.IsGameOver)
            return;

        int percent = 0;
        foreach (var unit in unitSO)
        {
            if (!unit.canCreate)
                continue;

            percent += unit.createPercent;
            unitRandom.Add(percent, unit.unitLevel);
        }

        this.DisableDropLine();

        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM);

        var prefab = GetUnitPrefab((int)this.previewUnitLevel);
        if (prefab == null)
        {
            Debug.Log("Start() get unit prefab failed.");
        }

        this.previewUnit = Instantiate(prefab, this.previewPosition).GetComponent<Unit>();
        this.previewUnit.InitNextUnit(this.previewUnitLevel);

        CreateUnit();
    }
    /// <summary>
    /// 기능 : 충돌완료 시 다음레벨유닛을 생성하고 Unit스트립트 컴포넌트를 담습니다.
    /// </summary>
    /// <param name="unitLevel">스크립터블 오브젝트에 담긴 유닛 별 레벨 정보. 타입 : Enum UnitLevel </param>
    /// <param name="position">충돌 포지션을 담아주세요. 다음레벨 생성위치가 됩니다. </param>
    /// <returns>  </returns>
    public void MergeComplete(UnitLevel unitLevel, Vector3 position)
    {
        unitLevel += 1;
        // 생성할 다음 레벨의 프리팹 할당
        var nextLevelPrefab = GetLevelPrefab(unitLevel);

        if (nextLevelPrefab == null)
        {
            Debug.Log("Next level prefab is null");
            return;
        }
        // 프리팹 생성 후 Unit 기능 부여
        var nextLevelUnit = Instantiate(nextLevelPrefab, position, Quaternion.identity).GetComponent<Unit>();
        if (nextLevelUnit == null)
        {
            Debug.Log("Level up routine next null");
            return;
        }

        nextLevelUnit.InitMergedUnit(unitLevel, this.GetLevelSprite(unitLevel));
        if (unitLevel != this.unitSO[(int)unitLevel].unitLevel)
        {
            Debug.Log("error");
        }
        GameManager_DH.Instance.AddScore(GetLevelScore(unitLevel));
    }
    /// <summary>
    /// 기능 : 드롭 완료 후 0.5초의 대기를 보장합니다.
    /// </summary>
    public void DropComplete()
    {
        StartCoroutine(PauseFunctionForSeconds(0.5f));
    }
    private IEnumerator PauseFunctionForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isDropped = true;
    }
    /// <summary>
    /// 기능 : 원하는 레벨에 맞는 프리팹 형태를 가져옵니다.
    /// </summary>
    /// <param name="level">대상 유닛의 레벨 SO를 받습니다.</param>
    /// <returns>"GameObject" 타입 프리팹 반환</returns>
    private GameObject GetLevelPrefab(UnitLevel level)
    {
        return level > UnitLevel.Level10 ? unitSO[(int)UnitLevel.Level10].unitPrefabs : unitSO[(int)level].unitPrefabs;
    }
    /// <summary>
    /// 기능 : 원하는 레벨에 맞는 스프라이트 이미지 SO를 가져옵니다.
    /// </summary>
    /// <param name="level"></param>
    /// <returns>"Sprite" 타입의 이미지 반환</returns>
    private Sprite GetLevelSprite(UnitLevel level)
    {
        return level > UnitLevel.Level10 ? unitSO[(int)UnitLevel.Level10].spriteAnimation : unitSO[(int)level].spriteAnimation;
    }
    /// <summary>
    /// 기능 : 원하는 레벨에 맞는 스코어 정보 SO를 가져옵니다.
    /// </summary>
    /// <param name="level"></param>
    /// <returns>"int" 타입의 점수 반환</returns>
    private int GetLevelScore(UnitLevel level)
    {
        return unitSO[(int)level].score;
    }
    /// <summary>
    /// 기능 : 유닛의 레벨을 랜덤으로 결정합니다.
    /// </summary>
    /// <returns>"UnitLevel" 타입의 레벨정보를 반환합니다. (Enum)</returns>
    private UnitLevel GetNextUnitLevelIndex()
    {
        int random = Random.Range(0, 100);
        foreach (var dic in unitRandom)
        {
            if (dic.Key >= random)
                return dic.Value;
        }
        return UnitLevel.Level0;
    }
    /// <summary>
    /// 기능 : "Int"타입의 Index에 맞는 프리팹 SO를 가져옵니다.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>"GameObject" 타입의 프리팹을 반환합니다.</returns>

    private GameObject GetUnitPrefab(int index)
    {
        return unitSO[index].unitPrefabs;
    }
    /// <summary>
    /// 기능 : 
    /// </summary>
    private void CreateDropUnit()
    {
        var prefab = GetUnitPrefab((int)this.previewUnitLevel);
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

        unit.InitDropUnit(this.previewUnitLevel, this.GetLevelSprite(this.previewUnitLevel));

        if (Menu == null)
            Debug.Log("UnitManager.uiAnimation is null");

        Menu.UnitScaleAnim(unit.gameObject, 1.5f);
        this.dropLine.transform.localScale = Vector3.one;
    }

    private void CreateNextUnit()
    {
        this.previewUnitLevel = GetNextUnitLevelIndex();

        Destroy(this.previewUnit.gameObject);

        var prefab = GetUnitPrefab((int)this.previewUnitLevel);
        if (prefab == null)
        {
            Debug.Log("CreateNextUnit : not found prefab");
            return;
        }

        this.previewUnit = Instantiate(prefab, this.previewPosition).GetComponent<Unit>();
        if (this.previewUnit == null)
        {
            Debug.Log("CreateNextUnit : unit instantiate failed.");
            return;
        }

        this.previewUnit.InitNextUnit(this.previewUnitLevel);

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
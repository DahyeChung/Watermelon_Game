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
    /// ��� : �浹�Ϸ� �� �������������� �����ϰ� Unit��Ʈ��Ʈ ������Ʈ�� ����ϴ�.
    /// </summary>
    /// <param name="unitLevel">��ũ���ͺ� ������Ʈ�� ��� ���� �� ���� ����. Ÿ�� : Enum UnitLevel </param>
    /// <param name="position">�浹 �������� ����ּ���. �������� ������ġ�� �˴ϴ�. </param>
    /// <returns>  </returns>
    public void MergeComplete(UnitLevel unitLevel, Vector3 position)
    {
        unitLevel += 1;
        // ������ ���� ������ ������ �Ҵ�
        var nextLevelPrefab = GetLevelPrefab(unitLevel);

        if (nextLevelPrefab == null)
        {
            Debug.Log("Next level prefab is null");
            return;
        }
        // ������ ���� �� Unit ��� �ο�
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
    /// ��� : ��� �Ϸ� �� 0.5���� ��⸦ �����մϴ�.
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
    /// ��� : ���ϴ� ������ �´� ������ ���¸� �����ɴϴ�.
    /// </summary>
    /// <param name="level">��� ������ ���� SO�� �޽��ϴ�.</param>
    /// <returns>"GameObject" Ÿ�� ������ ��ȯ</returns>
    private GameObject GetLevelPrefab(UnitLevel level)
    {
        return level > UnitLevel.Level10 ? unitSO[(int)UnitLevel.Level10].unitPrefabs : unitSO[(int)level].unitPrefabs;
    }
    /// <summary>
    /// ��� : ���ϴ� ������ �´� ��������Ʈ �̹��� SO�� �����ɴϴ�.
    /// </summary>
    /// <param name="level"></param>
    /// <returns>"Sprite" Ÿ���� �̹��� ��ȯ</returns>
    private Sprite GetLevelSprite(UnitLevel level)
    {
        return level > UnitLevel.Level10 ? unitSO[(int)UnitLevel.Level10].spriteAnimation : unitSO[(int)level].spriteAnimation;
    }
    /// <summary>
    /// ��� : ���ϴ� ������ �´� ���ھ� ���� SO�� �����ɴϴ�.
    /// </summary>
    /// <param name="level"></param>
    /// <returns>"int" Ÿ���� ���� ��ȯ</returns>
    private int GetLevelScore(UnitLevel level)
    {
        return unitSO[(int)level].score;
    }
    /// <summary>
    /// ��� : ������ ������ �������� �����մϴ�.
    /// </summary>
    /// <returns>"UnitLevel" Ÿ���� ���������� ��ȯ�մϴ�. (Enum)</returns>
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
    /// ��� : "Int"Ÿ���� Index�� �´� ������ SO�� �����ɴϴ�.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>"GameObject" Ÿ���� �������� ��ȯ�մϴ�.</returns>

    private GameObject GetUnitPrefab(int index)
    {
        return unitSO[index].unitPrefabs;
    }
    /// <summary>
    /// ��� : 
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
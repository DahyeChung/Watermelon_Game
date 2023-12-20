using System.Collections;
using UnityEngine;

public class UnitManager : SingletonMonoBehaviour<UnitManager>
{
    private int MaxLevel = 0;

    /// <summary>
    /// Dropping position
    /// </summary>
    [SerializeField] private Transform dropPosition;

    /// <summary>
    /// Next unit position
    /// </summary>
    [SerializeField] private Transform nextPosition;

    /// <summary>
    /// Prefep information for each unit (scriptable object)
    /// </summary>
    [SerializeField] private UnitScriptableObject[] unitSO;

    /// <summary>
    /// Drop Line
    /// </summary>
    [SerializeField] private GameObject dropLine;

    /// <summary>
    /// the unit that will drop after the drop unit.
    /// </summary>
    private Unit nextUnit;

    /// <summary>
    /// drop unit
    /// </summary>
    private Unit dropUnit;

    /// <summary>
    /// the level of the next unit to be created.
    /// </summary>
    private UnitLevel nextUnitLevel = UnitLevel.Level0;

    /// <summary>
    /// Animation displayed in UI
    /// </summary>
    private UIanim uiAnimation;

    /// <summary>
    /// It is true if the drop has been completed.
    /// </summary>
    private bool isDropped = true;

    private void Start()
    {
        // Checking game over
        if (GameManager.Instance.IsGameOver)
        {
            Debug.Log("game is over");
            return;
        }

        // At first you won't see any drop lines.
        this.DisableDropLine();

        // When the game starts, it plays a sound.
        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM);

        // first next level object
        var prefab = GetUnitPrefab((int)this.nextUnitLevel);
        this.nextUnit = Instantiate(prefab, this.dropPosition).GetComponent<Unit>();
        this.nextUnit.InitNextUnit(this.nextUnitLevel);

        // start coroutine
        CreateUnit();
    }

    /// <summary>
    /// When two units meet and merge, the unit calls this function and creates a new unit in its place.
    /// </summary>
    /// <param name="unitLevel"> unit level + 1 </param>
    /// <param name="position"> created position </param>
    public void MergeComplete(UnitLevel unitLevel, Vector3 position)
    {
        // change max level
        this.MaxLevel = Mathf.Max((int)unitLevel, this.MaxLevel);
        unitLevel += 1;

        // level to prefab
        var nextLevelPrefab = GetLevelPrefab(unitLevel);
        if (nextLevelPrefab == null)
        {
            Debug.Log("Next level prefab is null");
            return;
        }

        // created next level
        var nextLevelUnit = Instantiate(nextLevelPrefab, position, Quaternion.identity).GetComponent<Unit>();
        if (nextLevelUnit == null)
        {
            Debug.Log("Level up routine next null");
            return;
        }

        // initialize unit
        nextLevelUnit.InitMergedUnit(unitLevel);

        // add score
        GameManager.Instance.AddScore(GetLevelScore(unitLevel));
    }

    /// <summary>
    /// Enable isDropped after 0.5 seconds.
    /// </summary>
    public void DropComplete()
    {
        this.DisableDropLine();
        StartCoroutine(PauseFunctionForSeconds(0.5f));
    }

    /// <summary>
    /// coroutine DropComplete
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator PauseFunctionForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isDropped = true;
    }

    //----------------Unit Random Calculator---------------------------------

    /// <summary>
    /// Unit Level to Prefab
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private GameObject GetLevelPrefab(UnitLevel level)
    {
        return (int)level >= this.MaxLevel ? unitSO[(int)UnitLevel.Level10].UnitPrefabs : unitSO[(int)level].UnitPrefabs;
    }

    /// <summary>
    /// Returns unitLevel to score
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private int GetLevelScore(UnitLevel level)
    {
        return unitSO[(int)level].Score;
    }

    /// <summary>
    /// Returns a random value from 0 to 5
    /// </summary>
    /// <returns></returns>
    private UnitLevel GetNextUnitLevelIndex()
    {
        return (UnitLevel)UnityEngine.Random.Range(0, Mathf.Min(this.MaxLevel, 5));
    }

    /// <summary>
    /// index to unitPrefab
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private GameObject GetUnitPrefab(int index)
    {
        Debug.Log("get unit prefab : " + index);
        return unitSO[index].UnitPrefabs;
    }

    //----------------Unit Generator-----------------------------------------

    /// <summary>
    /// create drop unit
    /// </summary>
    private void CreateDropUnit()
    {
        // Get prefab based on the following unit levels
        var prefab = GetUnitPrefab((int)this.nextUnitLevel);
        if (prefab != null)
        {
            Debug.Log("CreateDropUnit : not found prefab");
            return;
        }

        // Create Drop Unit
        var unit = Instantiate(prefab, this.dropPosition).GetComponent<Unit>();
        if (unit == null)
        {
            Debug.Log("CreateDropUnit : unit instantiate failed.");
            return;
        }

        // Init Drop Unit
        unit.InitDropUnit(this.nextUnitLevel);

        // Enable Drop Line using Drop Unit Transform
        this.EnableDropLine(unit.transform);

        // Animation
        if (uiAnimation == null)
            Debug.Log("UnitManager.uiAnimation is null");

        uiAnimation.ScaleAnim(unit.gameObject, 1.5f);
    }

    /// <summary>
    /// Create Next Level and Next Unit
    /// </summary>
    private void CreateNextUnit()
    {
        // Create random next level
        this.nextUnitLevel = GetNextUnitLevelIndex();

        // Destroy previous unit
        if (this.nextUnit)
            Destroy(this.nextUnit);

        // Get prefab based on the following unit levels
        var prefab = GetUnitPrefab((int)this.nextUnitLevel);
        if (prefab != null)
        {
            Debug.Log("CreateNextUnit : not found prefab");
            return;
        }

        // Create Next Unit
        this.nextUnit = Instantiate(prefab, this.nextPosition).GetComponent<Unit>();
        if (this.nextUnit == null)
        {
            Debug.Log("CreateNextUnit : unit instantiate failed.");
            return;
        }

        // Init Next Unit
        this.nextUnit.InitNextUnit(this.nextUnitLevel);
    }

    /// <summary>
    /// Create drop unit and next unit
    /// </summary>
    private void CreateUnit()
    {
        // Create drop unit
        this.CreateDropUnit();

        // create next unit
        this.CreateNextUnit();

        // Make isDropped false to wait for DropComplete() to be called again.
        this.isDropped = false;
        StartCoroutine(WaitNext());
    }

    /// <summary>
    /// Wait for DropComplete() to be called again.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitNext()
    {
        // Wait for DropComplete() to be called again.
        while (!isDropped)
            yield return new WaitForSeconds(0.5f);

        // Create Unit
        CreateUnit();
    }

    /// <summary>
    /// Enable drop line
    /// </summary>
    private void EnableDropLine(Transform position)
    {
        this.dropLine.SetActive(true);
        this.dropLine.transform.SetParent(position);
        this.dropLine.transform.localPosition = new Vector3(0, -3, 0);
    }

    /// <summary>
    /// Disable drop line
    /// </summary>
    private void DisableDropLine()
    {
        this.dropLine.SetActive(false);
    }
}

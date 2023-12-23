using UnityEngine;

public enum UnitLevel
{
    Level0,
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level7,
    Level8,
    Level9,
    Level10,
}


[CreateAssetMenu]
public class UnitScriptableObject : ScriptableObject
{
    public UnitLevel unitLevel;
    public GameObject unitPrefabs;
    public Sprite spriteAnimation;
    public int score;
    public int createPercent;
    public bool canCreate;
}


using UnityEngine;

public enum PatternPage
{
    Page01,
    Page02,
    EveryPage
}

[System.Serializable]
public class BossPatternData
{
    public string BossPatternName;
    public string BossName;
    public PatternPage currentPage;
}

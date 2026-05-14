using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "BattleCharacter/Player")]
public class PlayerSO : ScriptableObject
{
    public string playerName;
    public int playerMaxHelth;
    public int playerDamage;
    public int playerSpeed;
    public Sprite Skill01Image;
    public Sprite Skill02Image;
    public Sprite Skill03Image;
    public Sprite Skill04Image;
    public string SkillAttackType02;
    public string SkillAttackType03;
    public string SkillAttackType04;
}

using UnityEngine;

public class SkillHUDManager : MonoBehaviour
{
    [Header("스킬 슬롯 연결")]
    public SkillSlotUI slotBasicAttack;         // 좌클릭
    public SkillSlotUI slotMelee;               // G  근접공격
    public SkillSlotUI slotParry;               // 우클릭+G  패링
    public SkillSlotUI slotDash;                // Shift  대시
    public SkillSlotUI slotThrow;               // E  투척
    public SkillSlotUI slotReload;              // R  재장전

    private static SkillHUDManager instance;
    public static SkillHUDManager Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    // PlayerAttackSystem에서 호출
    public void TriggerCooldown(SkillType type, float cooldown)
    {
        switch (type)
        {
            case SkillType.BasicAttack: slotBasicAttack?.StartCooldown(cooldown); break;
            case SkillType.Melee: slotMelee?.StartCooldown(cooldown); break;
            case SkillType.Parry: slotParry?.StartCooldown(cooldown); break;
            case SkillType.Dash: slotDash?.StartCooldown(cooldown); break;
            case SkillType.Throw: slotThrow?.StartCooldown(cooldown); break;
            case SkillType.Reload: slotReload?.StartCooldown(cooldown); break;
        }
    }
}

public enum SkillType
{
    BasicAttack,
    Melee,
    Parry,
    Dash,
    Throw,
    Reload
}
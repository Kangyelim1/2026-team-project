using System;
using UnityEngine;

[Serializable]
public class ItemData 
{
    [Header("기본정보")]
    public int ItemID;
    public string ItemName;
    public string ItemType;
    public string SlotType;
    public string Rarity;
    public string Description;

    [Header("장비 스텟")]
    public int EquipHP;
    public int EquipAttack;
    public int EquipDefence;
    public int EquipSpeed;
    public int EquipAP;

    [Header("VFX")]
    public string SpecialEffect;
    public float EffectValue;
    public float EffectDuration;

    [Header("아이템 설정")]
    public bool CanStack;
    public int MaxStack;

    [Header("가격 / 회금")]
    public int Price;
    public int UnlockQuestID;

    [Header("리소스")]
    public string SpriteName;
}

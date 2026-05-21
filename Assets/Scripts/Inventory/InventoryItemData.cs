using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem",
    menuName = "Inventory/Item")]
public class InventoryItemData : ScriptableObject
{
    [Header("아이템 정보")]
    public string itemName;

    [TextArea]
    public string itemDescription;

    public Sprite itemIcon;

    // =========================
    // 아이템 종류
    // =========================
    public InventoryItemType itemType;

    public int attackPower;
    public int healthPower;
}
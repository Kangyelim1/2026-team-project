using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : MonoBehaviour
{


    public Image itemIcon;
    public TMP_Text itemNameText;

    private InventoryItemData itemData;

    [Header("장착 테두리")]
    public Image equipOutline;

    public InventoryItemData GetItemData()
    {
        return itemData;
    }

    public void Setup(InventoryItemData data)
    {
        itemData = data;

        // 아이콘 표시
        itemIcon.sprite = data.itemIcon;

        itemNameText.text = data.itemName;

        // =========================
        // 버튼 클릭 이벤트 연결
        // =========================
        GetComponent<Button>().onClick
            .AddListener(OnClickItem);

        // 시작 시 꺼두기
        equipOutline.gameObject.SetActive(false);
    }

    // =========================
    // 아이템 클릭
    // =========================
    void OnClickItem()
    {
        InventoryUIManager.Instance
            .ToggleEquipItem(itemData);
    }

    // =========================
    // 장착 표시
    // =========================
    public void SetEquipped(bool isEquipped)
    {
        equipOutline.gameObject
            .SetActive(isEquipped);
    }
}
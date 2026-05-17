using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{

    
    

    // 아이템 데이터
    public ShopItemData itemData;
    

    // 아이템 이미지
    public Image itemImage;

    // 가격 텍스트
    public TMP_Text priceText;

    // 아이템 설정
    public void Setup(ShopItemData data)
    {
        itemData = data;

        itemImage.sprite = data.itemIcon;
        
        priceText.text = data.itemPrice.ToString();

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Debug.Log(itemData.itemName + " 클릭");

        ShopUIManager.Instance.OpenBuyPanel(itemData);
    }

    // 버튼 클릭
    public void OpenBuyPanel()
    {
        ShopUIManager.Instance.OpenBuyPanel(itemData);
    }
}
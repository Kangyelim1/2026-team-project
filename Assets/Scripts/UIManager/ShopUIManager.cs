using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour
{
    
    public static ShopUIManager Instance;

    [Header("전체 상점 패널")]
    public GameObject shopRootPanel;

    [Header("상점 메인 패널")]
    public GameObject shopPanel;

    [Header("구매 패널")]
    public GameObject buyPanel;

    [Header("재화 UI")]
    public TMP_Text goldText;

    [Header("경고 텍스트")]
    public GameObject warningText;

    [Header("구매창 UI")]
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemPriceText;

    public Image itemIconImage;

    [Header("아이템 버튼 프리팹")]
    public GameObject itemButtonPrefab;

    public Transform contentParent;

    public List<ShopItemData> shopItems = new List<ShopItemData>();

    // 현재 골드
    public int currentGold = 1000;

    // 현재 선택 아이템
    private ShopItemData currentItem;

    // 현재 상점
    private Interactable currentShop;

    // 플레이어
    private Transform player;

    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Player 찾기
        player = GameObject.FindGameObjectWithTag("Player").transform;

        shopRootPanel.SetActive(false);

        // 시작 시 패널 끄기
        shopPanel.SetActive(false);
        buyPanel.SetActive(false);

        warningText .SetActive(false);

        // 골드 표시
        UpdateGoldUI();

        CreateItemButtons();
    }

    void CreateItemButtons()
    {
        Debug.Log("상점 아이템 버튼 생성 시작");

        foreach (ShopItemData item in shopItems)
        {
            // 프리팹 생성
            GameObject buttonObj = Instantiate(
                itemButtonPrefab,
                contentParent
            );

            // ShopItemButton 스크립트 가져오기
            ShopItemButton button =
                buttonObj.GetComponent<ShopItemButton>();

            // 데이터 연결
            button.Setup(item);

            Debug.Log(item.itemName + " 버튼 생성 완료");
        }
    }

    private void Update()
    {
        
    }

    // 상점 열기
    public void OpenShopUI(Interactable shop)
    {
        currentShop = shop;

        shopRootPanel.SetActive(true);

        shopPanel.SetActive(true);
        buyPanel.SetActive(false);

        Debug.Log("상점 UI 열림");
    }

    // 상점 닫기
    public void CloseShopUI()
    {
        currentShop = null;

        shopRootPanel.SetActive(false);
        shopPanel.SetActive(false);
        buyPanel.SetActive(false);

        Debug.Log("상점 UI 닫힘");
    }

    // 구매창 열기
    public void OpenBuyPanel(ShopItemData item)
    {
        currentItem = item;
        

        buyPanel.SetActive(true);

        // UI 표시
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDescription;
        itemPriceText.text = item.itemPrice.ToString();

        itemIconImage.sprite = item.itemIcon;

        Debug.Log("구매창 열림");
    }

    // 구매창 닫기
    public void CloseBuyPanel()
    {
        // 구매창 끄기
        buyPanel.SetActive(false);

        
    }

    // 구매 버튼
    public void BuyItem()
    {
        // 골드 부족 체크
        if (currentGold < currentItem.itemPrice)
        {
            Debug.Log("골드 부족");
            StartCoroutine(ShowWarningText());
            return;
        }

        // 골드 차감
        currentGold -= currentItem.itemPrice;

        // UI 갱신
        UpdateGoldUI();

        Debug.Log(currentItem.itemName + " 구매 완료");

        // 구매창 닫기
        CloseBuyPanel();
    }

    // 골드 UI 갱신
    void UpdateGoldUI()
    {
        goldText.text = currentGold.ToString();
    }

    System.Collections.IEnumerator ShowWarningText()
    {
        // 텍스트 켜기
        warningText.SetActive(true);

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 텍스트 끄기
        warningText.SetActive(false);
    }
}
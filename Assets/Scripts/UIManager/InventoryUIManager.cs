using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    
    public static InventoryUIManager Instance;

    [Header("전체 인벤토리 패널")]
    public GameObject inventoryRootPanel;

    [Header("인벤토리 메인 패널")]
    public GameObject inventoryPanel;

    [Header("플레이어 정보")]
    public TMP_Text attackText;
    public TMP_Text hpText;

    [Header("재화 UI")]
    public TMP_Text goldText;

    [Header("인벤토리 아이템")]
    public GameObject itemButtonPrefab;

    public Transform contentParent;

    public List<InventoryItemData> allItems =
        new List<InventoryItemData>();

    [Header("장착 슬롯")]
    public Image weaponSlotImage;

    public Image clothesSlotImage;

    public Image skillSlot1Image;
    public Image skillSlot2Image;
    public Image skillSlot3Image;

    

    // 현재 인벤토리 건물
    private Interactable currentInventory;

    // 플레이어
    private Transform player;

    //현재 카테고리
    private InventoryItemType currentType;

    // =========================
    // 현재 장착중 아이템
    // =========================
    private InventoryItemData equippedWeapon;

    private InventoryItemData equippedClothes;

    private List<InventoryItemData>
        equippedSkills = new List<InventoryItemData>();

    // =========================
    // 기본 스탯
    // =========================
    public int baseAttack = 10;

    public int baseHealth = 100;

    // =========================
    // 현재 스탯
    // =========================
    private int currentAttack;

    private int currentHealth;

    // =========================
    // 생성된 버튼 저장
    // =========================
    private List<InventoryItemButton>
        itemButtons =
        new List<InventoryItemButton>();



    /*// 테스트용 스탯
    private int playerAttack = 10;
    private int playerHP = 100;*/

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 플레이어 찾기
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 시작 시 패널 끄기
        inventoryRootPanel.SetActive(false);
        inventoryPanel.SetActive(false);

        currentAttack = baseAttack;
        currentHealth = baseHealth;

        // UI 갱신
        UpdateStatUI();
        UpdateGoldUI();

       
    }

    private void Update()
    {
        
    }

    // =========================
    // 인벤토리 열기
    // =========================
    public void OpenInventoryUI(Interactable inventory)
    {
        currentInventory = inventory;

        inventoryRootPanel.SetActive(true);
        inventoryPanel.SetActive(true);

        // UI 최신화
        UpdateStatUI();
        UpdateGoldUI();

        Debug.Log("인벤토리 UI 열림");
    }

    // =========================
    // 인벤토리 닫기
    // =========================
    public void CloseInventoryUI()
    {
        currentInventory = null;

        inventoryRootPanel.SetActive(false);
        inventoryPanel.SetActive(false);

        Debug.Log("인벤토리 UI 닫힘");
    }

    // =========================
    // 스탯 UI 갱신
    // =========================
    void UpdateStatUI()
    {
        attackText.text =
        "ATK : " + currentAttack;

        hpText.text =
            "HP : " + currentHealth;
    }

    // =========================
    // 골드 UI 갱신
    // =========================
    void UpdateGoldUI()
    {
        // 상점 골드 가져오기
        goldText.text =
            ShopUIManager.Instance.currentGold.ToString();
    }

    //카테고리별 아이템 생성
    public void ShowCategoryItems(
    InventoryItemType type)
    {
        currentType = type;

        // 기존 버튼 삭제
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 아이템 생성
        foreach (InventoryItemData item in allItems)
        {
            // 타입 다르면 스킵
            if (item.itemType != type)
                continue;

            // 버튼 생성
            GameObject buttonObj =
                Instantiate(
                    itemButtonPrefab,
                    contentParent
                );

            // 버튼 스크립트 가져오기
            InventoryItemButton button =
                buttonObj.GetComponent<InventoryItemButton>();

            // 데이터 연결
            button.Setup(item);

            Debug.Log(item.itemName + " 생성 완료");
        }
    }

    // =========================
    // 검 카테고리
    // =========================
    public void ShowSwordItems()
    {
        ShowCategoryItems(
            InventoryItemType.Sword);
    }

    // =========================
    // 갑옷 카테고리
    // =========================
    public void ShowClothesItems()
    {
        ShowCategoryItems(
            InventoryItemType.Clothes);
    }

    // =========================
    // 스킬 카테고리
    // =========================
    public void ShowSkillItems()
    {
        ShowCategoryItems(
            InventoryItemType.Skill);
    }

    // =========================
    // 기타 카테고리
    // =========================
    public void ShowEtcItems()
    {
        ShowCategoryItems(
            InventoryItemType.Etc);
    }

    // =========================
    // 아이템 장착/해제
    // =========================
    public void ToggleEquipItem(
        InventoryItemData item)
    {
        switch (item.itemType)
        {
            // =========================
            // 검 장착
            // =========================
            case InventoryItemType.Sword:

                // 이미 장착 중이면 해제
                if (equippedWeapon == item)
                {
                    // =========================
                    // 스탯 감소
                    // =========================
                    currentAttack -= item.attackPower;

                    currentHealth -= item.healthPower;

                    // 장착 해제
                    equippedWeapon = null;

                    // 아이콘 제거
                    weaponSlotImage.sprite = null;

                    Debug.Log("검 장착 해제");
                }
                else
                {
                    
                    // 기존 장비 능력 제거
                    // =========================
                    if (equippedWeapon != null)
                    {
                        currentAttack -=
                            equippedWeapon.attackPower;

                        currentHealth -=
                            equippedWeapon.healthPower;
                    }

                    // 새 장비 장착
                    equippedWeapon = item;

                    // 슬롯 이미지 변경
                    weaponSlotImage.sprite =
                        item.itemIcon;

                    weaponSlotImage.color =
                        Color.white;

                    
                    // 새 장비 능력 적용
                    // =========================
                    currentAttack += item.attackPower;

                    currentHealth += item.healthPower;

                    Debug.Log(item.itemName +
                        " 검 장착");
                }

                // =========================
                // UI 갱신
                // =========================
                UpdateStatUI();

                break;

            // =========================
            // 갑옷 장착
            // =========================
            case InventoryItemType.Clothes:

                if (equippedClothes == item)
                {
                    // 스탯 감소
                    currentAttack -= item.attackPower;

                    currentHealth -= item.healthPower;

                    equippedClothes = null;

                    clothesSlotImage.sprite = null;

                    Debug.Log("갑옷 장착 해제");
                }
                else
                {
                    // 기존 장비 능력 제거
                    if (equippedClothes != null)
                    {
                        currentAttack -=
                            equippedClothes.attackPower;

                        currentHealth -=
                            equippedClothes.healthPower;
                    }

                    equippedClothes = item;

                    clothesSlotImage.sprite =
                        item.itemIcon;

                    clothesSlotImage.color =
                        Color.white;

                    // 새 장비 능력 적용
                    currentAttack += item.attackPower;

                    currentHealth += item.healthPower;

                    Debug.Log(item.itemName +
                        " 갑옷 장착");
                }

                // UI 갱신
                UpdateStatUI();

                break;

            // =========================
            // 스킬 장착
            // =========================
            case InventoryItemType.Skill:

                // 이미 장착 시 해제
                if (equippedSkills.Contains(item))
                {
                    equippedSkills.Remove(item);

                    RefreshSkillSlots();

                    Debug.Log("스킬 해제");
                }
                else
                {
                    // 최대 3개 제한
                    if (equippedSkills.Count >= 3)
                    {
                        Debug.Log(
                            "스킬은 최대 3개");

                        return;
                    }

                    equippedSkills.Add(item);

                    RefreshSkillSlots();

                    Debug.Log(item.itemName +
                        " 스킬 장착");
                }

                break;
        }
        RefreshEquipOutline();
    }

    void RefreshSkillSlots()
    {
        // 전부 초기화
        skillSlot1Image.sprite = null;
        skillSlot2Image.sprite = null;
        skillSlot3Image.sprite = null;

        skillSlot1Image.color =
            new Color(1, 1, 1, 0);

        skillSlot2Image.color =
            new Color(1, 1, 1, 0);

        skillSlot3Image.color =
            new Color(1, 1, 1, 0);

        // 1번 슬롯
        if (equippedSkills.Count > 0)
        {
            skillSlot1Image.sprite =
                equippedSkills[0].itemIcon;

            skillSlot1Image.color =
                Color.white;
        }

        // 2번 슬롯
        if (equippedSkills.Count > 1)
        {
            skillSlot2Image.sprite =
                equippedSkills[1].itemIcon;

            skillSlot2Image.color =
                Color.white;
        }

        // 3번 슬롯
        if (equippedSkills.Count > 2)
        {
            skillSlot3Image.sprite =
                equippedSkills[2].itemIcon;

            skillSlot3Image.color =
                Color.white;
        }
    }
    // =========================
    // 장착 테두리 갱신
    // =========================
    void RefreshEquipOutline()
    {
        foreach (InventoryItemButton
            button in itemButtons)
        {
            bool isEquipped = false;

            InventoryItemData item =
                button.GetItemData();

            // 검
            if (equippedWeapon == item)
                isEquipped = true;

            // 갑옷
            if (equippedClothes == item)
                isEquipped = true;

            // 스킬
            if (equippedSkills.Contains(item))
                isEquipped = true;

            button.SetEquipped(isEquipped);
        }
    }

}
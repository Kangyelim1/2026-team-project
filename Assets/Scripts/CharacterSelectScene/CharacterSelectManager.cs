using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Detail Panel UI")]
    public GameObject detailPanel;
    public Image detailIllustration;
    public TMP_Text detailNameText;
    public TMP_Text detailDescText;
    public TMP_Text detailStatText;

    // 👇 배경 이미지를 띄울 Image 변수를 새로 추가합니다!
    [Header("Detail Background UI")]
    public Image detailBackground;

    [HideInInspector]
    public CharacterCard selectedCharacterCard;

    public void SelectCharacter(CharacterCard card)
    {
        selectedCharacterCard = card;
        OpenDetail(card.myData);
    }

    public void OpenDetail(CharacterData data)
    {
        DataManager.SelectedPlayerID = data.id;
        PlayerPrefs.SetInt("SavedPlayerID", data.id);
        PlayerPrefs.Save();

        detailNameText.text = data.charName;
        detailDescText.text = data.desc;

        // ====================================================
        // [캐릭터별 맞춤 이미지 & 배경 띄우기 로직]
        // ====================================================
        if (data.id == 1 || data.charName.Contains("콩쥐"))
        {
            // 1. 콩쥐 일러스트 띄우기 (Kongjwi_Sword)
            Sprite kongjwiSprite = Resources.Load<Sprite>("Portraits/Kongjwi_Sword");
            if (kongjwiSprite != null)
            {
                detailIllustration.sprite = kongjwiSprite;
                detailIllustration.gameObject.SetActive(true);
            }
            else
            {
                detailIllustration.sprite = data.illustration;
            }

            // 2. 아침 배경 띄우기 (Bg_Morning)
            if (detailBackground != null)
            {
                Sprite bgSprite = Resources.Load<Sprite>("Backgrounds/Bg_Morning");
                if (bgSprite != null)
                {
                    detailBackground.sprite = bgSprite;
                    detailBackground.color = new Color(1, 1, 1, 1);
                    detailBackground.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            // 해님달님 등 다른 캐릭터일 때
            detailIllustration.sprite = data.illustration;

            if (detailBackground != null)
            {
                detailBackground.sprite = null;
                detailBackground.color = new Color(0.95f, 0.9f, 0.85f, 1f); // 기본 연한 베이지색
            }
        }
        // ====================================================

        // 스탯 텍스트 표시
        if (detailStatText != null)
        {
            if (DataManager.Instance != null && DataManager.Instance.playerDict.ContainsKey(DataManager.SelectedPlayerID))
            {
                PlayerData pData = DataManager.Instance.playerDict[DataManager.SelectedPlayerID];
                detailStatText.text = $"HP: {pData.hp}\nACTION: {pData.actionPoint}";
            }
            else
            {
                if (data.id == 1) detailStatText.text = "HP: 100\nACTION: 3";
                else if (data.id == 2) detailStatText.text = "HP: 80\nACTION: 2";
                else detailStatText.text = "";
            }
        }

        detailPanel.SetActive(true);
    }

    public void CloseDetail()
    {
        detailPanel.SetActive(false);
    }

    public void OnClickStartBattle()
    {
        if (selectedCharacterCard == null)
        {
            Debug.LogWarning("캐릭터가 선택되지 않았습니다!");
            return;
        }

        PlayerPrefs.SetInt("CurrentStage", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("StoryScene");
    }
}
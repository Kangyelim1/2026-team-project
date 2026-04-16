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

    [HideInInspector] public CharacterCard selectedCharacterCard;

    public void SelectCharacter(CharacterCard card)
    {
        selectedCharacterCard = card;
        OpenDetail(card.myData);
    }

    public void OpenDetail(CharacterData data)
    {
        DataManager.SelectedPlayerID = data.id;

        // ★ [안전장치] 선택한 캐릭터를 백업해둡니다.
        PlayerPrefs.SetInt("SavedPlayerID", data.id);
        PlayerPrefs.Save();

        detailNameText.text = data.charName;
        detailDescText.text = data.desc;
        detailIllustration.sprite = data.illustration;

        if (detailStatText != null)
        {
            if (DataManager.Instance != null && DataManager.Instance.playerDict.ContainsKey(DataManager.SelectedPlayerID))
            {
                PlayerData pData = DataManager.Instance.playerDict[DataManager.SelectedPlayerID];
                detailStatText.text = $"HP {pData.hp} / 행동력 {pData.actionPoint}";
            }
            else
            {
                if (data.id == 1) detailStatText.text = "HP 100 / 행동력 3";
                else if (data.id == 2) detailStatText.text = "HP 80 / 행동력 2";
                else detailStatText.text = "스탯 정보 없음";
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
            Debug.LogWarning("캐릭터를 먼저 선택해주세요!");
            return;
        }

        // ★ [핵심] 게임 시작 시 스테이지를 무조건 1로 만들고, 스토리를 가장 먼저 틉니다!
        PlayerPrefs.SetInt("CurrentStage", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("StoryScene"); // 배틀씬이 아니라 스토리씬으로 갑니다.
    }
}
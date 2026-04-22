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
        PlayerPrefs.SetInt("SavedPlayerID", data.id);
        PlayerPrefs.Save();

        detailNameText.text = data.charName;
        detailDescText.text = data.desc;
        detailIllustration.sprite = data.illustration;

        // [수정된 부분] 텍스트가 깔끔하게 "HP 100 / 행동력 3" 형태로 나오도록 수정
        if (detailStatText != null)
        {
            if (DataManager.Instance != null && DataManager.Instance.playerDict.ContainsKey(DataManager.SelectedPlayerID))
            {
                PlayerData pData = DataManager.Instance.playerDict[DataManager.SelectedPlayerID];
                // JSON에서 가져온 데이터를 줄바꿈(\n) 해서 표시합니다.
                detailStatText.text = $"HP {pData.hp}\n 행동력 {pData.actionPoint}";
            }
            else
            {
                // 데이터 매니저가 없을 때 (테스트용 하드코딩)
                if (data.id == 1)
                    detailStatText.text = "HP 100\n 행동력 3";
                else if (data.id == 2)
                    detailStatText.text = "HP 80\n 행동력 2";
                else
                    detailStatText.text = "";
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
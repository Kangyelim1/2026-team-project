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

    // [추가된 부분] 스탯을 표시할 UI 텍스트 변수
    [Header("Stat UI")]
    public TMP_Text detailStatText;

    [HideInInspector] public CharacterCard selectedCharacterCard;

    public void SelectCharacter(CharacterCard card)
    {
        selectedCharacterCard = card;
        OpenDetail(card.myData);
    }

    // 카드를 클릭했을 때 호출될 함수
    public void OpenDetail(CharacterData data)
    {
        // --- [리팩토링 완료] 이름 검사 하드코딩 삭제, ScriptableObject의 id 값을 직접 사용 ---
        DataManager.SelectedPlayerID = data.id;
        Debug.Log($"[캐릭터 선택됨] {data.name} 패널 열림! DataManager.SelectedPlayerID에 {DataManager.SelectedPlayerID} 자동 저장");

        // 1. 패널에 캐릭터 정보 데이터 채우기
        detailNameText.text = data.charName;
        detailDescText.text = data.desc;
        detailIllustration.sprite = data.illustration;

        // 2. 스탯 텍스트 표시
        if (detailStatText != null)
        {
            // 실제 게임 플레이 시 (JSON 데이터가 로드된 경우)
            if (DataManager.Instance != null && DataManager.Instance.playerDict.ContainsKey(DataManager.SelectedPlayerID))
            {
                PlayerData pData = DataManager.Instance.playerDict[DataManager.SelectedPlayerID];
                detailStatText.text = $"HP {pData.hp} / 행동력 {pData.actionPoint}";
            }
            else
            {
                // 테스트를 위해 캐릭터 선택 씬만 켰을 때 (DataManager가 없을 경우 예외 처리)
                if (data.id == 1) // 콩쥐
                {
                    detailStatText.text = "HP 100 / 행동력 3";
                }
                else if (data.id == 2) // 햇님달님
                {
                    detailStatText.text = "HP 80 / 행동력 2";
                }
                else
                {
                    detailStatText.text = "스탯 정보 없음";
                }

                Debug.LogWarning("DataManager를 찾을 수 없어 임시 스탯을 표시합니다.");
            }
        }

        // 3. 패널 켜기
        detailPanel.SetActive(true);
    }

    // 돌아가기 버튼용 함수
    public void CloseDetail()
    {
        detailPanel.SetActive(false);
    }

    public void OnClickStartBattle()
    {
        // 1. 선택된 캐릭터가 있는지 확인
        if (selectedCharacterCard == null)
        {
            Debug.LogWarning("캐릭터를 먼저 선택해주세요!");
            return;
        }

        // 2. 전투 씬으로 이동
        SceneManager.LoadScene("BattleScene");
    }
}
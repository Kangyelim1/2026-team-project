using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Detail Panel UI")]
    public GameObject detailPanel;
    public Image detailIllustration;
    public TMP_Text detailNameText;
    public TMP_Text detailDescText;

    // 카드를 클릭했을 때 호출될 함수
    public void OpenDetail(CharacterData data)
    {
        // 1. 패널에 캐릭터 정보 데이터 채우기
        detailNameText.text = data.charName;
        detailDescText.text = data.desc;
        detailIllustration.sprite = data.illustration;

        // 2. 패널 켜기
        detailPanel.SetActive(true);
    }

    // 돌아가기 버튼용 함수
    public void CloseDetail()
    {
        detailPanel.SetActive(false);
    }
}
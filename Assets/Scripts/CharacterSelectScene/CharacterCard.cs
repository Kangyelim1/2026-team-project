using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCard : MonoBehaviour
{
    public CharacterData myData; // 이 카드에 할당할 데이터
    public CharacterSelectManager manager;

    public void OnClickCard()
    {
        manager.SelectCharacter(this);
        
        // --- [안전 장치 추가] 인스펙터 설정 누락 방지용 이름 기반 확실한 ID 세팅 ---
        if (myData.name.Contains("SunMoon") || myData.name.Contains("햇님달님") || (myData.charName != null && myData.charName.Contains("햇님달님"))) {
            myData.id = 2; // 확실하게 ID 2번 부여
        } else if (myData.name.Contains("KongJwi") || myData.name.Contains("콩쥐") || (myData.charName != null && myData.charName.Contains("콩쥐"))) {
            myData.id = 1; // 확실하게 ID 1번 부여
        }
        
        // 2. 내가 들고 있는 데이터의 ID를 DataManager에 임시 저장
        DataManager.SelectedPlayerID = myData.id; 
    
        Debug.Log($"[캐릭터 선택 성공] {myData.charName} 선택됨! DataManager.SelectedPlayerID에 {DataManager.SelectedPlayerID} 값 저장 완료!");
    }
}
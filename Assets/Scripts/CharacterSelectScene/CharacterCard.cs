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
        // 1. 매니저에게 내가 선택되었다고 알림
        manager.SelectCharacter(this);
    
        // 2. 내가 들고 있는 데이터의 ID를 DataManager에 임시 저장
        DataManager.SelectedPlayerID = myData.id; 
    
        Debug.Log($"{myData.charName} 선택됨! ID: {myData.id}");
    }
}
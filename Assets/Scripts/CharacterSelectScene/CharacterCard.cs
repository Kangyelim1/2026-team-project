using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCard : MonoBehaviour
{
    public CharacterData myData; // 이 카드에 할당할 데이터
    public CharacterSelectManager manager;

    public void OnClickCard()
    {
        manager.OpenDetail(myData);
    }
}
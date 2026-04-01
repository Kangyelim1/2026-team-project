using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int id;
    public string charName;         //캐릭터 이름
    [TextArea] public string desc;  //캐릭터 설명
    public Sprite illustration;     //캐릭터 일러스트
    // 필요한 경우 스탯이나 다른 정보를 여기에 추가
}
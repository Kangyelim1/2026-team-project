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
    
    public PlayerTrait trait;       // 캐릭터 특성 (Summoner or Hunter)

    // summoner인지 확인
    public bool IsSummoner()
    {
        return trait == PlayerTrait.Summoner;
    }

    // hunter인지 확인
    public bool IsHunter()
    {
        return trait == PlayerTrait.Hunter;
    }
}
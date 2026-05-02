using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("캐릭터 기본 정보")]
    public int id;
    public string charName;
    public int currentHP;
    public int maxHP;
    public int currentAP;
    public string trait; // "Mob", "Elite", "Boss", "Summoner", "Hunter" 등

    [Header("전투 상태 정보 (에러 해결)")]
    public int maxAP = 3; // 기본 최대 행동력 (플레이어용)
    public List<int> usedSkillsThisTurn = new List<int>(); // 이번 턴에 사용한 스킬 기록
    public List<int> skillList = new List<int>(); // 플레이어/적이 사용하는 스킬(Attack ID) 리스트

    /// <summary>
    /// 플레이어 캐릭터 초기화 (게임 시작 시 호출됨)
    /// </summary>
    public void InitPlayer(int playerID)
    {
        this.id = playerID;
        skillList.Clear(); // ★ 중요: 이전 스킬 리스트 초기화

        // ID가 1이면 콩쥐, 2면 해님달님 (데이터 매니저가 있다면 거기서 불러와도 됩니다)
        if (playerID == 1)
        {
            this.charName = "콩쥐";
            this.maxHP = 100;
            this.currentHP = 100;
            this.maxAP = 3;
            this.currentAP = 3;
            this.trait = "Summoner";

            // ★ [수정됨] 콩쥐의 스킬 ID 할당 (데이터매니저에 1번, 5번 스킬이 있어야 UI에 뜹니다!)
            this.skillList.AddRange(new int[] { 1, 5 });
        }
        else if (playerID == 2)
        {
            this.charName = "해님달님";
            this.maxHP = 80;
            this.currentHP = 80;
            this.maxAP = 2;
            this.currentAP = 2;
            this.trait = "Hunter";

            // ★ [수정됨] 해님달님의 스킬 ID 할당 (데이터매니저에 6번, 7번 스킬이 있어야 UI에 뜹니다!)
            this.skillList.AddRange(new int[] { 6, 7 });
        }
        else
        {
            // 기본값
            this.charName = "알 수 없음";
            this.maxHP = 100;
            this.currentHP = 100;
            this.maxAP = 3;
            this.currentAP = 3;
            this.skillList.Add(1); // 기본 공격 하나는 주도록 설정
        }

        usedSkillsThisTurn.Clear();
    }

    /// <summary>
    /// 적 캐릭터 초기화 (전투 시작 시 호출됨)
    /// </summary>
    public void InitEnemy(int enemyID)
    {
        this.id = enemyID;
        skillList.Clear();

        // 기존 콩쥐 적들 & 해님달님 적들 초기화 하드코딩 (JSON 연동 전 기본 세팅)
        if (enemyID == 1) { this.charName = "무한의 벼"; this.maxHP = 50; this.skillList.AddRange(new int[] { 10, 30 }); }
        else if (enemyID == 2) { this.charName = "공격하는 베"; this.maxHP = 60; this.skillList.AddRange(new int[] { 10, 30 }); }
        else if (enemyID == 3) { this.charName = "팥쥐"; this.maxHP = 100; this.skillList.AddRange(new int[] { 11, 12, 30, 201, 202 }); }
        else if (enemyID == 4) { this.charName = "밑빠진 독"; this.maxHP = 80; this.skillList.AddRange(new int[] { 11, 30 }); }
        else if (enemyID == 5) { this.charName = "계모"; this.maxHP = 150; this.skillList.AddRange(new int[] { 20, 21, 31, 53, 301, 302 }); }
        else if (enemyID == 6) { this.charName = "원님"; this.maxHP = 150; this.skillList.AddRange(new int[] { 13, 21, 31, 50, 401, 402 }); }
        else if (enemyID == 7) { this.charName = "내면의 콩쥐"; this.maxHP = 200; this.skillList.AddRange(new int[] { 21, 31, 53, 202, 302, 402 }); }
        // 해님달님 적들
        else if (enemyID == 11) { this.charName = "아기 호랑이"; this.maxHP = 40; this.skillList.AddRange(new int[] { 10, 30 }); }
        else if (enemyID == 12) { this.charName = "부모 호랑이"; this.maxHP = 90; this.skillList.AddRange(new int[] { 11, 12, 201 }); }
        else if (enemyID == 13) { this.charName = "유령 호랑이"; this.maxHP = 120; this.skillList.AddRange(new int[] { 13, 20, 50, 202 }); }
        else if (enemyID == 14) { this.charName = "호랑이 무리"; this.maxHP = 150; this.skillList.AddRange(new int[] { 21, 31, 301, 302 }); }
        else if (enemyID == 15) { this.charName = "산의 주인"; this.maxHP = 250; this.skillList.AddRange(new int[] { 13, 21, 31, 53, 401, 402 }); }

        this.currentHP = this.maxHP;
        this.currentAP = 0; // 보통 적은 AP 시스템을 안 쓸 수 있으므로 0으로 둠
    }

    /// <summary>
    /// 턴이 시작될 때마다 호출됨 (BattleUI 에러 해결)
    /// </summary>
    public void OnTurnStart()
    {
        // 턴 시작 시 행동력 회복
        this.currentAP = this.maxAP;

        // 이번 턴에 사용한 스킬 기록 초기화
        usedSkillsThisTurn.Clear();
    }

    /// <summary>
    /// 플레이어가 스킬을 사용할 때 호출됨 (BattleUI 에러 해결 및 해님달님 스킬 추가)
    /// </summary>
    public void UseSkill(int skillID, Character target)
    {
        usedSkillsThisTurn.Add(skillID);

        // --- 해님달님 "떡 먹기" 스킬 (임시 ID: 6) ---
        if (skillID == 6)
        {
            int healAmount = 5;
            this.currentHP += healAmount;
            if (this.currentHP > this.maxHP) this.currentHP = this.maxHP;

            if (BattleUI.Instance != null)
                BattleUI.Instance.AddLog($"{this.charName}이(가) 떡을 먹고 체력을 {healAmount} 회복했다! (현재 HP: {this.currentHP})");

            return; // 떡 먹기는 적을 때리지 않으므로 여기서 끝
        }

        // --- 해님달님 "도끼질" 스킬 (임시 ID: 7) ---
        if (skillID == 7)
        {
            int damage = 10;
            target.TakeDamage(damage, this); // attacker로 자기 자신(this)을 넘겨서 패시브 발동 유도
            return;
        }

        // --- 일반/기존 공격 스킬 ---
        // (기본 공격 등 나머지 스킬 처리)
        int normalDamage = 4; // 임시 데미지
        if (skillID == 1 || skillID == 5) normalDamage = 4; // 기본 공격

        target.TakeDamage(normalDamage, this);
    }

    /// <summary>
    /// 적이 스킬(공격)을 사용할 때 호출됨
    /// </summary>
    public void UseEnemySkill(int attackID, Character target)
    {
        // 적 스킬 데이터에 따라 데미지를 다르게 주는 로직
        int damage = Random.Range(3, 8); // 임시 랜덤 데미지

        if (attackID == 401 || attackID == 402) damage = Random.Range(15, 25); // 보스급 강한 공격
        else if (attackID >= 300) damage = Random.Range(10, 15);
        else if (attackID >= 200) damage = Random.Range(8, 12);

        target.TakeDamage(damage, this);
    }

    /// <summary>
    /// 데미지를 받을 때 호출됨 (float 오류 해결 및 패시브 기능 추가)
    /// </summary>
    public void TakeDamage(int damage, Character attacker = null)
    {
        this.currentHP -= damage;
        if (this.currentHP < 0) this.currentHP = 0;

        // --- 해님달님 고유 패시브 "이중타격" 적용 ---
        // 공격자가 존재하고, 그 공격자가 해님달님(ID: 2)일 때 추가 데미지 발동
        if (attacker != null && attacker.id == 2)
        {
            // ★ 에러 해결: 소수점 계산 결과를 명시적으로 int로 형변환
            int bonusDamage = (int)(damage * 0.5f);

            if (bonusDamage > 0)
            {
                this.currentHP -= bonusDamage;
                if (this.currentHP < 0) this.currentHP = 0;

                if (BattleUI.Instance != null)
                    BattleUI.Instance.AddLog($"[이중타격] 추가 피해 {bonusDamage}!");
            }
        }

        // 화면에 데미지 텍스트 띄우기 (플레이어인지 적인지 판별)
        if (BattleUI.Instance != null)
        {
            bool isPlayer = (this.trait == "Summoner" || this.trait == "Hunter" || this.id == 1 || this.id == 2);
            BattleUI.Instance.ShowDamage(isPlayer, damage);
        }
    }
}
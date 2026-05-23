using TMPro;
using UnityEngine;


public class EnemyInteractable : MonoBehaviour
{
    [Header("적 설정")]
    public int enemyId = 1;          // DataManager의 enemyDict 키와 일치시킬 ID
    public string enemyName = ""; // 인스펙터에서 이름 설정
    public float interactRadius = 2f;

    public TextMeshProUGUI nameText;

    // 클릭 감지를 위한 콜라이더 필요 여부 확인용
    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
        if (col == null)
            Debug.LogWarning($"[EnemyInteractable] {enemyName}에 Collider2D가 없습니다. 마우스 클릭이 감지되지 않을 수 있습니다.");
    }

    private void Update()
    {
        nameText.text = enemyName;
    }


    public void OnAttacked(Vector3 playerPosition)
    {
        Debug.Log($"[공격] {enemyName}을(를) 공격했다!");

        if (GameManager.Instance == null)
        {
            Debug.LogError("[EnemyInteractable] GameManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // GameManager에 현재 위치와 적 ID를 넘기고 전투 씬으로 전환
        GameManager.Instance.GoToBattle(enemyId, playerPosition);
    }
}
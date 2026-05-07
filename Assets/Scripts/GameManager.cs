using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance;

    [Header("씬 이름 설정")]
    public string fieldSceneName = "FieldMap";   
    public string battleSceneName = "BattleScene"; 

    // 필드맵으로 돌아올 때 복원할 플레이어 위치
    [HideInInspector]
    public Vector3 savedPlayerPosition;

    // 현재 전투 중인 적의 ID (DataManager 연동용)
    [HideInInspector]
    public int currentEnemyId = -1;

    void Awake()
    {
        // [신규] 싱글톤 패턴: 이미 인스턴스가 있으면 중복 오브젝트 제거
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    public void GoToBattle(int enemyId, Vector3 playerPosition)
    {
        // 현재 플레이어 위치를 저장 (필드맵 복귀 시 이 위치에서 시작)
        savedPlayerPosition = playerPosition;
        currentEnemyId = enemyId;

        Debug.Log($"[GameManager] 전투 시작! enemyId:{enemyId}, 저장 위치:{playerPosition}");
        SceneManager.LoadScene(battleSceneName);
    }

    
    public void ReturnToField()
    {
        Debug.Log($"[GameManager] 필드맵 복귀, 복원 위치:{savedPlayerPosition}");
        SceneManager.LoadScene(fieldSceneName);
        // 씬 로드 완료 후 위치 복원은 OnSceneLoaded 이벤트로 처리
        SceneManager.sceneLoaded += OnFieldSceneLoaded;
    }

    
    private void OnFieldSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != fieldSceneName) return;

        // 이벤트 구독 해제 (일회성)
        SceneManager.sceneLoaded -= OnFieldSceneLoaded;

        // "Player" 태그로 플레이어 오브젝트 찾아 위치 복원
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = savedPlayerPosition;
            Debug.Log($"[GameManager] 플레이어 위치 복원 완료: {savedPlayerPosition}");
        }
        else
        {
            Debug.LogWarning("[GameManager] 플레이어 오브젝트를 찾을 수 없습니다. 태그가 'Player'인지 확인하세요.");
        }
    }
}
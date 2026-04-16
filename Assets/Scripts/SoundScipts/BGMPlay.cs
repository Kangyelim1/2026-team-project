using UnityEngine;

public class BGMPlay : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        // 씬이 바뀌어도 음악 오브젝트가 파괴되지 않게 설정
        DontDestroyOnLoad(gameObject);

        // AudioSource 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();

        // 코드로 반복 재생 설정 (인스펙터에서 체크하는 것과 동일)
        if (audioSource != null)
        {
            audioSource.loop = true; 
        }
    }

    // 버튼을 눌렀을 때 호출할 함수
    public void StopBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            // 만약 다음 씬에서도 아예 안 나오게 하고 싶다면 오브젝트를 삭제할 수도 있습니다.
            // Destroy(gameObject); 
        }
    }
}
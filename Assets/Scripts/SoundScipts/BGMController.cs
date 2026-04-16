using UnityEngine;

public class BGMController : MonoBehaviour
{
    // 버튼에 이 함수를 연결하세요
    public void StopMusic()
    {
        // AudioManager의 인스턴스를 찾아 bgmSource를 정지시킵니다.
        if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
        {
            AudioManager.Instance.bgmSource.Stop();
        }
    }

    // (참고) 다시 재생하고 싶을 때 사용할 함수
    public void PlayMusic()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
        {
            if (!AudioManager.Instance.bgmSource.isPlaying)
            {
                AudioManager.Instance.bgmSource.Play();
            }
        }
    }
}
using System.Collections;
using UnityEngine;

public class BossPatternSystem : MonoBehaviour
{
    public BossSystem bossSystem;
    public PlayerHelthSystem playerHelthSystem;

    private void Start()
    {
        bossSystem = FindAnyObjectByType<BossSystem>();
    }

    private void Update()
    {
        if (playerHelthSystem == null)
            playerHelthSystem = FindAnyObjectByType<PlayerHelthSystem>();
    }

    public IEnumerator BossPattern01()
    {
        Debug.Log("패턴01 실행");
        yield return new WaitForSeconds(1f);
        Debug.Log("패턴 종료");
        bossSystem.BossRandomPattern();
    }

    public IEnumerator BossPattern02()
    {
        Debug.Log("패턴02 실행");
        yield return new WaitForSeconds(1f);
        Debug.Log("패턴 종료");
        bossSystem.BossRandomPattern();
    }
}

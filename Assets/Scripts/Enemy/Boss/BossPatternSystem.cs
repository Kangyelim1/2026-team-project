using System.Collections;
using UnityEngine;

public class BossPatternSystem : MonoBehaviour
{
    public BossSystem bossSystem;
    public PlayerHelthSystem playerHelthSystem;

    [Header("레이져 공격")]
    public LineRenderer laserLine01;
    public LineRenderer laserLine02;
    public GameObject hitCollider;

    public Transform player;
    public Transform laserStart01;
    public Transform laserStart02;

    public float aimTime = 2f;
    public float laserLength = 20f;

    private Vector3 targetPos;

    private void Start()
    {
        bossSystem = FindAnyObjectByType<BossSystem>();
    }

    private void Update()
    {
        if (playerHelthSystem == null)
            playerHelthSystem = FindAnyObjectByType<PlayerHelthSystem>();
    }

    public IEnumerator LaserAttack()
    {
        laserLine01.enabled = true;
        laserLine02.enabled = true;

        hitCollider.SetActive(false);
        float timer = 0f;

        while (timer < aimTime)
        {
            targetPos = player.position;
            targetPos.z = 0f;

            Vector3 dir01 = (targetPos - laserStart01.position).normalized;
            Vector3 dir02 = (targetPos - laserStart02.position).normalized;

            laserLine01.positionCount = 2;
            laserLine01.SetPosition(0, laserStart01.position);
            laserLine01.SetPosition(1, laserStart01.position + dir01 * laserLength);

            laserLine02.positionCount = 2;
            laserLine02.SetPosition(0, laserStart02.position);
            laserLine02.SetPosition(1, laserStart02.position + dir02 * laserLength);

            timer += Time.deltaTime;
            yield return null;
        }

        hitCollider.transform.position = targetPos;
        hitCollider.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        hitCollider.SetActive(false);

        laserLine01.enabled = false;
        laserLine02.enabled = false;

        yield return new WaitForSeconds(0.1f);
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

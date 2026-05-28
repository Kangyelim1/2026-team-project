using System.Collections;
using UnityEngine;

public class BossPatternSystem : MonoBehaviour
{
    public BossSystem bossSystem;
    public EnemyHelthSystem enemyHelthSystem;

    public PlayerHelthSystem playerHelthSystem;
    public PlayerSystem playerSystem;

    [Header("레이져 공격")]
    public LineRenderer laserLine01;
    public LineRenderer laserLine02;
    public GameObject hitCollider;

    [Header("절류 방출")]
    public GameObject electricObject;
    public GameObject fakeElectricObject;


    public Transform player;
    public Transform laserStart01;
    public Transform laserStart02;

    public float aimTime = 2f;
    public float laserLength = 20f;

    private Vector3 targetPos;

    private void Start()
    {
        bossSystem = FindAnyObjectByType<BossSystem>();
        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
    }

    private void Update()
    {
        if (playerHelthSystem == null)
            playerHelthSystem = FindAnyObjectByType<PlayerHelthSystem>();   

        if(playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();
    }

    public IEnumerator LaserAttack()
    {
        if (player == null) yield break;

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
        yield return new WaitForSeconds(0.5f);
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
        electricObject.SetActive(false);
        fakeElectricObject.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            fakeElectricObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            fakeElectricObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }

        electricObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        electricObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);
        bossSystem.BossRandomPattern();
    }
}

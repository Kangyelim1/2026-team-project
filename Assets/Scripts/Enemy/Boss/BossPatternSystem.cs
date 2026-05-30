using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternSystem : MonoBehaviour
{
    public BossSystem bossSystem;
    public EnemyHelthSystem enemyHelthSystem;

    public PlayerHelthSystem playerHelthSystem;
    public PlayerSystem playerSystem;

    public float BossPatternTime;

    [Header("레이져 공격")]
    public LineRenderer laserLine01;
    public LineRenderer laserLine02;
    public GameObject hitCollider;

    [Header("전기 패턴")]
    public GameObject WormHole01;
    public GameObject WormHole02;
    public GameObject electricObject;

    [Header("검은 물체 생성 패턴")]
    public List<GameObject> objectList = new List<GameObject>();

    [Header("오브젝트 파괴 패턴")]
    public GameObject destoryObject;
    public bool isDestoryObject;

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

        if(enemyHelthSystem.currentBossHelth >= 150) aimTime = 2.5f;
        else aimTime = 2f;

        if(playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();

        playerSystem.LockOnImage.SetActive(true);

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
        playerSystem.LockOnImage.SetActive(false);
        hitCollider.transform.position = targetPos;
        yield return new WaitForSeconds(1f);
        hitCollider.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        hitCollider.SetActive(false);

        laserLine01.enabled = false;
        laserLine02.enabled = false;

        yield return new WaitForSeconds(BossPatternTime);
        bossSystem.BossRandomPattern();
    }

    public IEnumerator BossPattern02()
    {
        electricObject.SetActive(false);
        WormHole01.SetActive(false);
        WormHole02.SetActive(false);

        WormHole01.SetActive(true);
        WormHole02.SetActive(true);

        yield return new WaitForSeconds(2f);
        electricObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        electricObject.SetActive(false);
        WormHole01.SetActive(false);
        WormHole02.SetActive(false);

        yield return new WaitForSeconds(BossPatternTime);
        bossSystem.BossRandomPattern();
    }

    public IEnumerator CreateObjectPattern()
    {
        List<GameObject> randomObjects = new List<GameObject>();

        while (randomObjects.Count < 3)
        {
            int randomIndex = Random.Range(0, objectList.Count);
            GameObject randomObj = objectList[randomIndex];

            if (!randomObjects.Contains(randomObj))
            {
                randomObjects.Add(randomObj);
                randomObj.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject obj in randomObjects)
        {
            obj.SetActive(false);
        }
        yield return new WaitForSeconds(BossPatternTime);
        bossSystem.BossRandomPattern();
    }

    public IEnumerator DestoryObjectPattern()
    {
        if (playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();

        Vector3 spawnPos = new Vector3(playerSystem.gameObject.transform.position.x, playerSystem.transform.position.y + 12f, 0f);
        Instantiate(destoryObject, spawnPos, Quaternion.identity);
        

        yield return new WaitForSeconds(0.5f);
        Debug.Log("7초 카운트 시작");

        yield return new WaitForSeconds(7f);

        if (isDestoryObject)
        {
            Debug.Log("오브젝트 제거 성공");
            isDestoryObject = false;
            yield return new WaitForSeconds(BossPatternTime);
            bossSystem.BossRandomPattern();
        }
        else
        {
            Debug.Log("오브젝트 제거 실패");
            playerHelthSystem.Die();
        }
    }
}


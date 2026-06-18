using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class BossPatternSystem : MonoBehaviour
{
    public BossSystem bossSystem;
    public EnemyHelthSystem enemyHelthSystem;
    public EnemySystem enemySystem;

    public PlayerHelthSystem playerHelthSystem;
    public PlayerSystem playerSystem;

    public CinemachineCamera playerCamera;

    public float BossPatternTime;

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

    [Header("전기 패턴")]
    public GameObject WormHole01;
    public GameObject WormHole02;
    public GameObject electricObject;
    public GameObject fakeObject;


    [Header("검은 물체 생성 패턴")]
    public List<GameObject> objectList = new List<GameObject>();

    [Header("오브젝트 파괴 패턴")]
    public GameObject destoryObject;
    public bool isDestoryObject;

    [Header("미사일 패턴")]
    public CinemachineCamera bossCamera;
    public GameObject missilePrefab;
    public GameObject TargetPoint;
    public Transform missileFirePoint;
    public Transform[] dropPoints;
    public float missileShootDelay = 0.3f;
    public float cameraShakePower = 0.3f;
    public float cameraShakeTime = 0.2f;
    public float warningTime = 1f;
    public float warningRange = 4f;
    public int warningCount = 3;

    [Header("돌진")]
    public EnemyChargeSystem enemyChargeSystem;

    public bool isPattern;

    [Header("드론 소환")]
    public GameObject gunDronePrefab;
    public Transform droneSpawnPoint;

    public GameSoundManager gameSoundManager;

    private void Start()
    {
        bossSystem = FindAnyObjectByType<BossSystem>();
        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        enemySystem = FindAnyObjectByType<EnemySystem>();
        enemyChargeSystem = FindAnyObjectByType<EnemyChargeSystem>();
        gameSoundManager = FindAnyObjectByType<GameSoundManager>();
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
        isPattern = true;

        if (enemyHelthSystem.currentBossHelth >= 150) aimTime = 2f;
        else aimTime = 1.5f;

        if(playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();

        playerSystem.LockOnImage.SetActive(true);
        gameSoundManager.OnFindEnemySound("조준 패턴");

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
        yield return new WaitForSeconds(0.3f);
        hitCollider.SetActive(true);
        gameSoundManager.OnFindEnemySound("레이저 패턴");
        yield return new WaitForSeconds(0.3f);
        hitCollider.SetActive(false);

        laserLine01.enabled = false;
        laserLine02.enabled = false;

        yield return new WaitForSeconds(BossPatternTime);
        isPattern = false;
        bossSystem.BossRandomPattern();
    }

    public IEnumerator BossPattern02()
    {
        isPattern = true;

        electricObject.SetActive(false);
        WormHole01.SetActive(false);
        WormHole02.SetActive(false);

        WormHole01.SetActive(true);
        WormHole02.SetActive(true);
        gameSoundManager.OnFindEnemySound("레이저 시작 사운드");

        for (int i = 0; i < 3; i++)
        {
            fakeObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            fakeObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.2f);
        electricObject.SetActive(true);
        gameSoundManager.OnFindEnemySound("레이저 패턴");
        yield return new WaitForSeconds(0.5f);
        electricObject.SetActive(false);
        WormHole01.SetActive(false);
        WormHole02.SetActive(false);

        yield return new WaitForSeconds(BossPatternTime);
        isPattern = false;
        bossSystem.BossRandomPattern();
    }

    public IEnumerator CreateObjectPattern()
    {
        isPattern = true;
        List<GameObject> randomObjects = new List<GameObject>();

        while (randomObjects.Count < 10)
        {
            int randomIndex = Random.Range(0, objectList.Count);
            GameObject randomObj = objectList[randomIndex];

            if (!randomObjects.Contains(randomObj))
            {
                randomObjects.Add(randomObj);
                randomObj.SetActive(true);
                gameSoundManager.OnFindEnemySound("타겟 지정");
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }
        
        yield return new WaitForSeconds(0.55f);

        foreach (GameObject obj in randomObjects)
        {
            obj.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(BossPatternTime);
        isPattern = false;
        bossSystem.BossRandomPattern();
    }

    public IEnumerator DestoryObjectPattern()
    {
        if (playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();

        isPattern = true;
        enemySystem.isPattern = true;
        enemyHelthSystem.isInvincibility = true;

        if (playerSystem == null) yield break;
        playerSystem.FakeDestoryObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        playerSystem.FakeDestoryObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);

        Vector3 spawnPos = new Vector3(playerSystem.gameObject.transform.position.x, playerSystem.transform.position.y + 12f, 0f);
        Instantiate(destoryObject, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        Debug.Log("7초 카운트 시작");
        float timer = 0f;

        while (timer < 7f)
        {
            if (isDestoryObject)
            {
                Debug.Log("오브젝트 제거 성공");
                gameSoundManager.OnFindEnemySound("오브젝트 파괴 소리");

                isDestoryObject = false;

                yield return new WaitForSeconds(BossPatternTime);

                isPattern = false;
                enemySystem.isPattern = false;
                enemyHelthSystem.isInvincibility = false;

                bossSystem.BossRandomPattern();
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("오브젝트 제거 실패");
        playerHelthSystem.Die();
    }

    public IEnumerator Missile()
    {
        if (playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();

        isPattern = true;
        enemySystem.isPattern = true;

        bossCamera.gameObject.SetActive(true);
        playerCamera.Priority = 10;
        bossCamera.Priority = 20;

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < warningCount; i++)
        {
            gameSoundManager.OnFindEnemySound("미사일 발사");

            GameObject missile = Instantiate(missilePrefab, missileFirePoint.position, Quaternion.identity);

            if (missile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                rb.AddForce(Vector2.up * 100f, ForceMode2D.Impulse);

            bossCamera.transform.DOShakePosition(cameraShakeTime, cameraShakePower);

            yield return new WaitForSeconds(missileShootDelay);
        }

        bossCamera.Priority = 0;
        playerCamera.Priority = 10;
        bossCamera.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        List<float> usedX = new List<float>();

        for (int i = 0; i < warningCount; i++)
        {
            float randomX = playerSystem.transform.position.x;

            if (i == 0)
            {
                randomX = playerSystem.transform.position.x;
            }
            else
            {
                int tryCount = 0;
                int maxTryCount = 50;

                do
                {
                    randomX = playerSystem.transform.position.x + Random.Range(-warningRange, warningRange);
                    tryCount++;
                }
                while (usedX.Exists(x => Mathf.Abs(x - randomX) < 1.5f) && tryCount < maxTryCount);
            }

            usedX.Add(randomX);

            Vector3 warningPos = new Vector3(randomX, playerSystem.transform.position.y, 0f);

            GameObject warning = Instantiate(TargetPoint, warningPos, Quaternion.identity);

            gameSoundManager.OnFindEnemySound("타겟 지정");

            Destroy(warning, warningTime);

            Vector3 spawnPos = warningPos + Vector3.up * 100f;

            GameObject bullet = Instantiate(missilePrefab, spawnPos, Quaternion.Euler(0, 0, 180));

            if (bullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                rb.linearVelocity = Vector2.down * 60;

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(warningTime);

        yield return new WaitForSeconds(BossPatternTime);

        isPattern = false;
        enemySystem.isPattern = false;

        bossSystem.BossRandomPattern();
    }

    public IEnumerator Rush()
    {
        StartCoroutine(enemyChargeSystem.ChargeSequence());
        yield return new WaitForSeconds(BossPatternTime);
        isPattern = false;
        bossSystem.BossRandomPattern();
    }

    public IEnumerator SuicideDrone()
    {
        isPattern = true;

        Instantiate(gunDronePrefab, droneSpawnPoint.position, Quaternion.identity);
        Debug.Log("사격 드론 생성");

        yield return new WaitForSeconds(BossPatternTime);

        isPattern = false;
        bossSystem.BossRandomPattern();
    }
}


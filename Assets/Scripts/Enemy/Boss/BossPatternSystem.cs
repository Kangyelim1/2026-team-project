using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using static EnemyChargeSystem;

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
    public CinemachineCamera wormHoleCamera;
    public float showTime = 2f;

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

    [Header("돌진")]
    public float chargeReadyTime = 0.5f;
    public float chargeDistance = 7f;
    public float chargeSpeed = 22f;
    private Vector2 startPos;
    private int moveDir = 1;
    private bool isCoroutineRunning = false;
    public enum ChargeState { Patrol, Ready, Charge, Stun, Return }
    public ChargeState currentState = ChargeState.Patrol;

    [Header("스턴 설정")]
    public float stunTime = 1f;

    [Header("상태 확인")]
    public bool isInvincible = false;

    public float dropNearRangeX = 4f;
    public float dropNearRangeY = 1f;
    public float minDropDistance = 1f;

    public float missileShootDelay = 0.3f;
    public float cameraShakePower = 0.3f;
    public float cameraShakeTime = 0.2f;
    public float warningTime = 1f;

    public float warningRange = 4f;
    public int warningCount = 3;

    public Rigidbody2D rb;

    

    public bool isPattern;

    private void Start()
    {
        bossSystem = FindAnyObjectByType<BossSystem>();
        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        enemySystem = FindAnyObjectByType<EnemySystem>();
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

        if (enemyHelthSystem.currentBossHelth >= 150) aimTime = 2.5f;
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
        wormHoleCamera.gameObject.SetActive(true);

        playerCamera.Priority = 10;
        wormHoleCamera.Priority = 20;
        wormHoleCamera.Follow = WormHole01.transform;
        yield return new WaitForSeconds(showTime);

        wormHoleCamera.Follow = WormHole02.transform;
        yield return new WaitForSeconds(showTime);

        wormHoleCamera.Priority = 0;
        playerCamera.Priority = 10;
        wormHoleCamera.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);
        electricObject.SetActive(true);

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
        isPattern = false;
        bossSystem.BossRandomPattern();
    }

    public IEnumerator DestoryObjectPattern()
    {
        if (playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();

        isPattern = true;
        enemySystem.isPattern = true;

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
            isPattern = false;
            enemySystem.isPattern = false;
            bossSystem.BossRandomPattern();
        }
        else
        {
            Debug.Log("오브젝트 제거 실패");
            playerHelthSystem.Die();
        }
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

        for (int i = 0; i < 3; i++)
        {
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
            float randomX;

            if (i == 0)
            {
                randomX = playerSystem.transform.position.x;
            }
            else
            {
                do
                {
                    randomX = playerSystem.transform.position.x + Random.Range(-warningRange, warningRange);
                }
                while (usedX.Exists(x => Mathf.Abs(x - randomX) < 1.5f));
            }

            usedX.Add(randomX);

            Vector3 warningPos = new Vector3(randomX, playerSystem.transform.position.y, 0f);

            GameObject warning = Instantiate(TargetPoint, warningPos, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            Destroy(warning, warningTime);

            Vector3 spawnPos = warningPos + Vector3.up * 100f;

            GameObject bullet = Instantiate(missilePrefab, spawnPos, Quaternion.identity);

            if (bullet.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.down * 100;
            }
        }

        yield return new WaitForSeconds(warningTime);

        yield return new WaitForSeconds(BossPatternTime);
        isPattern = false;
        enemySystem.isPattern = false;
        bossSystem.BossRandomPattern();
    }

    public IEnumerator Rush()
    {
        StartCoroutine(ChargeSequence());
        yield return new WaitForSeconds(BossPatternTime);
        isPattern = false;
        bossSystem.BossRandomPattern();
    }

    IEnumerator ChargeSequence()
    {
        isCoroutineRunning = true;

        currentState = ChargeState.Ready;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("차징 시작! 0.5초 후 돌진");

        yield return new WaitForSeconds(chargeReadyTime);

        currentState = ChargeState.Charge;
        isInvincible = true;
        Debug.Log("돌진 시작! 무적 ON");

        float targetX = transform.position.x + (moveDir * chargeDistance);

        while (true)
        {
            rb.linearVelocity = new Vector2(moveDir * chargeSpeed, rb.linearVelocity.y);

            bool reachedTarget = (moveDir == 1 && transform.position.x >= targetX)
                              || (moveDir == -1 && transform.position.x <= targetX);

            if (reachedTarget)
            {
                Debug.Log("목표 지점 도달 → 스턴");
                break;
            }

            yield return null;
        }

        yield return StartCoroutine(StunSequence());
    }

    IEnumerator StunSequence()
    {
        currentState = ChargeState.Stun;
        isInvincible = false;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("스턴! 무적 OFF. " + stunTime + "초 후 귀환");

        yield return new WaitForSeconds(stunTime);

        currentState = ChargeState.Return;
        isCoroutineRunning = false;
        Debug.Log("귀환 시작");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState != ChargeState.Charge) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어 명중!");

            PlayerHelthSystem playerHelth =
                collision.gameObject.GetComponent<PlayerHelthSystem>();

            if (playerHelth == null)
                playerHelth = collision.gameObject.GetComponentInChildren<PlayerHelthSystem>();

            if (playerHelth != null)
                playerHelth.Die();

            StopAllCoroutines();
            StartCoroutine(StunSequence());
        }
        else if (!collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("벽 충돌! 스턴");
            StopAllCoroutines();
            StartCoroutine(StunSequence());
        }
    }

    public IEnumerator SuicideDrone()
    {
        yield return new WaitForSeconds(BossPatternTime);
        isPattern = false;
        bossSystem.BossRandomPattern();
    }
}


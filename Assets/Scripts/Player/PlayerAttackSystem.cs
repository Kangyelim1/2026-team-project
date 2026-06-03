using System.Collections;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
    [Header("참조")]
    public PlayerSystem playerSystem;
    public Camera mainCamera;

    [Header("기본공격")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float attackCooldown = 2f;

    [Header("콤보 탄환")]
    public float comboShotDelay = 0.3f;
    public int comboDamage = 16;

    [Header("다중공격")]
    public float multiAttackDuration = 1f;
    public float multiAttackCooldown = 15f;

    [Header("투척무기")]
    public GameObject throwWeaponPrefab;
    public Transform throwPoint;
    public float throwSpeed = 12f;
    public float throwCooldown = 60f;

    [Header("근접공격")]
    public Transform meleePoint;
    public float meleeRange = 1.2f;
    public LayerMask enemyLayer;

    [Header("패링")]
    public float parryDuration = 0.1f;
    public float parryCooldown = 2f;
    public int parryDamage = 6;

    [Header("궁극기")]
    public float ultimateCastTime = 2f;
    public float ultimateCooldown = 100f;
    public float ultimateRange = 8f;
    public int ultimateDamage = 30;

    [Header("덫")]
    public GameObject trapPrefab;
    public Transform trapPoint;
    public float trapCooldown = 90f;

    [Header("상호작용")]
    public float interactHoldTime = 1.5f;

    private float lastAttackTime = -999f;
    private float lastMultiAttackTime = -999f;
    private float lastThrowTime = -999f;
    private float lastParryTime = -999f;
    private float lastUltimateTime = -999f;
    private float lastTrapTime = -999f;

    private bool multiAttackMode;
    private bool isParryWindow;
    private bool isUltimateCasting;
    private float fHoldTimer;

    public bool IsInvincible => isParryWindow || isUltimateCasting || (playerSystem != null && playerSystem.IsDash);
    public bool IsParryWindow => isParryWindow;

    private void Awake()
    {
        if (playerSystem == null) playerSystem = GetComponent<PlayerSystem>();
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleInteraction();
        HandleAttackInput();
    }

    private void HandleInteraction()
    {
        if (Input.GetKey(KeyCode.F) && !Input.GetMouseButton(1))
        {
            fHoldTimer += Time.deltaTime;

            if (fHoldTimer >= interactHoldTime)
            {
                Debug.Log("상호작용 실행");
                fHoldTimer = -999f;
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            fHoldTimer = 0f;
        }
    }

    private void HandleAttackInput()
    {
        bool comboHeld = Input.GetMouseButton(1);

        if (comboHeld && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ComboShot());
            return;
        }

        if (comboHeld && Input.GetKeyDown(KeyCode.Space))
        {
            if (playerSystem != null)
                playerSystem.HighJump(1.5f);
            return;
        }

        if (comboHeld && Input.GetKeyDown(KeyCode.LeftShift))
        {
            MeleeAttack(10);
            return;
        }

        if (comboHeld && Input.GetKeyDown(KeyCode.F))
        {
            TryParry();
            return;
        }

        if (comboHeld && Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCoroutine(UltimateRoutine());
            return;
        }

        if (comboHeld && Input.GetKeyDown(KeyCode.E))
        {
            InstallTrap();
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            TryMultiAttackMode();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowWeapon();
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(BasicAttack());
        }
    }

    IEnumerator BasicAttack()
    {
        if (!multiAttackMode && Time.time < lastAttackTime + attackCooldown)
           yield break;

        lastAttackTime = Time.time;
        playerSystem.playerAnimator.SetBool("isGun", true);
        ShootProjectile(bulletPrefab, firePoint, bulletSpeed);
        
        yield return new WaitForSeconds(0.3f);
        playerSystem.playerAnimator.SetBool("isGun", false);
    }

    private IEnumerator ComboShot()
    {
        yield return new WaitForSeconds(comboShotDelay);
        ShootProjectile(bulletPrefab, firePoint, bulletSpeed);
        Debug.Log("콤보탄 발사 / 데미지 16");
    }

    private void ShootProjectile(GameObject prefab, Transform spawnPoint, float speed)
    {
        if (prefab == null || spawnPoint == null || mainCamera == null)
            return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 dir = (mousePos - spawnPoint.position).normalized;

        GameObject bullet = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * speed;
    }

    private void TryMultiAttackMode()
    {
        if (Time.time < lastMultiAttackTime + multiAttackCooldown)
            return;

        StartCoroutine(MultiAttackRoutine());
    }

    private IEnumerator MultiAttackRoutine()
    {
        lastMultiAttackTime = Time.time;
        multiAttackMode = true;
        Debug.Log("다중공격 ON");

        yield return new WaitForSeconds(multiAttackDuration);

        multiAttackMode = false;
        Debug.Log("다중공격 OFF");
    }

    private void ThrowWeapon()
    {
        if (Time.time < lastThrowTime + throwCooldown)
            return;

        if (throwWeaponPrefab == null || throwPoint == null || mainCamera == null)
            return;

        lastThrowTime = Time.time;
        ShootProjectile(throwWeaponPrefab, throwPoint, throwSpeed);
        Debug.Log("투척무기 사용 / 데미지 22");
    }

    private void MeleeAttack(int damage)
    {
        if (meleePoint == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(meleePoint.position, meleeRange, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            EnemyHelthSystem enemy = hit.GetComponent<EnemyHelthSystem>();
            if (enemy == null) enemy = hit.GetComponentInParent<EnemyHelthSystem>();

            if (enemy != null)
            {
                ApplyDamage(enemy, damage);
            }
        }

        Debug.Log("근접공격 / 데미지 10");
    }

    private void TryParry()
    {
        if (Time.time < lastParryTime + parryCooldown)
            return;

        StartCoroutine(ParryRoutine());
    }

    private IEnumerator ParryRoutine()
    {
        lastParryTime = Time.time;
        isParryWindow = true;
        Debug.Log("패링 시작");

        yield return new WaitForSeconds(parryDuration);

        isParryWindow = false;
        Debug.Log("패링 종료");
    }

    private IEnumerator UltimateRoutine()
    {
        if (Time.time < lastUltimateTime + ultimateCooldown)
            yield break;

        lastUltimateTime = Time.time;
        isUltimateCasting = true;

        if (playerSystem != null && playerSystem.LockOnImage != null)
            playerSystem.LockOnImage.SetActive(true);

        Debug.Log("궁극기 선딜 시작");
        yield return new WaitForSeconds(ultimateCastTime);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, ultimateRange, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            EnemyHelthSystem enemy = hit.GetComponent<EnemyHelthSystem>();
            if (enemy == null) enemy = hit.GetComponentInParent<EnemyHelthSystem>();

            if (enemy != null)
            {
                ApplyDamage(enemy, ultimateDamage);
            }
        }

        if (playerSystem != null && playerSystem.LockOnImage != null)
            playerSystem.LockOnImage.SetActive(false);

        isUltimateCasting = false;
        Debug.Log("궁극기 발동 / 데미지 30");
    }

    private void InstallTrap()
    {
        if (Time.time < lastTrapTime + trapCooldown)
            return;

        if (trapPrefab == null || trapPoint == null)
            return;

        lastTrapTime = Time.time;
        Instantiate(trapPrefab, trapPoint.position, Quaternion.identity);
        Debug.Log("덫 설치 / 데미지 22");
    }

    private void ApplyDamage(EnemyHelthSystem enemy, int damage)
    {
        if (enemy.enemySystem != null && enemy.enemySystem.enemyType == EnemyType.Boss)
        {
            enemy.currentBossHelth -= damage;
            if (enemy.currentBossHelth <= 0)
                enemy.Die();
        }
        else
        {
            enemy.Die();
        }
    }

    public bool TryParryCounter(Collider2D collision)
    {
        if (!isParryWindow) return false;

        EnemyHelthSystem enemy = collision.GetComponent<EnemyHelthSystem>();
        if (enemy == null) enemy = collision.GetComponentInParent<EnemyHelthSystem>();

        if (enemy != null)
        {
            ApplyDamage(enemy, parryDamage);
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (meleePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleePoint.position, meleeRange);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ultimateRange);
    }
}
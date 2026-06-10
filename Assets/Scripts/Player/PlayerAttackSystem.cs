using System.Collections;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
    [Header("����")]
    public PlayerSystem playerSystem;
    public GameSoundManager gameSoundManager;
    public Camera mainCamera;

    [Header("�⺻ ����")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float bulletLifetime = 0.4f;
    public float attackCooldown = 1.0f;

    [Header("źâ")]
    public int maxAmmo = 6;
    public float reloadTime = 1.5f;
    private bool isReloading = false;
    
    public bool isShootingLocked = false;
    public int currentAmmo;

    [Header("�޺� źȯ (��Ŭ�� + ��Ŭ��)")]
    public float comboShotDelay = 0.3f;
    public int comboDamage = 16;

    [Header("��ô���� (E)")]
    public GameObject throwWeaponPrefab;
    public Transform throwPoint;
    public float throwSpeed = 12f;
    public float throwCooldown = 60f;

    [Header("�������� (G)")]
    public Transform meleePoint;
    public float meleeRange = 1.2f;
    public float meleeCooldown = 1.0f;
    public LayerMask enemyLayer;

    [Header("�и� (��Ŭ�� + G)")]
    public float parryDuration = 1.0f;
    public float parryCooldown = 2f;
    public int parryDamage = 6;

    [Header("��ȣ�ۿ�")]
    public float interactHoldTime = 1.5f;

    private float lastAttackTime = -999f;
    private float lastMeleeTime = -999f;
    private float lastThrowTime = -999f;
    private float lastParryTime = -999f;

    private bool isParryWindow;
    private bool isComboShotRunning = false;
    private float fHoldTimer;

    public bool IsInvincible => isParryWindow || (playerSystem != null && playerSystem.IsDash);
    public bool IsParryWindow => isParryWindow;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;

    private void Awake()
    {
        if (playerSystem == null) playerSystem = GetComponent<PlayerSystem>();
        if (mainCamera == null) mainCamera = Camera.main;
        gameSoundManager = FindAnyObjectByType<GameSoundManager>();

        currentAmmo = maxAmmo;
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
                Debug.Log("��ȣ�ۿ� ����");
                fHoldTimer = -999f;
            }
        }
        if (Input.GetKeyUp(KeyCode.F))
            fHoldTimer = 0f;
    }

    private void HandleAttackInput()
    {
        bool comboHeld = Input.GetMouseButton(1);

        if (comboHeld && Input.GetMouseButtonDown(0))
        {
            if (!isComboShotRunning)
                StartCoroutine(ComboShot());
            return;
        }

        if (comboHeld && Input.GetKeyDown(KeyCode.G))
        {
            TryParry();
            return;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            MeleeAttack(10);
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowWeapon();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(BasicAttack());
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReloading && currentAmmo < maxAmmo)
                StartCoroutine(Reload());
        }
    }

    private bool CanShoot()
    {
        if (isShootingLocked)
        {
            Debug.Log("źâ ����! RŰ�� �������ϼ���.");
            return false;
        }
        if (isReloading)
        {
            Debug.Log("������ ��...");
            return false;
        }
        return true;
    }

    IEnumerator BasicAttack()
    {
        if (!CanShoot()) yield break;
        if (Time.time < lastAttackTime + attackCooldown) yield break;

        lastAttackTime = Time.time;
        currentAmmo--;
        Debug.Log($"�߻� | ���� źȯ: {currentAmmo}/{maxAmmo}");

        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            isShootingLocked = true;
            Debug.Log("źâ ����! RŰ�� �������ϼ���.");
        }

        SkillHUDManager.Instance?.TriggerCooldown(SkillType.BasicAttack, attackCooldown);

        playerSystem.playerAnimator.SetBool("isGun", true);
        yield return new WaitForSeconds(0.2f);

        gameSoundManager.OnFindPlayerSound("�÷��̾� �⺻����");
        ShootProjectile(bulletPrefab, firePoint, bulletSpeed);

        yield return new WaitForSeconds(0.1f);
        playerSystem.playerAnimator.SetBool("isGun", false);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log($"������ ��... ({reloadTime}��)");

        SkillHUDManager.Instance?.TriggerCooldown(SkillType.Reload, reloadTime);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        isShootingLocked = false;
        Debug.Log($"������ �Ϸ�! źȯ: {currentAmmo}/{maxAmmo}");
    }

    private IEnumerator ComboShot()
    {
        if (!CanShoot()) yield break;
        if (Time.time < lastAttackTime + attackCooldown) yield break;

        isComboShotRunning = true;
        lastAttackTime = Time.time;
        currentAmmo--;
        Debug.Log($"�޺� �߻� | ���� źȯ: {currentAmmo}/{maxAmmo}");

        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            isShootingLocked = true;
            Debug.Log("źâ ����! RŰ�� �������ϼ���.");
        }

        SkillHUDManager.Instance?.TriggerCooldown(SkillType.BasicAttack, attackCooldown);

        yield return new WaitForSeconds(comboShotDelay);
        playerSystem.playerAnimator.SetBool("isGun", true);

        yield return new WaitForSeconds(0.2f);
        gameSoundManager.OnFindPlayerSound("�÷��̾� �⺻����");
        ShootProjectile(bulletPrefab, firePoint, bulletSpeed);

        yield return new WaitForSeconds(0.1f);
        playerSystem.playerAnimator.SetBool("isGun", false);
        Debug.Log($"�޺�ź �߻� / ������ {comboDamage}");

        isComboShotRunning = false;
    }

    private void ShootProjectile(GameObject prefab, Transform spawnPoint, float speed)
    {
        if (prefab == null || spawnPoint == null || mainCamera == null) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 dir = (mousePos - spawnPoint.position).normalized;

        GameObject bullet = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = dir * speed;

        Destroy(bullet, bulletLifetime);
    }

    private void ThrowWeapon()
    {
        if (Time.time < lastThrowTime + throwCooldown) return;
        if (throwWeaponPrefab == null || throwPoint == null || mainCamera == null) return;

        lastThrowTime = Time.time;
        ShootProjectile(throwWeaponPrefab, throwPoint, throwSpeed);

        // ��ô ��Ÿ�� HUD
        SkillHUDManager.Instance?.TriggerCooldown(SkillType.Throw, throwCooldown);
        Debug.Log("��ô���� ��� / ������ 22");
    }

    private void MeleeAttack(int damage)
    {
        if (Time.time < lastMeleeTime + meleeCooldown) return;
        if (meleePoint == null) return;

        lastMeleeTime = Time.time;

        Collider2D[] hits = Physics2D.OverlapCircleAll(meleePoint.position, meleeRange, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            EnemyHelthSystem enemy = hit.GetComponent<EnemyHelthSystem>();
            if (enemy == null) enemy = hit.GetComponentInParent<EnemyHelthSystem>();
            if (enemy != null)
                ApplyDamage(enemy, damage);
        }

        SkillHUDManager.Instance?.TriggerCooldown(SkillType.Melee, meleeCooldown);
        Debug.Log("�������� / ������ 10");
    }

    private void TryParry()
    {
        if (Time.time < lastParryTime + parryCooldown) return;
        StartCoroutine(ParryRoutine());
    }

    private IEnumerator ParryRoutine()
    {
        lastParryTime = Time.time;
        isParryWindow = true;
        Debug.Log("�и� ����");
        gameSoundManager.OnFindPlayerSound("�и�");

        SkillHUDManager.Instance?.TriggerCooldown(SkillType.Parry, parryCooldown);

        yield return new WaitForSeconds(parryDuration);

        isParryWindow = false;
        Debug.Log("�и� ����");
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
            ApplyDamage(enemy, parryDamage);

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (meleePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleePoint.position, meleeRange);
        }
    }
}
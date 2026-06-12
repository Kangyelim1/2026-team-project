using System.Collections;
using UnityEngine;

public class PlayerItemSystem : MonoBehaviour
{
    [Header("Required References")]
    public PlayerAttackSystem attackSystem;
    public PlayerSystem playerSystem;

    [Header("Rapid Gun Settings")]
    public float rapidFireRate = 0.1f;
    public float rapidBulletSpeed = 15f;
    public int rapidGunMaxAmmo = 30;
    public int rapidGunDamage = 3;

    public bool hasRapidGun = false;
    private int currentRapidAmmo = 0;
    private bool isRapidFiring = false;

    public bool hasShieldCharm = false;

    private void Awake()
    {
        if (attackSystem == null)
            attackSystem = GetComponent<PlayerAttackSystem>();
        if (playerSystem == null)
            playerSystem = GetComponent<PlayerSystem>();
    }

    private void Update()
    {
        if (hasRapidGun && Input.GetMouseButton(0))
        {
            if (!isRapidFiring)
                StartCoroutine(RapidFire());
        }
    }

    public void PickupItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.RapidGun:
                ActivateRapidGun();
                break;
            case ItemType.ShieldCharm:
                ActivateShieldCharm();
                break;
        }
    }

    private void ActivateRapidGun()
    {
        hasRapidGun = true;
        currentRapidAmmo = rapidGunMaxAmmo;

        attackSystem.isShootingLocked = true;

        Debug.Log("Item: Rapid Gun activated! Ammo: " + currentRapidAmmo);
    }

    private IEnumerator RapidFire()
    {
        isRapidFiring = true;

        if (currentRapidAmmo <= 0)
        {
            DeactivateRapidGun();
            yield break;
        }

        currentRapidAmmo--;
        Debug.Log("Rapid Gun ammo: " + currentRapidAmmo + " / " + rapidGunMaxAmmo);

        if (attackSystem.firePoint != null && attackSystem.bulletPrefab != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;

                Vector2 dir = (mousePos - attackSystem.firePoint.position).normalized;

                GameObject bullet = Instantiate(
                    attackSystem.bulletPrefab,
                    attackSystem.firePoint.position,
                    Quaternion.identity);

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = dir * rapidBulletSpeed;

                BulletSystem bs = bullet.GetComponent<BulletSystem>();
                if (bs != null) bs.damage = rapidGunDamage;

                Destroy(bullet, 0.4f);
            }
        }

        if (currentRapidAmmo <= 0)
            DeactivateRapidGun();

        yield return new WaitForSeconds(rapidFireRate);
        isRapidFiring = false;
    }

    private void DeactivateRapidGun()
    {
        hasRapidGun = false;
        currentRapidAmmo = 0;
        isRapidFiring = false;

        attackSystem.isShootingLocked = false;
        attackSystem.currentAmmo = attackSystem.maxAmmo;

        Debug.Log("Item: Rapid Gun expired. Back to default weapon. Ammo refilled.");
    }

    private void ActivateShieldCharm()
    {
        if (hasShieldCharm)
        {
            Debug.Log("Item: Shield Charm already active!");
            return;
        }

        hasShieldCharm = true;
        Debug.Log("Item: Shield Charm activated! Next hit will be blocked.");
    }

    public bool TryBlockHit()
    {
        if (!hasShieldCharm) return false;

        hasShieldCharm = false;
        Debug.Log("Item: Shield Charm blocked 1 hit!");
        return true;
    }

    public int RapidAmmoLeft => currentRapidAmmo;
    public bool HasRapidGun => hasRapidGun;
    public bool HasShieldCharm => hasShieldCharm;
}
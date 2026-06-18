using System.Collections;
using UnityEngine;

public enum GunType { Default, Special }

public class GunSystem : MonoBehaviour
{
    public static GunSystem Instance;

    [Header("기본 총 설정")]
    public int defaultMaxAmmo = 6;
    public float defaultReloadTime = 1.5f;

    [Header("특수 총 설정 (아이템)")]
    public int specialMaxAmmo = 30;

    [Header("현재 상태 (읽기 전용)")]
    public GunType currentGun = GunType.Default;
    public int currentAmmo;
    public int maxAmmo;

    private bool isReloading = false;

    public static event System.Action<int, int, bool> OnAmmoChanged;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        EquipDefault();
    }

    public void EquipDefault()
    {
        StopAllCoroutines();
        currentGun = GunType.Default;
        maxAmmo = defaultMaxAmmo;
        currentAmmo = defaultMaxAmmo;
        isReloading = false;
        NotifyUI();
    }

    public void EquipSpecial()
    {
        StopAllCoroutines();
        currentGun = GunType.Special;
        maxAmmo = specialMaxAmmo;
        currentAmmo = specialMaxAmmo;
        isReloading = false;
        NotifyUI();
    }

    public bool TryShoot()
    {
        if (isReloading) return false;
        if (currentAmmo <= 0) return false;

        currentAmmo--;
        NotifyUI();

        if (currentGun == GunType.Special && currentAmmo <= 0)
        {
            Invoke(nameof(EquipDefault), 0.1f);
            return true;
        }

        if (currentGun == GunType.Default && currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

        return true;
    }

    public void ManualReload()
    {
        if (currentGun == GunType.Special) return; 
        if (isReloading) return;
        if (currentAmmo == maxAmmo) return;
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        isReloading = true;
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo, true); 

        yield return new WaitForSeconds(defaultReloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        NotifyUI();
    }

    void NotifyUI()
    {
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo, false);
    }
}
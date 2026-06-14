using UnityEngine;
using TMPro;

public class BulletUI : MonoBehaviour
{
    [Header("UI 텍스트 연결")]
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI reloadText;

    void OnEnable()
    {
        AmmoEvents.OnAmmoChanged += UpdateUI;
    }

    void OnDisable()
    {
        AmmoEvents.OnAmmoChanged -= UpdateUI;
    }

    void Start()
    {
        PlayerAttackSystem atk = FindAnyObjectByType<PlayerAttackSystem>();
        if (atk != null && ammoText != null)
            ammoText.text = $"{atk.currentAmmo}/{atk.maxAmmo}";

        if (reloadText != null)
            reloadText.gameObject.SetActive(false);
    }

    void UpdateUI(int current, int max, bool isReloading)
    {
        if (ammoText != null)
            ammoText.text = $"{current}/{max}";

        if (reloadText != null)
            reloadText.gameObject.SetActive(isReloading);
    }
}
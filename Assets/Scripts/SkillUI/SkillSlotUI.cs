using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [Header("UI ¿¬°á")]
    public Image cooldownMask;
    public TextMeshProUGUI cooldownText;
    public Image skillIcon;

    private float totalCooldown;
    private float remainTime;
    private bool isCoolingDown = false;

    private void Update()
    {
        if (!isCoolingDown) return;

        remainTime -= Time.deltaTime;

        if (remainTime <= 0f)
        {
            remainTime = 0f;
            isCoolingDown = false;
            cooldownMask.fillAmount = 0f;
            cooldownText.text = "";
            skillIcon.color = Color.white;
            return;
        }

        cooldownMask.fillAmount = remainTime / totalCooldown;
        cooldownText.text = remainTime > 1f
            ? Mathf.CeilToInt(remainTime).ToString()
            : remainTime.ToString("F1"); 
    }

    public void StartCooldown(float cooldown)
    {
        totalCooldown = cooldown;
        remainTime = cooldown;
        isCoolingDown = true;
        skillIcon.color = new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    public bool IsReady => !isCoolingDown;
}
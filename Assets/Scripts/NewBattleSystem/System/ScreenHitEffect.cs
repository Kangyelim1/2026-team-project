using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenHitEffect : MonoBehaviour
{
    public Image damageFlash;

    public void PlayerHitFlash()
    {
        damageFlash.color = new Color(1, 0, 0, 0);

        Sequence seq = DOTween.Sequence();

        seq.Append(damageFlash.DOFade(0.35f, 0.08f));

        seq.Append(damageFlash.DOFade(0f, 0.25f));
    }
}

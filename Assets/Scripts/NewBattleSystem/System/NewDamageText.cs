using TMPro;
using UnityEngine;
using DG.Tweening;

public class NewDamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    public void ShowDamage(int damage, AttackType type)
    {
        damageText.text = damage.ToString();

        if (AttackType.Hit == type) damageText.color = Color.red;
   
        if(AttackType.Hill == type) damageText.color = Color.green;

        if(AttackType.Defense == type) damageText.color = Color.yellow;

        if(AttackType.Attack == type) damageText.color = Color.white;


        RectTransform rect = GetComponent<RectTransform>();

        Vector3 startPos = rect.position;

        rect.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOScale(1f, 0.08f));

        seq.Append(rect.DOScale(1f, 0.06f));

        seq.Join(rect.DOMoveY(startPos.y + 70f, 0.7f));

        seq.Insert(0.2f, damageText.DOFade(0f, 0.5f));

        seq.OnComplete(() => { Destroy(gameObject);});
    }
}

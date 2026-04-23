using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float duration = 1.0f;

    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetDamage(int damage)
    {
        text.text = "-" + damage.ToString();

        StopAllCoroutines(); // 기존 거 정리
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.localPosition += Vector3.up * moveSpeed * Time.unscaledDeltaTime;

            Color c = text.color;
            c.a = Mathf.Lerp(1, 0, timer / duration);
            text.color = c;

            timer += Time.unscaledDeltaTime; // 여기 중요
            yield return null;
        }

        Destroy(gameObject);

        Debug.Log("삭제됨");
        Destroy(gameObject);
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class DummySystem : MonoBehaviour
{
    [Header("HIT 텍스트")]
    public GameObject hitText;

    private void Start()
    {
        if (hitText != null)
            hitText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet"))
            return;

        Debug.Log("허수아비 피격");

        StopAllCoroutines();
        StartCoroutine(ShowHitText());
    }

    private IEnumerator ShowHitText()
    {
        hitText.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        hitText.SetActive(false);
    }
}
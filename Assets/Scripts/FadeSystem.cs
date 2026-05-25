using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;

    private void Start()
    {
        fadeImage.gameObject.SetActive(true);
    }
    public void StartFadeIn(float duration)
    {
        StartCoroutine(Fade(1f, 0f, duration));
    }


    public void StartFadeOut(float duration)
    {
        StartCoroutine(Fade(0f, 1f, duration));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        Color tempColor = fadeImage.color;

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            tempColor.a = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            fadeImage.color = tempColor;
            yield return null;
        }

        tempColor.a = endAlpha;
        fadeImage.color = tempColor;
    }
}
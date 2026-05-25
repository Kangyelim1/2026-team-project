using System.Collections;
using UnityEngine;

public class GameManger: MonoBehaviour
{
    public static GameManger Instance;
    public GameObject SavePoint;
    public GameObject PlayerPrefab;
    public FadeManager fadeManger;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        fadeManger = Object.FindAnyObjectByType<FadeManager>();
    }

    public IEnumerator DiePlayer()
    {
        fadeManger.StartFadeOut(0.5f);
        yield return new WaitForSeconds(0.5f);
        if (PlayerPrefab == null) yield break;
        Instantiate(PlayerPrefab, SavePoint.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        fadeManger.StartFadeIn(0.5f);
    }

}

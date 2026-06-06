using System.Collections;
using UnityEngine;

public class BombSystem : MonoBehaviour
{
    public GameObject bombPoint;

    private void Start()
    {
        StartCoroutine(Bomb());
    }

    IEnumerator Bomb()
    {
        yield return new WaitForSeconds(0.5f);
        bombPoint.SetActive(true);
    }
}

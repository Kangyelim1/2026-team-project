using UnityEngine;

public class DummySystem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("허수아비 충돌");
    }
}
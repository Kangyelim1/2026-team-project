using Unity.VisualScripting;
using UnityEngine;

public class DestoryObejct : MonoBehaviour
{
    public int DownForce;
    public Rigidbody2D rb;

    public void Start()
    {
        rb.AddForceY(-DownForce, ForceMode2D.Force);
    }
}

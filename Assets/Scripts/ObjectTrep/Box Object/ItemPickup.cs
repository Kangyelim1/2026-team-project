using UnityEngine;

public enum ItemType
{
    RapidGun,
    ShieldCharm
}

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;

    [Header("Bob Animation")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.15f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float y = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerItemSystem itemSystem = collision.GetComponentInParent<PlayerItemSystem>();
        if (itemSystem == null)
            itemSystem = collision.GetComponent<PlayerItemSystem>();

        if (itemSystem != null)
        {
            itemSystem.PickupItem(itemType);
            Destroy(gameObject);
        }
    }
}
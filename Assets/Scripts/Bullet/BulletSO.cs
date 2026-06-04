using UnityEngine;
public enum BulletType
{
    PlayerBullet,
    EnemyBullet,
    Missile
}

[CreateAssetMenu(fileName = "BulletSO", menuName = "ScriptableObject/Bullet")]
public class BulletSO : ScriptableObject
{
    public float bulletRifeTime;
    public BulletType bulletType;
}

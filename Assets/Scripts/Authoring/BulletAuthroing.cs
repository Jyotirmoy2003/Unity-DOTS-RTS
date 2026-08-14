using Unity.Entities;
using UnityEngine;

public class BulletAuthroing : MonoBehaviour
{
    public float speed;
    public int damageAmount;
    public class Baker : Baker<BulletAuthroing>
    {
        public override void Bake(BulletAuthroing authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet
            {
                speed = authoring.speed,
                damageAmount = authoring.damageAmount,
            });
        }
    }
}

public struct Bullet : IComponentData
{
    public float speed;
    public int damageAmount;
}

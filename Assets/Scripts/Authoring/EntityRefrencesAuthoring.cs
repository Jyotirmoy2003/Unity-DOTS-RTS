using Unity.Entities;
using UnityEngine;

public class EntityRefrencesAuthoring : MonoBehaviour
{
    public GameObject bulletPrefabGameobject;
    public GameObject zombiePrefabGameobject;
    public class Baker : Baker<EntityRefrencesAuthoring>
    {
        public override void Bake(EntityRefrencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityRefrences
            {
                bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameobject,TransformUsageFlags.Dynamic),
                zombiePrefabEntity = GetEntity(authoring.zombiePrefabGameobject,TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct EntityRefrences : IComponentData
{
    public Entity bulletPrefabEntity;
    public Entity zombiePrefabEntity;
}

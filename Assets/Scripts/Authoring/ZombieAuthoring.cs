using Unity.Entities;
using UnityEngine;

public class ZombieAuthoring : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class Baker : Baker<ZombieAuthoring>
    {
        public override void Bake(ZombieAuthoring authoring)
        {
            Entity entity= GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new Zombie());
        }
    }
}

public struct Zombie : IComponentData
{
    
}
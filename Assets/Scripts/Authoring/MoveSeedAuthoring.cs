using Unity.Entities;
using UnityEngine;

public class MoveSeedAuthoring : MonoBehaviour
{
    public float value ;

    public class Baker : Baker<MoveSeedAuthoring>
    {
        public override void Bake(MoveSeedAuthoring authoring)
        {
            Entity entity= GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new MoveSpeed
            {
                value =  authoring.value,
            });
        }
    }
}

public struct MoveSpeed : IComponentData
{
    public float value;
}


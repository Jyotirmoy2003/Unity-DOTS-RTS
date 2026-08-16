using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RandomWalkingAuthoring : MonoBehaviour
{
    public float3 targetPoisition;
    public float3 originPosition;
    public float distanceMin;
    public float distanceMax;
    public class Baker : Baker<RandomWalkingAuthoring>
    {
        public override void Bake(RandomWalkingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new RandomWalking
            {
                targetPoisition = authoring.targetPoisition,
                originPosition = authoring.originPosition,
                distanceMin = authoring.distanceMin,
                distanceMax = authoring.distanceMax,
                random = new Unity.Mathematics.Random(1),
            });
        }
    }
}

public struct RandomWalking : IComponentData
{
    public float3 targetPoisition;
    public float3 originPosition;
    public float distanceMin;
    public float distanceMax;
    public Unity.Mathematics.Random random;
}

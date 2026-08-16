using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ZombieSpwanerAuthoring : MonoBehaviour
{
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;


    public class Baker : Baker<ZombieSpwanerAuthoring>
    {
        public override void Bake(ZombieSpwanerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpwaner
            {
                timerMax = authoring.timerMax,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
            });
        }
    }
}

public struct ZombieSpwaner : IComponentData
{
    public float timer;
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
}

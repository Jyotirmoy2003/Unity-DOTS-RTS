using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

partial struct ZombieSpawnerySystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityRefrences entityRefrences = SystemAPI.GetSingleton<EntityRefrences>();
        EntityCommandBuffer entityCommandBuffer =  SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        
        foreach((RefRO<LocalTransform> localTransform, RefRW<ZombieSpwaner> zombieSpawner) 
        in SystemAPI.Query<RefRO<LocalTransform>,RefRW<ZombieSpwaner>>())
        {
            
            zombieSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(zombieSpawner.ValueRO.timer >0f)
            {
                continue;
            }

            zombieSpawner.ValueRW.timer = zombieSpawner.ValueRO.timerMax;

            Entity zombieEntity = state.EntityManager.Instantiate(entityRefrences.zombiePrefabEntity);
            SystemAPI.SetComponent(zombieEntity,LocalTransform.FromPosition(localTransform.ValueRO.Position));

            entityCommandBuffer.AddComponent(zombieEntity, new RandomWalking
            {
                originPosition =localTransform.ValueRO.Position,
                targetPoisition =localTransform.ValueRO.Position,
                distanceMin = zombieSpawner.ValueRO.randomWalkingDistanceMin,
                distanceMax = zombieSpawner.ValueRO.randomWalkingDistanceMax,
                random = new Unity.Mathematics.Random((uint)zombieEntity.Index),

            });
        }
    }

    
}

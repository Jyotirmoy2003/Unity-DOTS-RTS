using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using UnityEngine;

partial struct FindTargetSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;



        NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

        foreach((RefRO<LocalTransform> localTransform,RefRW<FindTarget> findTarget, RefRW<Target> target) in 
        SystemAPI.Query<RefRO<LocalTransform> ,  RefRW<FindTarget>,RefRW<Target>>())
        {
            
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(findTarget.ValueRO.timer > 0f)
            {
                //Timer not elapsed
                continue;
            }
            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;
            
            
            distanceHits.Clear();
            CollisionFilter collisonFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAsssets.UNITS_LAYER,
                GroupIndex = 0,
            }; 


            if(collisionWorld.OverlapSphere(localTransform.ValueRO.Position,findTarget.ValueRO.range,ref distanceHits,collisonFilter))
            {
                foreach(DistanceHit distanceHit in distanceHits)
                {
                    Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                    if(targetUnit.faction == findTarget.ValueRO.targetFaction)
                    {
                        //Valid target;
                        target.ValueRW.targetEntity = distanceHit.Entity;
                        break;
                    }

                    
                }
            }

        }
    }

    
}

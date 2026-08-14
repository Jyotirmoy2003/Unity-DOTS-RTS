using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoverSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach((RefRW<LocalTransform> localTransform, RefRO<Bullet> bullet, RefRO<Target> target, Entity entity) in 
        SystemAPI.Query<RefRW<LocalTransform>,RefRO<Bullet>,RefRO<Target>>().WithEntityAccess())
        {
            if(target.ValueRO.targetEntity ==Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                break;
            }
            LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position,targetTransform.Position);
            
            float3 moveDirection = targetTransform.Position - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);


            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position,targetTransform.Position);
            if(distanceAfterSq >distanceBeforeSq)
            {
                localTransform.ValueRW.Position = targetTransform.Position;
            }
            float destroyDistanceSq= .2f;
            if(math.distancesq(localTransform.ValueRO.Position,targetTransform.Position )<= destroyDistanceSq)
            {
                //clsoe enough to damage target
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                int damageAmount = 1;
                targetHealth.ValueRW.healthAmount -= damageAmount;
            
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }

}

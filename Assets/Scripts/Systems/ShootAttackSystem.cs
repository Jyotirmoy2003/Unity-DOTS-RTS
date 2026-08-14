using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

partial struct ShootAttackSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityRefrences entityRefrences = SystemAPI.GetSingleton<EntityRefrences>();

        foreach((RefRO<LocalTransform> localTransofrom,RefRW<ShootAttack> shootAttack, RefRO<Target> target)
         in SystemAPI.Query<RefRO<LocalTransform>,RefRW<ShootAttack>,RefRO<Target>>())
        {
            if(target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }

            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(shootAttack.ValueRO.timer > 0f)
            {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;


           Entity bulletEntity = state.EntityManager.Instantiate(entityRefrences.bulletPrefabEntity);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(localTransofrom.ValueRO.Position));
            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        
        
        }
    }

   
}

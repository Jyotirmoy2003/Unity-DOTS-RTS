using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct ShootAttackSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityRefrences entityRefrences = SystemAPI.GetSingleton<EntityRefrences>();

        foreach((RefRW<LocalTransform> localTransofrom,RefRW<ShootAttack> shootAttack, RefRO<Target> target,RefRW<UnitMover> unitMover)
         in SystemAPI.Query<RefRW<LocalTransform>,RefRW<ShootAttack>,RefRO<Target>,RefRW<UnitMover>>())
        {
            if(target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            if((math.distance(localTransofrom.ValueRO.Position,targetLocalTransform.Position)> shootAttack.ValueRO.attackDistance))
            {
                //move closer
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                continue;
            }
            else
            {
                //Close enough stop moving and attack
                unitMover.ValueRW.targetPosition = localTransofrom.ValueRO.Position;
            }
            //rotation of units
            float3 aimDirection = targetLocalTransform.Position - localTransofrom.ValueRO.Position;
            aimDirection = math.normalize(aimDirection);

            quaternion targetRotation = quaternion.LookRotation(aimDirection,math.up());

            localTransofrom.ValueRW.Rotation = math.slerp(localTransofrom.ValueRO.Rotation,targetRotation,SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

            //Shoot unit
            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(shootAttack.ValueRO.timer > 0f)
            {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

            
           Entity bulletEntity = state.EntityManager.Instantiate(entityRefrences.bulletPrefabEntity);
            float3 bulletSpawnWorldPosition = localTransofrom.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpwanLocalPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));
            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        
        
        }
    }

   
}

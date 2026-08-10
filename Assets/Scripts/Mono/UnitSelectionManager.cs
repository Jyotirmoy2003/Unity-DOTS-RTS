using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;

public class UnitSelectionManager : MonoSingleton<UnitSelectionManager>
{
    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;
    private Vector2 selectionStartMousePosition;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition =Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this,EventArgs.Empty);
        }

        if(Input.GetMouseButtonUp(0))
        {
            Vector2 selectionEndMousePosition = Input.mousePosition;

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //Deselect all entity
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
            NativeArray<Entity>entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            for(int i=0;i<entityArray.Length;i++)
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[i],false);
            }


            Rect selectionArea = GetSelectedAreaRect();
            float selectionAreaSize = selectionArea.width + selectionArea.height;
            float multipleSelectionSizeMin = 40f;

            bool isMultipleSelection = selectionAreaSize >multipleSelectionSizeMin;
            if(isMultipleSelection)
            {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform,Unit>().WithPresent<Selected>().Build(entityManager);

                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                
                for(int i =0 ;i<localTransformArray.Length;i++)
                {
                    LocalTransform localTransform = localTransformArray[i];
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(localTransform.Position);
                    if(selectionArea.Contains(unitScreenPosition)) //Unit is inside the selection area
                    {
                        entityManager.SetComponentEnabled<Selected>(entityArray[i],true);
                    }
                }
            }
            else
            {
                //Single Select
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                int unitLayer = 6;
                RaycastInput raycastInput = new RaycastInput
                {
                   Start = cameraRay.GetPoint(0f),
                   End = cameraRay.GetPoint(999f),
                   Filter = new CollisionFilter
                   {
                       BelongsTo = ~0u,
                       CollidesWith = 1u << unitLayer,
                       GroupIndex = 0,
                   } 
                };
                if(collisionWorld.CastRay(raycastInput,out Unity.Physics.RaycastHit raycastHit))
                {
                    if(entityManager.HasComponent<Unit>(raycastHit.Entity))
                    {
                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity,true);
                    }
                }

            }


            

            OnSelectionAreaEnd?.Invoke(this,EventArgs.Empty);
        }
        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover,Selected>().Build(entityManager);
        
            NativeArray<Entity>entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            
            NativeArray<float3> movePositionArray = GenerateMovePositionArray(mouseWorldPosition,entityArray.Length);
            for(int i =0 ;i<unitMoverArray.Length;i++)
            {
                UnitMover unitMover = unitMoverArray[i];
                unitMover.targetPosition =movePositionArray[i];
                unitMoverArray[i] = unitMover;
            }

            entityQuery.CopyFromComponentDataArray(unitMoverArray);



        }
    }


    public Rect GetSelectedAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(
            Mathf.Min(selectionStartMousePosition.x,selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y,selectionEndMousePosition.y)
        );

        Vector2 upperRightCorner = new Vector2(
            Mathf.Max(selectionStartMousePosition.x,selectionEndMousePosition.x),
            Mathf.Max(selectionStartMousePosition.y,selectionEndMousePosition.y)
        );


        return new Rect(
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upperRightCorner.x-lowerLeftCorner.x,
            upperRightCorner.y-lowerLeftCorner.y
        );
    }


    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition,int positionCount)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount,Allocator.Temp);
        if(positionCount <= 0)
        {
            return positionArray;
        }

        positionArray[0] = targetPosition;
        if(positionCount == 1) return positionArray;

        float ringSize = 2.2f;
        int ring = 0;
        int positionIndex = 1;

        while (positionIndex < positionCount)
        {
            int ringPositionCount = 3+ ring * 2;

            for(int i=0;i<ringPositionCount;i++)
            {
                float angle = i * (math.PI2/ringPositionCount);
                float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                float3 ringPosition = targetPosition + ringVector;
                

                positionArray[positionIndex] = ringPosition;
                positionIndex ++;

                if(positionIndex >= positionCount)
                {
                    break;
                }
            }

            ring++;
        }

        return positionArray;


    }




}

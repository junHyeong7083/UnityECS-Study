using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;


namespace HelloCube.Reparenting
{
    public partial struct ReparentingSystem : ISystem
    {
        bool attached;
        float timer;
        const float interval = 0.7f;

        [BurstCompile]
        public void OnCreate(ref SystemState state )
        {
            timer = interval;
            attached = true;
            state.RequireForUpdate<Execute.Reparenting>();
            state.RequireForUpdate<RotationSpeed>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            timer -= SystemAPI.Time.DeltaTime;
            if (timer > 0) return;

            timer = interval;
            var rotatorEntity = SystemAPI.GetSingletonEntity<RotationSpeed>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            // EntityCommandBuffer 이 뭔지 공부해야함
            /// 엔티티와 컴포넌트의 생성, 삭제, 추가, 수정 등을 예약하기 위한 버퍼
            /// EntityManager을 사용하면 다른 시스템이 데이터를 사용할 때 충돌 발생 가능!!
            /// 변경작업을 버퍼에 기록 => 안전한 시점에 실행해서 충돌을 방지가능 

            /// ecb 생성 : new EntityCommandBuffer(Allocator.Temp)
            /// 기록 : ecb.AddComponent() , ecb.RemoveComponent()
            /// 실행 : ecb.Playback(EntityManager)

            if (attached)
            {
                // buffer이 뭐지?
                // dynamicbuffer????
                DynamicBuffer<Child> children = SystemAPI.GetBuffer<Child>(rotatorEntity);
                for (int i = 0; i < children.Length; i++)
                {     
                    ecb.RemoveComponent<Parent>(children[i].Value);
                }

            }
            else
            {
                foreach (var (transform, entity) in
                         SystemAPI.Query<RefRO<LocalTransform>>()
                             .WithNone<RotationSpeed>()
                             // 쿼리한 엔티티의 참조
                             .WithEntityAccess()) 
                {
                    ecb.AddComponent(entity, new Parent { Value = rotatorEntity });
                }

  
            }

            ecb.Playback(state.EntityManager);

            attached = !attached;
        }
    }
}
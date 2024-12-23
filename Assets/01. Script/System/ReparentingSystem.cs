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
            // EntityCommandBuffer �� ���� �����ؾ���
            /// ��ƼƼ�� ������Ʈ�� ����, ����, �߰�, ���� ���� �����ϱ� ���� ����
            /// EntityManager�� ����ϸ� �ٸ� �ý����� �����͸� ����� �� �浹 �߻� ����!!
            /// �����۾��� ���ۿ� ��� => ������ ������ �����ؼ� �浹�� �������� 

            /// ecb ���� : new EntityCommandBuffer(Allocator.Temp)
            /// ��� : ecb.AddComponent() , ecb.RemoveComponent()
            /// ���� : ecb.Playback(EntityManager)

            if (attached)
            {
                // buffer�� ����?
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
                             // ������ ��ƼƼ�� ����
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
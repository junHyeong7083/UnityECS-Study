using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace HelloCube.MainThread
{
    public partial struct RotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.MainThread>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, speed) in
                SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>())
            {
                transform.ValueRW = transform.ValueRO.RotateY(
                    speed.ValueRO.RadiansPerSecond * deltaTime);
            }
        }
    }


}

namespace HelloCube.JobEntity
{
    public partial struct RotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.IJobEntity>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new RotationJob { deltaTime = SystemAPI.Time.DeltaTime };
            
            job.Schedule();

            // state.Dependency = job.Schedule(state.Dependency);
        }

      
    }

    [BurstCompile]
    partial struct RotationJob : IJobEntity
    {
        public float deltaTime;
        void Execute(ref LocalTransform transform, in RotationSpeed speed)
        {
            transform = transform.RotateY(speed.RadiansPerSecond * deltaTime);
        }
    }
}

namespace HelloCube.Aspects
{
    public partial struct RotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.Aspects>();  
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;  // �� �������� ��� �ð��� �����ɴϴ�.
            double elapsedTime = SystemAPI.Time.ElapsedTime;  // ��ü ��� �ð��� �����ɴϴ�.

            // ȸ�� ó��
            foreach (var (transform, speed) in
                SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>())  
            {
                transform.ValueRW = transform.ValueRO.RotateY(speed.ValueRO.RadiansPerSecond * deltaTime);  
            }

            // ���� �̵� ó��
            foreach (var movement in SystemAPI.Query<VerticalMovementAspect>())  
            {
                movement.Move(elapsedTime); 
            }
        }

    }

    readonly partial struct VerticalMovementAspect : IAspect
    {
        readonly RefRW<LocalTransform> m_Transform;
        readonly RefRO<RotationSpeed> m_Speed;

        public void Move(double elapsedTime)
        {
            m_Transform.ValueRW.Position.y = (float)math.sin(elapsedTime * m_Speed.ValueRO.RadiansPerSecond);
        }
    }

}

namespace HelloCube.JobChunk
{
    public partial struct RotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.IJobChunk>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spinningCubesQuery = SystemAPI.QueryBuilder().WithAll<RotationSpeed, LocalTransform>().Build();

            var job = new RotationJob
            {
                TransformTypeHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>(), // readonly false
                RotationSpeedTypeHandle = SystemAPI.GetComponentTypeHandle<RotationSpeed>(true), // readonly true
                DeltaTime = SystemAPI.Time.DeltaTime
            };  
            state.Dependency = job.Schedule(spinningCubesQuery, state.Dependency);
        }
    }

    [BurstCompile]
    struct RotationJob : IJobChunk
    {
        public ComponentTypeHandle<LocalTransform> TransformTypeHandle;
        [ReadOnly] public ComponentTypeHandle<RotationSpeed> RotationSpeedTypeHandle;
        public float DeltaTime;


        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            Assert.IsFalse(useEnabledMask); // ��� ��ƼƼ�� �۾� ������� ����
          //  Assert.IsTrue(useEnabledMask); // Ȱ��ȭ�� ��ƼƼ�� ó��

            var transforms = chunk.GetNativeArray(ref TransformTypeHandle);
            var rotationSpeeds = chunk.GetNativeArray(ref RotationSpeedTypeHandle);
            for (int i = 0, chunkEntityCount = chunk.Count; i < chunkEntityCount; i++)
            {
                transforms[i] = transforms[i].RotateY(rotationSpeeds[i].RadiansPerSecond * DeltaTime);
            }
        }

    }
}
using Unity.Burst;
using Unity.Entities;

namespace HelloCube.StateChange
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct ProfilerModuleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.StateChange>();
        }

        public void OnUpdate(ref SystemState state)
        {
            ref var frameData = ref SystemAPI.GetSingletonRW<StateChangeProfilerModule.FrameData>().ValueRW;
            StateChangeProfilerModule.SpinPerf = frameData.SpinPerf;
            StateChangeProfilerModule.UpdatePerf = frameData.SetStatePerf;
        }
    }
}
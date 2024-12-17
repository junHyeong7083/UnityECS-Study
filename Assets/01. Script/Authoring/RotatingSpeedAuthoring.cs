using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace HelloCube
{
    public class RotatingSpeedAuthoring : MonoBehaviour
    {
        public float DegressPerSecond = 360.0f;

        class Baker : Baker<RotatingSpeedAuthoring>
        {
            public override void Bake(RotatingSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new RotationSpeed
                {
                    RadiansPerSecond = math.radians(authoring.DegressPerSecond)
                });
            }
        }

    }


    public struct RotationSpeed : IComponentData
    {
        public float RadiansPerSecond;
    }

}
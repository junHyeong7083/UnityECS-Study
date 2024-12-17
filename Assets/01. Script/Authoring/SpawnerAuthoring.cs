using UnityEngine;
using Unity.Entities;

namespace HelloCube.Prefabs
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject CubePrefab;

        class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new Spawner
                {
                    Prefab = GetEntity(authoring.CubePrefab, TransformUsageFlags.None)
                }); 


            }
        }
    }

    struct Spawner : IComponentData
    {
        public Entity Prefab;
    }


}
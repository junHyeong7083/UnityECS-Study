using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace HelloCube.GameObjectSync
{
   public partial struct DirectoryInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.GameObjectSync>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var go = GameObject.Find("Directory");

            if( go == null)
            {
                return;
            }

            var directory = go.GetComponent<DirectoryA>();

            var directoryManaged = new DirecctoryManaged();

            directoryManaged.RotatorPrefab = directory.RotatorPrefab;
            directoryManaged.RotationToggle = directory.RotationToggle;

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, directoryManaged);
        }
    }



    public class DirecctoryManaged : IComponentData
    {
        public GameObject RotatorPrefab;
        public Toggle RotationToggle;

        public DirecctoryManaged()
        {

        }
    }

}
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SansyHuman.Gravity2D
{
    public class Gravity2DSystem : JobComponentSystem
    {
        /*
        [BurstCompile]
        protected override void OnUpdate()
        {
            Entities.ForEach<RigidBody2D>((Entity entity, ref RigidBody2D body) =>
            {
                float2 force = World.GetOrCreateSystem<QuadTree>().CalculateGravity(entity, body.Position, body.Mass);
                body.Velocity += force * body.GravityScale / body.Mass * Time.deltaTime;
            });
        }
        */

        struct Gravity2DCalculateJob : IJobForEachWithEntity<RigidBody2D>
        {
            public float deltaTime;

            public void Execute(Entity entity, int index, ref RigidBody2D body)
            {
                float2 force = QuadTree.Instance.CalculateGravity(entity, body.Position, 1);
                body.Velocity += force * body.GravityScale * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Gravity2DCalculateJob job = new Gravity2DCalculateJob() { deltaTime = Time.deltaTime };
            return job.Schedule(this, inputDeps);
        }
    }
}
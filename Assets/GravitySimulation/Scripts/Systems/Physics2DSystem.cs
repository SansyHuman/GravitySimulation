using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SansyHuman.Gravity2D
{
    public class Physics2DSystem : JobComponentSystem
    {
        [BurstCompile]
        struct RigidBodyMovementJob : IJobForEach<Translation, Rotation, RigidBody2D>
        {
            public float deltaTime;

            public void Execute(ref Translation translation, ref Rotation rotation, ref RigidBody2D body)
            {
                body.Position += body.Velocity * deltaTime;
                body.Rotation += body.AngularVelocity * deltaTime;

                float2 linearDragForce = -body.LinearDrag * math.length(body.Velocity) * body.Velocity;
                body.Velocity += linearDragForce / body.Mass;
                float angularDragForce = -body.AngularDrag * math.pow(body.AngularVelocity, 2);
                body.AngularVelocity += angularDragForce / body.Inertia;

                translation.Value = new float3(body.Position.x, body.Position.y, 0);
                rotation.Value = quaternion.Euler(0, 0, body.Rotation);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var rigidBodyMove = new RigidBodyMovementJob() { deltaTime = Time.deltaTime };
            return rigidBodyMove.Schedule(this, inputDependencies);
        }
    }
}
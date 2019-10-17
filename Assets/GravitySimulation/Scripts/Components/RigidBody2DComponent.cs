using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System;

namespace SansyHuman.Gravity2D
{
    [Serializable]
    public struct RigidBody2D : IComponentData
    {
        public float Mass;
        public float LinearDrag;
        public float AngularDrag;
        public float GravityScale;

        public float2 Position;
        public float Rotation;
        public float2 Velocity;
        public float AngularVelocity;
        public float Inertia;
    }

    [RequireComponent(typeof(TranslationProxy))]
    [RequireComponent(typeof(RotationProxy))]
    [RequireComponent(typeof(LocalToWorldProxy))]
    [RequireComponent(typeof(CopyTransformToGameObjectProxy))]
    public class RigidBody2DComponent : ComponentDataProxy<RigidBody2D> { }
}
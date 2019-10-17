using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using System.Text;

namespace SansyHuman.Gravity2D
{
    public class QuadTree : Singleton<QuadTree>
    {
        public struct Rect
        {
            public float2 bottomLeft;
            public float2 topRight;

            public float2 Midpoint => (bottomLeft + topRight) * 0.5f;
            public float Width => topRight.x - bottomLeft.x;
            public float Height => topRight.y - bottomLeft.y;

            public Rect(float x1, float y1, float x2, float y2)
            {
                bottomLeft = new float2(x1, y1);
                topRight = new float2(x2, y2);
            }

            public Rect(float2 topLeft, float2 bottomRight)
            {
                this.bottomLeft = topLeft;
                this.topRight = bottomRight;
            }

            public bool IsInArea(Entity entity)
            {
                EntityManager manager = World.Active.EntityManager;
                RigidBody2D body = manager.GetComponentData<RigidBody2D>(entity);
                float2 pos = body.Position;

                return pos.x >= bottomLeft.x && pos.x < topRight.x && pos.y >= bottomLeft.y && pos.y < topRight.y;
            }

            public Rect[] SubRects()
            {
                Rect[] rects = new Rect[4];
                float xMid = (bottomLeft.x + topRight.x) * 0.5f;
                float yMid = (bottomLeft.y + topRight.y) * 0.5f;

                rects[0] = new Rect(bottomLeft.x, bottomLeft.y, xMid, yMid);
                rects[1] = new Rect(xMid, bottomLeft.y, topRight.x, yMid);
                rects[2] = new Rect(bottomLeft.x, yMid, xMid, topRight.y);
                rects[3] = new Rect(xMid, yMid, topRight.x, topRight.y);

                return rects;
            }
        }

        public class QuadTreeNode
        {
            public List<Entity> objects;
            public bool isExternal;

            public QuadTreeNode bottomLeft;
            public QuadTreeNode bottomRight;
            public QuadTreeNode topLeft;
            public QuadTreeNode topRight;

            public Rect area;
            public float mass;
            public float2 centerOfMass;

            public QuadTreeNode(Rect area, Entity[] objects) : this(area, objects, WorldSettings.MaximumDepth)
            {
                
            }

            private QuadTreeNode(Rect area, Entity[] objects, int depth)
            {
                this.area = area;
                this.objects = new List<Entity>(objects.Length);
                for (int i = 0; i < objects.Length; i++)
                {
                    if (area.IsInArea(objects[i]))
                        this.objects.Add(objects[i]);
                }

                if (this.objects.Count <= 1 || depth <= 0)
                    isExternal = true;

                mass = 0;
                centerOfMass = float2.zero;

                EntityManager manager = World.Active.EntityManager;
                for (int i = 0; i < this.objects.Count; i++)
                {
                    RigidBody2D body = manager.GetComponentData<RigidBody2D>(this.objects[i]);
                    mass += body.Mass;
                    centerOfMass += body.Mass * body.Position;
                }

                if (this.objects.Count == 0 || mass == 0)
                    centerOfMass = area.Midpoint;
                else
                    centerOfMass /= mass;

                if (!isExternal)
                {
                    Rect[] subRects = area.SubRects();
                    Entity[] entities = this.objects.ToArray();

                    bottomLeft = new QuadTreeNode(subRects[0], entities, depth - 1);
                    bottomRight = new QuadTreeNode(subRects[1], entities, depth - 1);
                    topLeft = new QuadTreeNode(subRects[2], entities, depth - 1);
                    topRight = new QuadTreeNode(subRects[3], entities, depth - 1);
                }
            }

            public float2 CalculateGravity(Entity entity, float2 pos, float mass)
            {
                float2 force = float2.zero;
                float2 displ = pos - centerOfMass;
                float dist = math.length(displ);
                if (dist == 0)
                    dist = 0.001f;

                if (isExternal)
                {
                    if (objects.Count == 0 || (objects.Count == 1 && objects[0].Index == entity.Index))
                        force = float2.zero;
                    else
                        force -= displ * WorldSettings.GravityConstant * this.mass * mass / math.pow(dist, 3);
                }
                else if (math.abs(area.Width / dist) < WorldSettings.Threshold)
                {
                    force -= displ * WorldSettings.GravityConstant * this.mass * mass / math.pow(dist, 3);
                }
                else
                {
                    force += bottomLeft.CalculateGravity(entity, pos, mass);
                    force += bottomRight.CalculateGravity(entity, pos, mass);
                    force += topLeft.CalculateGravity(entity, pos, mass);
                    force += topRight.CalculateGravity(entity, pos, mass);
                }

                return force;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Node objects: ");
                for (int i = 0; i < objects.Count; i++)
                    builder.Append(objects[i].Index);
                if (!isExternal)
                {
                    builder.Append(topLeft.ToString());
                    builder.Append(topRight.ToString());
                    builder.Append(bottomLeft.ToString());
                    builder.Append(bottomRight.ToString());
                }

                return builder.ToString();
            }
        }

        public static QuadTreeNode baseNode;

        private EntityManager manager;

        private void Start() //protected override void OnCreate()
        {
            manager = World.Active.EntityManager;
        }

        class QuadTreeInternal : ComponentSystem
        {
            [BurstCompile]
            protected override void OnUpdate()
            {
                float width = WorldSettings.Width;
                float height = WorldSettings.Height;
                float2 center = WorldSettings.Center;
                float2 bottomLeft = center - new float2(width * 0.5f, height * 0.5f);
                float2 topRight = center + new float2(width * 0.5f, height * 0.5f);
                Rect world = new Rect(bottomLeft, topRight);

                List<Entity> objects = new List<Entity>(1024);
                Entities.ForEach<RigidBody2D>((Entity entity, ref RigidBody2D body) =>
                {
                    objects.Add(entity);
                });

                baseNode = new QuadTreeNode(world, objects.ToArray());
            }
        }

        public float2 CalculateGravity(Entity entity, float2 pos, float mass)
        {
            return baseNode != null ?  baseNode.CalculateGravity(entity, pos, mass) : float2.zero;
        }
    }
}
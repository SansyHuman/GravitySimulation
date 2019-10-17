using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using SansyHuman.Gravity2D;

public class Test : MonoBehaviour
{
    public GameObject cube;
    public int numberOfCube;

    private EntityManager manager;

    private void Awake()
    {
        manager = World.Active.EntityManager;
    }

    private void Start()
    {
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)Time.realtimeSinceStartup);
        for (int i = 0; i < numberOfCube; i++)
        {
            GameObject cube = Instantiate(this.cube);
            RigidBody2DComponent bodyProxy = cube.GetComponent<RigidBody2DComponent>();
            RigidBody2D body = bodyProxy.Value;
            body.Mass = rand.NextFloat(0.1f, 3f);
            body.Position = new float2(rand.NextFloat(-500f, 500f), rand.NextFloat(-500f, 500f));
            float angle = math.atan2(body.Position.y, body.Position.x) + math.PI * 0.5f;
            float speed = math.pow(math.length(body.Position), 2f / 3.5f);
            body.Velocity = new float2(speed * math.cos(angle), speed * math.sin(angle));
            bodyProxy.Value = body;

            cube.transform.parent = transform;
        }
    }
}

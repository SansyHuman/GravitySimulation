using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using SansyHuman;

namespace SansyHuman.Gravity2D
{
    public class WorldSettings : Singleton<WorldSettings>
    {
        [SerializeField] private float width = 10;
        [SerializeField] private float height = 10;
        [SerializeField] private float2 center = float2.zero;
        [SerializeField] private float threshold = 0.5f;
        [SerializeField] [Range(1, 100)] private int maximumDepth = 10;
        [SerializeField] private float gravityConstant = 9.8f;

        public static float Width
        {
            get => WorldSettings.Instance.width;
            set
            {
                WorldSettings.Instance.width = value;
                if (WorldSettings.Instance.width < 0)
                    WorldSettings.Instance.width = 0;
            }
        }

        public static float Height
        {
            get => WorldSettings.Instance.height;
            set
            {
                WorldSettings.Instance.height = value;
                if (WorldSettings.Instance.height < 0)
                    WorldSettings.Instance.height = 0;
            }
        }

        public static float2 Center
        {
            get => WorldSettings.Instance.center;
            set => WorldSettings.Instance.center = value;
        }

        public static float Threshold
        {
            get => WorldSettings.Instance.threshold;
            set
            {
                WorldSettings.Instance.threshold = value;
                if (WorldSettings.Instance.threshold < 0)
                    WorldSettings.Instance.threshold = 0;
            }
        }

        public static int MaximumDepth
        {
            get => WorldSettings.Instance.maximumDepth;
            set
            {
                WorldSettings.Instance.maximumDepth = value;
                if (WorldSettings.Instance.maximumDepth < 1)
                    WorldSettings.Instance.maximumDepth = 1;
                if (WorldSettings.Instance.maximumDepth > 100)
                    WorldSettings.Instance.maximumDepth = 100;
            }
        }

        public static float GravityConstant
        {
            get => WorldSettings.Instance.gravityConstant;
            set => WorldSettings.Instance.gravityConstant = value;
        }
    }
}
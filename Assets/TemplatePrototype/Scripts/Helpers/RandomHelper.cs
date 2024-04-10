using System;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.ParticleSystem;

namespace HyrphusQ.Helpers
{
    using HyrphusQ.Const;

    public static class RandomHelper
    {
        /// <summary>
        /// Prevents infinite loop and crash UnityEditor in while loop
        /// </summary>
        private readonly static int MaxAttempt = 100;

        public static float Random01() =>
            UnityEngine.Random.Range(Const.FloatValue.ZeroF, Const.FloatValue.OneF);
        public static float RandomOpposite(float range) =>
            UnityEngine.Random.Range(-range, range);
        public static int RandomRange(int range) =>
            UnityEngine.Random.Range(-range, range);
        public static float RandomRange(float min, float max) =>
            UnityEngine.Random.Range(min, max);
        public static int RandomRange(int min, int max) =>
            UnityEngine.Random.Range(min, max);
        public static float RandomRange(MinMaxCurve minMaxCurve) =>
            UnityEngine.Random.Range(minMaxCurve.constantMin, minMaxCurve.constantMax);
        public static Vector3 RandomDirection(Axis axisFlag = Axis.X | Axis.Y | Axis.Z)
        {
            var randomDirection = Vector3.zero;
            if ((axisFlag & Axis.X) != 0)
                randomDirection += Vector3.right * RandomOpposite(Const.FloatValue.OneF);
            if ((axisFlag & Axis.Y) != 0)
                randomDirection += Vector3.up * RandomOpposite(Const.FloatValue.OneF);
            if ((axisFlag & Axis.Z) != 0)
                randomDirection += Vector3.forward * RandomOpposite(Const.FloatValue.OneF);
            return randomDirection;
        }
        public static Vector3 RandomDirection(Func<Vector3, bool> predicate, Axis axisFlag = Axis.X | Axis.Y | Axis.Z)
        {
            int attempt = Const.IntValue.Zero;
            while (true)
            {
                var randomDirection = RandomDirection(axisFlag);
                if ((predicate?.Invoke(randomDirection) ?? true) || attempt > MaxAttempt)
                    return randomDirection;
                attempt++;
            }
        }
        public static Vector3 RandomPositionByBounds(BoundingSphere boundingSphere, Func<Vector3, bool> predicate = null)
        {
            int attempt = Const.IntValue.One;
            while (true)
            {
                var randomNormalizedDirection = RandomDirection().normalized;
                var randomPoint = boundingSphere.position + randomNormalizedDirection * boundingSphere.radius;
                if ((predicate?.Invoke(randomPoint) ?? true) || attempt > MaxAttempt)
                    return randomPoint;
                attempt++;
            }
        }
        public static Vector3 RandonPositionByBounds(Bounds boundingBox, Func<Vector3, bool> predicate = null)
        {
            int attempt = Const.IntValue.One;
            while (true)
            {
                var randomNormalizedDirection = new Vector3(
                    RandomOpposite(boundingBox.extents.x), 
                    RandomOpposite(boundingBox.extents.y), 
                    RandomOpposite(boundingBox.extents.z));

                var randomPoint = boundingBox.center + randomNormalizedDirection;
                if ((predicate?.Invoke(randomPoint) ?? true) || attempt > MaxAttempt)
                    return randomPoint;
                attempt++;
            }
        }
    }
}

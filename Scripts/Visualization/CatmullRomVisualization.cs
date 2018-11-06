using System.Runtime.CompilerServices;
using System;
using UnityEngine;
using static SplineTools.CatmullRomSpline;

namespace SplineTools.Visualization {

    [ExecuteInEditMode]
    public class CatmullRomVisualization : PointsContainer {

        [SerializeField, Tooltip("What position are we targeting to compute the angle?")]
        private Transform target;
        [SerializeField]
        private float constraint = 10;
        [SerializeField]
        private bool drawGizmos;

        private Vector3[] binormalBuffer;   // Stores the binormals of the catmull rom spline.
        private Vector3[] pointsBuffer;     // Stores all of the sampled points
        private Vector3 direction;

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            SampleCatmullRomSpline(in points, lineSegments, loop, out pointsBuffer);
            SampleCatmullRomBinormals(in points, lineSegments, loop, out binormalBuffer);
            if (drawGizmos) {
                Gizmos.color = Color.green;
                for (int i = 1; i < pointsBuffer.Length; i++) {
                    Gizmos.DrawLine(pointsBuffer[i], pointsBuffer[i - 1]);
                }

                Gizmos.color = Color.blue;
                for (int i = 0; i < binormalBuffer.Length; i++) {
                    Gizmos.DrawRay(pointsBuffer[i], binormalBuffer[i]);
                    Gizmos.DrawRay(pointsBuffer[i], -binormalBuffer[i]);
                }
            }

            if (target) {
                float minDistance = float.MaxValue, minAngle = float.MaxValue;
                for (int i = 0; i < binormalBuffer.Length; i++) {
                    var binormal = binormalBuffer[i];
                    var direction = pointsBuffer[i] - target.position;
                    var distance = direction.sqrMagnitude;
                    var min = Min(in binormal, in direction);

                    var from = target.position;
                    from.y = direction.y = 0.5f;

                    if (IsClosestEstimatedAngle(in min.Item1, in constraint, in minAngle) &&
                            IsDistanceLessThan(in distance, in minDistance)) {
                        Gizmos.color = Color.green;
                        Gizmos.DrawRay(target.position, min.Item2);
                        this.direction = direction;
                        minDistance = distance;
                    }
                }
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(target.position, direction);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsClosestEstimatedAngle(in float current, in float constraint, in float previous) =>
            current < previous && current < constraint;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsDistanceLessThan(in float current, in float min) => current < min;

        private ValueTuple<float, Vector3> Min(in Vector3 binormal, in Vector3 direction) {
            var angle = Vector3.Angle(binormal, direction);
            float other;
            return ((other = Vector3.Angle(binormal, -direction)) < angle) ? 
                ValueTuple.Create(other, -direction) : ValueTuple.Create(angle, direction);
        }
#endif
    }
}

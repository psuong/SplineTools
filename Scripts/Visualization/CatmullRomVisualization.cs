using System.Runtime.CompilerServices;
using UnityEngine;
using static SplineTools.CatmullRomSpline;

namespace SplineTools.Visualization {

    public class CatmullRomVisualization : PointsContainer {

        [SerializeField, Tooltip("What position are we targetting to compute the angle?")]
        private Transform target;
        [SerializeField]
        private float angleConstraint = 10;

        private Vector3[] binormalBuffer;   // Stores the binormals of the catmull rom spline.
        private Vector3[] pointsBuffer;     // Stores all of the sampled points

#if UNITY_EDITOR
        private void Start() {
            SampleCatmullRomSpline(in points, lineSegments, loop, out pointsBuffer);
            SampleCatmullRomBinormals(in points, lineSegments, loop, out binormalBuffer);
        }

        private void Update() {
            if (target) {
                for (int i = 0; i < binormalBuffer.Length; i++) {
                    var reverseBinormal = binormalBuffer[i];
                    var direction = pointsBuffer[i] - target.position;

                    var from = target.position;
                    from.y = direction.y = 0.5f;
                    if (IsClosestEstimatedAngle(reverseBinormal, direction, in angleConstraint, out var deg)) {
                        Debug.DrawRay(target.position, direction, Color.green);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsClosestEstimatedAngle(Vector3 from, Vector3 to, in float constraint, out float deg) =>
            (deg = Vector3.Angle(from, to)) <= constraint;
#endif
    }
}

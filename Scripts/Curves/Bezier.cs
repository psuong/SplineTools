using UnityEngine;
using System.Collections.Generic;

namespace Curves {

    // TODO: Add looping functionality of Bezier curves
    
    [CreateAssetMenu(menuName = "Curves/Bezier", fileName = "Bezier Curve")]
    public class Bezier : ScriptableObject {

#pragma warning disable 414
        [HideInInspector]
        public Vector3[] points = { Vector3.zero, Vector3.forward * 10f };
        [SerializeField, HideInInspector]
        private Vector3[] controlPoints = { new Vector3(-2.5f, 0f, 2.5f), new Vector3(2.5f, 0f, 7.5f) };
#pragma warning restore 414

        public Vector3[] GetCubicBezierPoints(float factor) {
            var points = new List<Vector3>();
            var size = this.points.Length;

            for (int i = 1; i < size; i++) {
                var start = this.points[i - 1];
                var end = this.points[i];

                var controlStart = controlPoints[i == 1 ? 0 : i];
                var controlEnd = controlPoints[i == 1 ? i : i + (i - 1)];

                var interval = 1f / factor;

                for (float t = 0; t < factor; t += interval) {
                    var point = Bezier.GetCubicBezierCurve(start, controlStart, controlEnd, end, t);
                    points.Add(point);
                }
            }

            return points.ToArray();
        }
        
        /// <summary>
        /// Gets a point along the tangent of the cubic bezier curve.
        /// </summary>
        /// <param name="p0">Start point</param>
        /// <param name="p1">First control point</param>
        /// <param name="p2">Second control point</param>
        /// <param name="p3">End point</param>
        /// <returns>A point along the cubic bezier curve</returns>
        public static Vector3 GetCubicBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            t = Mathf.Clamp01(t);
            var inverseT = 1f - t;

            return (Mathf.Pow(inverseT, 3) * p0) + (3 * Mathf.Pow(inverseT, 2) * t * p1) + (3 * inverseT * Mathf.Pow(t, 2) * p2) + (Mathf.Pow(t, 3) * p3); 
        }

        /// <summary>
        /// Gets a point along the tangent of the quadratic bezier curve.
        /// </summary>
        /// <param name="p0">Start point</param>
        /// <param name="p1">Control point</param>
        /// <param name="p2">End point</param>
        /// <returns>A point along the quadratic bezier curve</returns>
        public static Vector3 GetQuadraticBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            t = Mathf.Clamp01(t);
            var inverseT = 1f - t;

            return inverseT * inverseT * p0 + 2f * inverseT * t * p1 + t * t * p2;
        }
    }
}

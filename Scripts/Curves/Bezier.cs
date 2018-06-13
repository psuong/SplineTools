using CommonStructures;
using UnityEngine;
using System.Collections.Generic;

namespace Curves {

    [CreateAssetMenu(menuName = "Curves/Bezier", fileName = "Bezier Curve")]
    public class Bezier : ScriptableObject {

#pragma warning disable 414
        [HideInInspector]
        public Vector3[] points = { Vector3.zero, Vector3.forward * 10f };
        [HideInInspector]
        public Vector3[] controlPoints = { new Vector3(-2.5f, 0f, 2.5f), new Vector3(2.5f, 0f, 7.5f) };
#pragma warning restore 414
        
        /// <summary>
        /// Calculates a cubic bezier curve given a factor. The factor will determine the parametric value,
        /// t.
        /// </summary>
        /// <param name="segments">How many line segments should be generated in the spline?</param>
        /// <returns>An array of points.</returns>
        public Vector3[] GetCubicBezierPoints(int segments) {
            var bezierPoints = new Vector3[segments + 1];
            var size = points.Length;

            Debug.LogErrorFormat("Size: {0}", size);

            for (int i = 1; i < size; i++) {
                var start = points[i - 1];
                var end = points[i];

                var controlStart = controlPoints[i == 1 ? 0 : i];
                var controlEnd = controlPoints[i == 1 ? i : i + (i - 1)];

                for (int t = 0; t <= segments; t++) {
                    var progress = ((float)t) / ((float)segments);
                    var point = Bezier.GetCubicBezierPoint(start, controlStart, controlEnd, end, progress);
                    bezierPoints[t] = point;
                }
            }
            return bezierPoints;
        }
        
        /// <summary>
        /// Creates a pairs of points which define a bezier curve.
        /// </summary>
        /// <param name="segments">How many segments should be defined within the bezier?</param>
        /// <param name="width">What is the width of the bezier?</param>
        /// <returns>An array of points defining a bezier</returns>
        public Tuple<Vector3, Vector3>[] GetCubicBezierPoints(int segments, float width) {
            var pSize = points.Length;
            var size = (segments + 1) * (pSize - 1);
            var cubicPoints = new Tuple<Vector3, Vector3>[size];
            var index = 0;

            for (int i = 1; i < pSize; i++) {
                var start = points[i - 1];
                var end = points[i];

                var cIndex = i * 2;

                var controlStart = controlPoints[cIndex - 2];
                var controlEnd = controlPoints[cIndex - 1];

                for (int t = 0; t <= segments; t++) {
                    var progress = ((float)t) / ((float)segments);

                    var lhs = Bezier.GetCubicBezierPoint(start, controlStart, controlEnd, end, progress);
                    var tangent = Bezier.GetTangent(start, controlStart, controlEnd, end, progress);
                    var binormal = Bezier.GetBinormal(tangent.normalized, Vector3.up);

                    var rhs = lhs + (binormal * width);

                    var pair = Tuple<Vector3, Vector3>.CreateTuple(rhs, lhs);
                    cubicPoints[index] = pair;
                    index++;
                }
            }
            return cubicPoints;
        }
        
        /// <summary>
        /// Returns the binormal vector.
        /// </summary>
        /// <param name="tangent">The tangent of a point</param>
        /// <param name="normal">The normal of the tangent.</param>
        /// <returns>The cross product between a tangent and a normal.</returns>
        public static Vector3 GetBinormal(Vector3 tangent, Vector3 normal) {
            return Vector3.Cross(tangent, normal);
        }

        /// <summary>
        /// Gets a point along the tangent of the cubic bezier curve.
        /// </summary>
        /// <param name="p0">Start point</param>
        /// <param name="p1">First control point</param>
        /// <param name="p2">Second control point</param>
        /// <param name="p3">End point</param>
        /// <param name="t">The parametric value, t.</param>
        /// <returns>A point along the cubic bezier curve</returns>
        public static Vector3 GetCubicBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
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

        /// <summary>
        /// Gets the tangent (velocity) at a point.
        /// </summary>
        /// <param name="p0">The first point on the bezier curve</param>
        /// <param name="p1">The first control point on the bezier curve</param>
        /// <param name="p2">The second control point on the bezier curve</param>
        /// <param name="p3">The second point on the bezier curve</param>
        /// <param name="t">Parametric value, t, which defines the percentage on the curve</param>
        /// <returns>The first derivative at a said point.</returns>
        public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            t = Mathf.Clamp01(t);
            var inverseT = 1f - t;
            
            return (3f * Mathf.Pow(inverseT, 2)  * p1 - p0) + (6f * inverseT * t * (p2 - p1)) + (3 * Mathf.Pow(t, 2) * (p3 - p2));
        }
        
        /// <summary>
        /// Returns an array of of normalised tangents for each parametric value t within the linesteps.
        /// </summary>
        /// <param name="lineStep">How many line segments should there be?</param>
        /// <param name="points">An array of points that define the array.</param>
        /// <param name="cPoints">An array of control points which define the bezier.</param>
        /// <returns>Returns an array of tuples defining the direction and its points of origin.</returns>
        public static Tuple<Vector3, Vector3>[] GetTangentsNormalised(int lineStep, Vector3[] points, Vector3[] cPoints) {
            var directions = new List<Tuple<Vector3, Vector3>>();

            for (int i = 1; i < points.Length; i++) {
                var start = points[i - 1];
                var end = points[i];

                var index = i * 2;

                var cStart = cPoints[index - 2];
                var cEnd = cPoints[index - 1];
                for (int t = 0; t <= lineStep; t++) {
                    float progress = ((float)t) / ((float)lineStep);

                    var point = Bezier.GetCubicBezierPoint(start, cStart, cEnd, end, progress);
                    var tangent = Bezier.GetTangent(start, cStart, cEnd, end, progress).normalized;

                    directions.Add(Tuple<Vector3, Vector3>.CreateTuple(point, tangent));
                }
            }
            return directions.ToArray();
        }
    }
}

using CommonStructures;
using UnityEngine;
using System.Collections.Generic;

namespace Curves {

    [CreateAssetMenu(menuName = "Curves/Bezier", fileName = "Bezier Curve")]
    public class Bezier : ScriptableObject {
        
        /// <summary>
        /// Returns the number of splines within the bezier.
        /// </summary>
        public int SplineCount {
            get {
                return (points.Length - 1) / 3;
            }
        }

#pragma warning disable 414
        [HideInInspector]
        public Vector3[] points = { Vector3.zero, new Vector3(-2.5f, 0f, 2.5f), new Vector3(2.5f, 0f, 7.5f), Vector3.forward * 10f };
#pragma warning restore 414

        /// <summary>
        /// Samples all of the points needed in a bezier curve.
        /// </summary>
        /// <param name="segments">The total number of line segments along a path.</param>
        /// <returns>An array of points along a bezier curve.</returns>
        public Vector3[] SampleCubicBezierCurve(int segments) {
            var bezierPoints = new List<Vector3>();
            var size = points.Length;

            // Immediately add the first point.
            bezierPoints.Add(points[0]);

            for (int i = 0; i < size - 1; i+=3) {
                var p0 = points[i];
                var c0 = points[i + 1];
                var c1 = points[i + 2];
                var p1 = points[i + 3];

                for (int j = 0; j <= segments; j++) {
                    var t = ((float) j) / segments;
                    var pt = Bezier.GetCubicBezierPoint(p0, c0, c1, p1, t);
                    var bSize = bezierPoints.Count;

                    var index = Mathf.Clamp(bSize - 1, 0, bSize);
                    if (bezierPoints[index] != pt) {
                        bezierPoints.Add(pt);
                    }
                }
            }
            return bezierPoints.ToArray();
        }
        
        /// <summary>
        /// Samples a series of pair of points along a bezier curve.
        /// </summary>
        /// <param name="segments">How many segments are generated in a bezier curve?</param>
        /// <param name="width">How far apart are the pair of points?</param>
        /// <returns>An array of tuples containing the points along a bezier curve.</returns>
        public Tuple<Vector3, Vector3>[] SampleCubicBezierCurve(int segments, float width) {
            var bezierPoints = new List<Tuple<Vector3, Vector3>>();
            var size = points.Length;

            for (int i = 0; i < size - 1; i += 3) {
                var p0 = points[i];
                var c0 = points[i + 1];
                var c1 = points[i + 2];
                var p1 = points[i + 3];

                for (int t = 0; t <= segments; t++) {
                    var progress = ((float) t) / segments;

                    var rhs = Bezier.GetCubicBezierPoint(p0, c0, c1, p1, progress);
                    var tangent = Bezier.GetTangent(p0, c0, c1, p1, progress);
                    var binormal = Bezier.GetBinormal(tangent.normalized, Vector3.up);

                    var lhs = rhs + (binormal * width);
                    var tuple = Tuple<Vector3, Vector3>.CreateTuple(lhs, rhs);
                    var bSize = bezierPoints.Count;

                    var index = Mathf.Clamp(bSize - 1, 0, bSize);

                    if (index == 0) {
                        bezierPoints.Add(Tuple<Vector3, Vector3>.CreateTuple(lhs, rhs));
                    } else if (index > 0 && bezierPoints[index].item1 != tuple.item1 && bezierPoints[index].item2 != tuple.item2) {
                        bezierPoints.Add(tuple);
                    }
                }
            }
            return bezierPoints.ToArray();
        }
#region Static Functions
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
        /// Accumulates the distance of the entire bezier curve(s).
        /// </summary>
        /// <param name="points">The points of the bezier curve.</param>
        /// <returns>The total distance of the bezier curve.</returns>
        public static float GetCubicBezierDistance(Vector3[] points) {
            var sum = 0f;
            for (int i = 1; i < points.Length; i++) {
                var start = points[i - 1];
                var end = points[i];
                sum += Vector3.Distance(end, start);
            }
            return sum;
        }

        /// <summary>
        /// Creates a lookup table of the distances.
        /// </summary>
        /// <param name="vertices">An array of vertices.</param>
        /// <returns>An array of accumulating distances.</returns>
        public static float[] GetCubicLengthTable(Vector3[] vertices) {
            var distances = new float[vertices.Length];
            distances[0] = 0f;
            var total = 0f;
            for (int i = 1; i < vertices.Length; i++) {
                var start = vertices[i - 1];
                var end = vertices[i];

                var distance = (end - start).magnitude;
                total += distance;
                distances[i] = total;
            }
            return distances;
        }

        /// <summary>
        /// Generates a look up table for each spline.
        /// </summary>
        public static float[][] GetCubicLengthTable(Vector3[] points, Vector3[] controlPoints, int segments) {
            var splineCount = points.Length - 1;

            var distances = new float[splineCount][];
            var previous = points[0];

            for (int i = 1, splineIndex = 0; i < points.Length; i++, splineIndex++) {
                var index = i * 2;
                var lhs = points[i - 1];
                var rhs = points[i];

                var cStart = controlPoints[index - 2];
                var cEnd = controlPoints[index - 1];

                distances[splineIndex] = new float[segments];

                for (int j = 0; j <= segments; j++) {
                    var t = (float) j / segments;
                    var point = Bezier.GetCubicBezierPoint(lhs, cStart, cEnd, rhs, t);
                    var magnitude = (point - previous).magnitude;

                    distances[splineIndex][j] = magnitude;
                }
            }

            return distances;
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
        /// <param name="segments">How many line segments should there be?</param>
        /// <param name="points">An array of points that define the bezier.</param>
        /// <returns>Returns an array of tuples defining the direction and its points of origin.</returns>
        public static Tuple<Vector3, Vector3>[] GetTangentsNormalised(Vector3[] points, int segments) {
            var directions = new List<Tuple<Vector3, Vector3>>();
            var size = points.Length;

            for (int i = 0; i < size - 1; i += 3) {
                var p0 = points[i];
                var c0 = points[i + 1];
                var c1 = points[i + 2];
                var p1 = points[i + 3];

                for (int t = 0; t <= segments; t++) {
                    var progress = (float) t  / segments;

                    var pt = Bezier.GetCubicBezierPoint(p0, c0, c1, p1, progress);
                    var tangent = Bezier.GetTangent(p0, c0, c1, p1, progress).normalized;
                    directions.Add(Tuple<Vector3, Vector3>.CreateTuple(pt, tangent));
                }
            }

            return directions.ToArray();
        }
    }
#endregion
}

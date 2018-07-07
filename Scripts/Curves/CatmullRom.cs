using CommonStructures;
using Curves.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Curves {

    [CreateAssetMenu(menuName = "Curves/CatmullRom", fileName = "CatmullRom Curve")]
    public class CatmullRom : ScriptableObject, ISpline {
        
        /// <summary>
        /// The number of splines between all of the points.
        /// </summary>
        public int SplineCount {
            get {
                var size = points.Length;
                return isLooping ? points.Length : (size % 2 == 0) ? size / 2 - 1 : size / 2 + 1;
            }
        }

        [HideInInspector]
        public Vector3[] points = { Vector3.back, Vector3.left, Vector3.forward, Vector3.right };
        [HideInInspector]
        public bool isLooping;

        /// <summary>
        /// Samples and generates a Catmull Rom Spline between all of the points if looping is toggled. Otherwise,
        /// the Catmull Rom Spline is generated between all of the points except the first and last point in the array.
        /// </summary>
        /// <param name="segments">How many segments are there within each line segment?</param>
        public Vector3[] SampleCatmullRomSpline(int segments) {
            var pts = new List<Vector3>();

            for (int i = 0; i < points.Length; i++) {
                if ((i == 0 || i == points.Length - 1 || i == points.Length - 2) && !isLooping) {
                    continue;
                }

                var p0 = points[ClampIndex(i - 1)];
                var p1 = points[ClampIndex(i)];
                var p2 = points[ClampIndex(i + 1)];
                var p3 = points[ClampIndex(i + 2)];

                for (int t = 0; t <= segments; t++) {
                    var progress = (float) t / segments;
                    var pt = CatmullRom.GetCatmullRomSplinePoint(p0, p1, p2, p3, progress);
                    if (pts.Count == 0) {
                        pts.Add(pt);
                    } else if (pts[pts.Count - 1] != pt) {
                        pts.Add(pt);
                    }
                }
            }

            return pts.ToArray();
        }

        /// <summary>
        /// Returns an array of normalised tangents for each parametric value t within the Catmull Rom Spline.
        /// </summary>
        /// <param name="segments">How many line segments are there within each spline.</param>
        /// <returns>An array of tuples defining the direction relative to the original point.</returns>
        public Tuple<Vector3, Vector3>[] SampleCatmullRomSpline(int segments, float width) {
            var directions = new List<Tuple<Vector3, Vector3>>();

            for (int i = 0; i < points.Length; i++) {
                if ((i == 0 || i == points.Length - 1 || i == points.Length - 2) && !isLooping) {
                    continue;
                }

                var p0 = points[ClampIndex(i - 1)];
                var p1 = points[ClampIndex(i)];
                var p2 = points[ClampIndex(i + 1)];
                var p3 = points[ClampIndex(i + 2)];

                for (int t = 0; t <= segments; t++) {
                    var progress = (float) t / segments;
                    var rhs = CatmullRom.GetCatmullRomSplinePoint(p0, p1, p2, p3, progress);
                    var tangent = CatmullRom.GetTangent(p0, p1, p2, p3, progress);
                    var lhs = rhs + (tangent.Binormal(Vector3.up) * width);

                    if (directions.Count == 0) {
                        directions.Add(Tuple<Vector3, Vector3>.CreateTuple(lhs, rhs));                   
                    } else if (directions.Count > 0 && directions[directions.Count - 1].item2 != rhs) {
                        directions.Add(Tuple<Vector3, Vector3>.CreateTuple(lhs, rhs));
                    }
                }
            }
            return directions.ToArray();
        }

        /// <summary>
        /// Returns the accumulating distance of each segment within the first spline.
        /// </summary>
        /// <param name="segments">How many segments are there in the spline?</param>
        /// <returns>An array of distances from the original point for each point within the spline.</returns>
        public float[] GetLengthTable(int segments) {
            var vertices = SampleCatmullRomSpline(segments);

            var distances = new float[SplineCount * (segments + 1) - (SplineCount - 1)];

            var previous = vertices[0];
            var total = 0f;

            for (int i = 1; i < distances.Length; i++) {
                distances[i] = total += (vertices[i] - previous).magnitude;
                previous = vertices[i];
            }

            return distances;
        }

        private int ClampIndex(int i) {
            if (i < 0) {
                return points.Length - 1;
            } else if (i > points.Length) {
                return 1;
            } else if (i > points.Length - 1) {
                return 0;
            } else {
                return i;
            }
        }

#region Static Functions
        /// <summary>
        /// Returns a point on the catmull rom spline between p1 and p2.
        /// </summary>
        /// <param name="p0">The first control point.</param>
        /// <param name="p1">The first point of the actual spline.</param>
        /// <param name="p2">The end point of the actual spline.</param>
        /// <param name="p3">The second control point.</param>
        /// <param name="t">The parametric value between a curve, typically between 0 and 1.</param>
        /// <returns>A point between p1 and p2.</returns>
        public static Vector3 GetCatmullRomSplinePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            t = Mathf.Clamp01(t);
            var a = 2f * p1;
            var b = p2 - p0;
            var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            var d = -p0 + 3f * p1 - 3f * p2 + p3;

            // 0.5f ((2f * p1) + ((p2 - p0) * t) + ((2f * p0 - 5f * p1 + 4f * p2 - p3) * t^2) * ((-p0 + 3f * p1 - 3f * p2 + p3) * t^3)

            return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
        }

        /// <summary>
        /// Gets the tangent (velocity) at a point in the Catmull Rom Spline.
        /// </summary>
        /// <param name="p0">The first control point.</param>
        /// <param name="p1">The first point of the actual spline.</param>
        /// <param name="p2">The end point of the actual spline.</param>
        /// <param name="p3">The second control point.</param>
        /// <param name="t">Parametric value, t, which defines the position of the point on the curve.</param>
        /// <returns>The first derivative along the point of the curve normalised.</returns>
        public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            var a = p2 - p0;
            var b = (2 * t) * (2 * p0 - 5f * p1 + 4f * p2 - p3);
            var c = (3 * t * t) * (-p0 + 3f * p1 - 3f * p2 + p3);

            return (0.5f * (a + b + c)).normalized;
        }
#endregion
    }
}

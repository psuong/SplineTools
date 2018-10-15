using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SplineTools {

    public static class CatmullRomSpline {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampIndex(int index, int sampleSize) {
            if (index < 0) 
                return sampleSize - 1;
            else if (index > sampleSize)
                return 1;
            else if (index > sampleSize - 1)
                return 0;
            else
                return index;
        }

        /// <summary>
        /// Returns the derivative of the CatmullRomSpline at a given point.
        /// </summary>
        /// <param name="p0">The first point on the CatmullRomSpline.</param>
        /// <param name="p1">The second point on the CatmullRomSpline.</param>
        /// <param name="p2">The third point on the CatmullRomSpline.</param>
        /// <param name="p3">The fourth point on the CatmullRomSpline.</param>
        /// <param name="normalize">Should the tangent be normalised?</param>
        public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, bool normalize = false) {
            var a = p2 - p0;
            var b = (2 * t) * (2 * p0 - 5f * p1 + 4f * p2 - p3);
            var c = (3 * t * t) * (-p0 + 3f * p1 - 3f * p2 + p3);

            var tangent = 0.5f * (a + b + c);
            return normalize ? tangent.normalized : tangent;
        }

        /// <summary>
        /// Returns the position between four Vector3 positions.
        /// </summary>
        /// <param name="p0">The first ctrl point on the CatmullRomSpline.</param>
        /// <param name="p1">The actual first point on the CatmullRomSpline.</param>
        /// <param name="p2">The end point on the CatmullRomSpline.</param>
        /// <param name="p3">The second ctrl point on the CatmullRomSpline.</param>
        /// <param name="t">Parametric value t between 0, 1.</param>
        public static Vector3 GetCatmullRomPosition(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            var a = 2f * p1;
            var b = p2 - p0;
            var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            var d = -p0 + 3f * p1 - 3f * p2 + p3;

            return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
        }

        /// <summary>
        /// Samples points along the Catmull Rom Spline into a buffer.
        /// </summary>
        /// <param name="samples">The control points to sample.</param>
        /// <param name="segments">The number of segments between each spline.</param>
        /// <param name="loop">Should the Catmull Rom Spline loop?</param>
        /// <param name="points">The buffer to write to.</param>
        public static void SampleCatmullRomSpline(ref Vector3[] samples, int segments, bool loop, out Vector3[] points) {
            var size = samples.Length;
            points = new Vector3[size * segments];
            for (int i = 0; i < size; i++) {
                if ((i == 0 || i == (size - 1) || i == (size - 2)) && !loop)
                    continue;

                var p0 = samples[ClampIndex(i - 1, size)];
                var p1 = samples[i];
                var p2 = samples[ClampIndex(i + 1, size)];
                var p3 = samples[ClampIndex(i + 2, size)];

                var index = i * segments;
                points[index] = p1;
                for (int k = 1; k < segments; k++) {
                    var t = ((float)k) / segments;
                    var point = GetCatmullRomPosition(p0, p1, p2, p3, t);
                    points[index + k] = point;
                }
            }
        }

        /// <summary>
        /// Samples points along the Catmull Rom Spline.
        /// </summary>
        /// <param name="samples">The control points to sample.</param>
        /// <param name="segments">The segments between each spline.</param>
        /// <param name="points">The genereated output of the Catmull Rom Spline.</param>
        /// <param name="loop">Should the catmull Rom Spline loop?</param>
        public static void SampleCatmullRomSpline(ref Vector3[] samples, int segments, bool loop, out IList<Vector3> points) {
            var size = samples.Length;
            points = new List<Vector3>();
            for (int i = 0; i < size; i++) {
                if ((i == 0 || i == (size - 1) || i == (size - 2)) && !loop)
                    continue;

                var p0 = samples[ClampIndex(i - 1, size)];
                var p1 = samples[i];
                var p2 = samples[ClampIndex(i + 1, size)];
                var p3 = samples[ClampIndex(i + 2, size)];

                points.Add(p1);
                for (int k = 1; k < segments; k++) {
                    var t = ((float)k) / segments;
                    var point = GetCatmullRomPosition(p0, p1, p2, p3, t);
                    points.Add(point);
                }
            }
        }
    }
}

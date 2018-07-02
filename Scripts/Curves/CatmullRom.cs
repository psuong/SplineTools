using System.Collections.Generic;
using UnityEngine;

namespace Curves {
    
    [CreateAssetMenu(menuName = "Curves/CatmullRom", fileName = "CatmullRom Curve")]
    public class CatmullRom : ScriptableObject {
        
        [HideInInspector]
        public Vector3[] points = { Vector3.zero, Vector3.left, Vector3.forward, Vector3.right };
        [HideInInspector]
        public bool isLooping;

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
                    pts.Add(pt);
                }
            }

            return pts.ToArray();
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

            return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
        }
#endregion
    }
}

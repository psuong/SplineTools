using UnityEngine;

namespace Curves.Utility {

    public static class MathUtility {
        
        /// <summary>
        /// Returns a rotated point on the XZ plane.
        /// </summary>
        /// <param name="pivot">The point of reference.</param>
        /// <param name="radius">The distance away from the point.</param>
        /// <param name="angle">The angle of the point in degrees.</param>
        /// <returns>The point with a variable angle and distance away from the pivot.</returns>
        public static Vector3 GetCircleXZPoint(Vector3 pivot, float radius, float angle) {
            var v = Vector3.zero;

            var radians = angle * Mathf.Deg2Rad;
            v.x = radius * Mathf.Cos(radians);
            v.z = radius * Mathf.Sin(radians);

            return v + pivot;
        }
    }

    public static class FloatExtension {

        /// <summary>
        /// Samples an array and returns the value closest to the parametric value t.
        /// </summary>
        /// <param name="data">The set of values to search.</param>
        /// <param name="t">A parametric value between 0 and 1.</param>
        /// <returns>The value closest to t.</returns>
        public static float Sample(this float[] data, float t) {
            t = Mathf.Clamp01(t);
            var size = data.Length;

            if (size == 0) {
                return 0f;
            } else if (size == 1) {
                return data[1];
            }

            var floatI = t * (size - 1);
            var lhs = Mathf.FloorToInt(floatI);
            var rhs = Mathf.FloorToInt(floatI + 1);

            if (rhs >= size) {
                return data[size - 1];
            } else if (lhs <= 0) {
                return data[0];
            } else {
                return Mathf.Lerp(data[lhs], data[rhs], floatI - lhs);
            }
        }
        
        /// <summary>
        /// Samples a matrix of a size t given a row.
        /// </summary>
        /// <param name="data">A matrix of data given</param>
        public static float Sample(this float[][] data, int index, float t) {
            var size = data.Length;
            if (size == 0) {
                return 0f;
            } else {
                index = Mathf.Clamp(index, 0, size - 1);
                t = Mathf.Clamp01(t);
                
                return data[index].Sample(t);
            }
        }
    }
}

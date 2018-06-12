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
}

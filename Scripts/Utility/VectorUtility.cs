using System.Runtime.CompilerServices;
using UnityEngine;

namespace SplineTools.Utilities {

    public static class VectorUtility {

        /// <summary>
        /// Returns the binormal of the tangent along a curve.
        /// </summary>
        /// <param name="tangent">The line tangent to the curve.</param>
        /// <param name="normal">The projected normal of the tangent.</param>
        /// <returns>The perpendicular point to the tangent.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Binormal(this Vector3 tangent, Vector3 normal) {
            return Vector3.Cross(tangent, normal);
        }
    }
}

using UnityEngine;

namespace SplineTools.Utilities {

    public static class VectorUtility {

        public static Vector3 Binormal(this Vector3 tangent, Vector3 normal) {
            return Vector3.Cross(tangent, normal);
        }
    }
}

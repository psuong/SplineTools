using UnityEngine;

namespace Math {

    public static class Vector3Extensions {
        public static Vector3 Round(this Vector3 value, float factor = 100f) {
            return new Vector3(
                   (Mathf.Round(value.x) * factor) / factor,
                   (Mathf.Round(value.y) * factor) / factor,
                   (Mathf.Round(value.x) * factor) / factor);
        }
    }
}

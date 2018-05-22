using UnityEngine;

namespace Curves {
    
    public class Bezier : MonoBehaviour {
        
        [SerializeField]
        private Vector3[] points = new Vector3[] { Vector3.zero, Vector3.forward * 10f };
        [SerializeField]
        private Vector3[] controlPoints;
        
        /// <summary>
        /// A bezier is defined by (1 - t)^2 * p0 + 2(1 - t) * t * p1 + t^2 * p2.
        /// </summary>
        /// <param name="p0">First point</param>
        /// <param name="p1">Second point</param>
        /// <param name="p2">Third point</param>
        /// <param name="t">Parametric value between o and 1</param>
        /// <returns>The value alongside the bezier curve</returns>
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            t = Mathf.Clamp01(t);
            var inverseT = 1f - t;

            return inverseT * inverseT * p0 + 2f * inverseT * t * p1 + t * t * p2;
        }
    }
}

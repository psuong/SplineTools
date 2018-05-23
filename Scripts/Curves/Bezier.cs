using System.Collections.Generic;
using UnityEngine;

namespace Curves {

    public enum BezierMode {
        Quadratic,
        Cubic
    }
    
    public class Bezier : MonoBehaviour {
        
        [SerializeField]
        private Vector3[] points = new Vector3[] { Vector3.zero, Vector3.forward * 10f };
        [SerializeField]
        private Vector3[] controlPoints;

        private float progress;
        [SerializeField]
        private IList<Vector3> bezierPoints = new List<Vector3>();

        private void Awake() {
            progress = 0f;
        }

        private void OnDrawGizmos() {
            for (int i = 1; i < bezierPoints.Count; i++) {
                var previous = transform.TransformPoint(bezierPoints[i - 1]);
                var current = transform.TransformPoint(bezierPoints[i]);
                Gizmos.DrawLine(previous, current);
            }
        }

        public void ClearPoints() {
            bezierPoints.Clear();
        }

        public void PopulateCubicPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            bezierPoints.Add(GetCubicBezier(p0, p1, p2, p3, t));
        }
        
        /// <summary>
        /// A bezier is defined by (1 - t)^2 * p0 + 2(1 - t) * t * p1 + t^2 * p2.
        /// </summary>
        /// <param name="p0">First point</param>
        /// <param name="p1">Second point</param>
        /// <param name="p2">Third point</param>
        /// <param name="t">Parametric value between o and 1</param>
        /// <returns>The value alongside the bezier curve</returns>
        public static Vector3 GetQuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            t = Mathf.Clamp01(t);
            var inverseT = 1f - t;

            return inverseT * inverseT * p0 + 2f * inverseT * t * p1 + t * t * p2;
        }

        public static Vector3 GetCubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            t = Mathf.Clamp01(t);
            var inverseT = 1f - t;

            return (Mathf.Pow(inverseT, 3) * p0) + (3 * Mathf.Pow(inverseT, 2) * t * p1) + (3 * inverseT * Mathf.Pow(t, 2) * p2) + (Mathf.Pow(t, 3) * p3); 
        }
    }
}

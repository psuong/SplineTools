using static SplineTools.CatmullRomSpline;
using System.Collections.Generic;
using UnityEngine;

namespace SplineTools.Visualization {

    public class CatmullRomVisualization : PointsContainer {

        [SerializeField]
        private Color gizmos = Color.magenta;
        [SerializeField, Range(0.1f, 1f)]
        private float radius = 0.1f;

        private IList<Vector3> generated;

        private void OnDrawGizmosSelected() {
            Gizmos.color = gizmos;
            foreach (var point in points) {
                Gizmos.DrawWireSphere(point, radius);
            }
            SampleCatmullRomSpline(ref points, 10, loop, out generated);

            for (int i = 1; i < generated.Count; i++) {
                Gizmos.DrawLine(generated[i - 1], generated[i]);
            }
        }
    }
}

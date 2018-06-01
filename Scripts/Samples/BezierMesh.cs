using UnityEngine;

namespace Curves {

    public class BezierMesh : MonoBehaviour {
        
        [SerializeField]
        private Bezier bezier;
        [SerializeField]
        private float t = 100f;
        [SerializeField]
        private Vector3[] points;

        private void Start() {
        }
        
        public void GeneratePoints() {
            for (float i = 0; i < 1; i += 1 / t) {
            }
        }
    }
}

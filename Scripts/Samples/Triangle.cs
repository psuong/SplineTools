using UnityEngine;

namespace Curves {

    /**
     * This will just create a single triangle.
     * Normal: Vector3.up
     * UVs: Depends on the coordinates of a unit square.
     * Points: Any value
     */
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Triangle : MonoBehaviour {

        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uvs;
        public int[] triangleIndices;

        private MeshFilter meshFilter;
        private MeshGenerator meshGenerator;

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            var radius = 0.2f;
            try {
                foreach (var point in vertices) {
                    Gizmos.DrawWireSphere(point, radius);
                }
            } catch(System.Exception) {}
        }

        public void GenerateMesh() {
            meshFilter = GetComponent<MeshFilter>();
            meshGenerator = new MeshGenerator();

            meshGenerator.AddVertices(vertices);
            meshGenerator.AddNormals(normals);
            meshGenerator.AddUVs(uvs);
            meshGenerator.AddTriangle(triangleIndices);

            meshFilter.sharedMesh = meshGenerator.CreateMesh();
        }
    }
}

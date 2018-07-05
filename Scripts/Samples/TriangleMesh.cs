using UnityEngine;

namespace Curves {

    /**
     * This will just create a single triangle.
     * Normal: Vector3.up
     * UVs: Depends on the coordinates of a unit square.
     * Points: Any value
     */
    public class TriangleMesh : BaseMesh {

#pragma warning disable 414
        [SerializeField]
        private Vector3[] vertices;
        [SerializeField]
        private Vector3[] normals;
        [SerializeField]
        private Vector2[] uvs;
        [SerializeField]
        private int[] triangleIndices;
#pragma warning restore 414

        protected override void GenerateTriangles() {
            triangleIndices = new int[] { 0, 1, 2 };
        }

        public override void GenerateMesh() {
            meshFilter = GetComponent<MeshFilter>();
            meshGenerator = new MeshGenerator();

            meshGenerator.AddVertices(vertices);
            meshGenerator.AddNormals(normals);
            meshGenerator.AddUVs(uvs);
            meshGenerator.AddTriangles(triangleIndices);

            meshFilter.sharedMesh = meshGenerator.CreateMesh();
        }
    }
}

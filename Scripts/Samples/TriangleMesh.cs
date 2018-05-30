using UnityEngine;

namespace Curves {

    /**
     * This will just create a single triangle.
     * Normal: Vector3.up
     * UVs: Depends on the coordinates of a unit square.
     * Points: Any value
     */
    public class TriangleMesh : BaseMesh {

        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uvs;
        public int[] triangleIndices;

        public override void GenerateMesh() {
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

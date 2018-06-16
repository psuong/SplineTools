using UnityEngine;

namespace Curves {

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class QuadMesh : BaseMesh {

        // Bottom left, Bottom right, Top Left, Top Right
        [SerializeField, HideInInspector]
        private Vector3[] points = { new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 0, 1) };
        [SerializeField, Tooltip("How refine should the mesh be? The higher the resolution the more vertices the mesh has.")]
        private int resolution = 1;

        private Vector3[] vertices;
        private int[] triangles;

        public override void GenerateMesh() {
            vertices = new Vector3[(resolution + 1) * (resolution + 1)];
            meshFilter = GetComponent<MeshFilter>();
            meshGenerator = meshGenerator?? new MeshGenerator();

            meshGenerator.Clear();

            // The amount to lerp and find the new Vector
            var tInterval = 1f / resolution;

            // The progress of t vertically and horizontally
            float tVertical = 0, tHorizontal = 0;

            for (int y = 0, i = 0; y <= resolution; y++) {
                var startLeft = Vector3.Lerp(points[2], points[0], tVertical);
                var endLeft = Vector3.Lerp(points[3], points[1], tVertical);
                for (int x = 0; x <= resolution; x++, i++) { 
                    var current = Vector3.Lerp(startLeft, endLeft, tHorizontal);
                    tHorizontal += tInterval;
                    vertices[i] = current;
                } 
                tVertical += tInterval;
                tHorizontal = 0f;
            }

            GenerateTriangles();
            var uvs = MeshGenerator.GenerateUvs(vertices.Length, resolution, resolution);

            meshGenerator.AddVertices(vertices);
            meshGenerator.AddTriangle(triangles);
            meshGenerator.AddUVs(uvs);

            var mesh = meshGenerator.CreateMesh();
            mesh.name = name;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            meshFilter.sharedMesh = mesh;
        }

        private void GenerateTriangles() {
            triangles = new int[resolution * resolution * 6];
            for (int ti = 0, vi = 0, y = 0; y < resolution; y++, vi++) {
                for (int x = 0; x < resolution; x++, ti += 6, vi++) {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + resolution + 1;
                    triangles[ti + 5] = vi + resolution + 2; 
                }
            }
        }
    }
}

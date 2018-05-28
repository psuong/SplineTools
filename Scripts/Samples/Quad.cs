using System.Collections.Generic;
using UnityEngine;

namespace Curves {

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Quad : BaseMesh {

        // Top left, Top right, Bottom Left, Bottom Right
        public Vector3 topLeft, topRight, bottomLeft, bottomRight;
        public int resolution = 1;

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
                var startLeft = Vector3.Lerp(bottomLeft, topLeft, tVertical);
                var endLeft = Vector3.Lerp(bottomRight, topRight, tVertical);
                for (int x = 0; x <= resolution; x++, i++) {
                    var current = Vector3.Lerp(startLeft, endLeft, tHorizontal);
                    tHorizontal += tInterval;
                    vertices[i] = current;
                } 
                tVertical += tInterval;
                tHorizontal = 0f;
            }

            GenerateTriangles();

            meshGenerator.AddVertices(vertices);
            meshGenerator.AddTriangle(triangles);

            meshFilter.sharedMesh = meshGenerator.CreateMesh();
            meshFilter.sharedMesh.RecalculateNormals();
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

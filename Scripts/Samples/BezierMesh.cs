using CommonStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Curves {

    public class BezierMesh : BaseMesh {

        [SerializeField]
        private Bezier bezier;
        [SerializeField, Tooltip("How wide are the curves away from each other?")]
        private float width = 1f;
        [SerializeField]
        private int resolution = 1;
        [SerializeField, Range(5, 100), Tooltip("How many line segments define the bezier curve?")]
        private int segments = 10;

        // Store the vertices for the mesh.
        private Tuple<Vector3, Vector3>[] vertices;
        private int[] triangles;

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.green;

            GeneratePoints();
            
            for (int i = 1; i < vertices.Length; i++) {
                var start = vertices[i - 1];
                var end = vertices[i];

                Gizmos.DrawLine(start.item1, end.item1);
                Gizmos.DrawLine(start.item2, end.item2);
            }
        }
#endif
        private void GeneratePoints() {
            vertices = bezier.GetCubicBezierPoints(segments, width);
        }

        private void GenerateTriangles() {
            triangles = new int[vertices.Length * resolution * 6];
            for (int ti = 0, vi = 0, y = 0; y < vertices.Length; y++, vi++) {
                for (int x = 0; x < resolution; x++, ti += 6, vi++) {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + resolution + 1;
                    triangles[ti + 5] = vi + resolution + 2; 
                }
            }
        }

        public override void GenerateMesh() {
            GeneratePoints();
            GenerateTriangles();

            meshGenerator = meshGenerator?? new MeshGenerator();
            meshGenerator.Clear();

            var points = new List<Vector3>();

            var size = vertices.Length;

            var tInterval = 1f / resolution;

            var tHorizontal = 0f;

            for (int y = 0; y < size; y++) {
                var vertex = vertices[0];
                var bottomLeft = vertex.item1;
                var bottomRight = vertex.item2;
                for (var x = 0; x < resolution; x++) {
                    var current = Vector3.Lerp(bottomLeft, bottomRight, tHorizontal);
                    tHorizontal += tInterval;
                    points.Add(current);
                }
            }
            meshGenerator.AddVertices(points.ToArray());
            meshGenerator.AddTriangle(triangles);

            var mesh = meshGenerator.CreateMesh();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }
    }
}

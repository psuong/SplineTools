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
        
        /*
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
        */

        /**
         * Testing the mesh generation of a single row in a triangle.
         */
        private void GenerateTriangles(IList<Vector3> mVertices) {
            triangles = new int[segments * resolution * 6];
            for (int ti = 0, vi = 0, y = 0; y < segments; y++, vi++) {
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

            meshFilter = meshFilter?? GetComponent<MeshFilter>();
            meshGenerator = meshGenerator?? new MeshGenerator();
            meshGenerator.Clear();

            var mVertices = new List<Vector3>();

            foreach (var tuple in vertices) {
                for (int t = 0; t <= resolution; t++) {
                    var progress = ((float) t) / ((float) resolution);
                    var pt = Vector3.Lerp(tuple.item1, tuple.item2, progress);
                    mVertices.Add(pt);
                }
            }

            GenerateTriangles(mVertices);

            var mesh = meshGenerator.CreateMesh();
            mesh.SetVertices(mVertices);
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            
            meshFilter.mesh = mesh;
        }
    }
}

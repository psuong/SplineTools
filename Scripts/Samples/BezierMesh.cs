using CommonStructures;
using UnityEngine;

namespace Curves {

    public class BezierMesh : BaseMesh {
        
        [Tooltip("What bezier profile should we use?")]
        public Bezier bezier;
        [Tooltip("How wide are the curves away from each other?")]
        public float width = 1f;
        [Tooltip("What is the resolution along the x axis?")]
        public int resolution = 1;
        [Range(5, 100), Tooltip("How many line segments define the bezier curve?")]
        public int segments = 10;

        // Store the vertices for the mesh.
        private Tuple<Vector3, Vector3>[] vertices;
        private int[] triangles;
        
        private void GenerateTriangles(int size) {
            triangles = new int[segments * resolution * 6];
            for (int ti = 0, vi = 0, y = 0; y < segments - 1; y++, vi++) {
                for (int x = 0; x < resolution; x++, ti += 6, vi++) {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + resolution + 1;
                    triangles[ti + 5] = vi + resolution + 2;
                }
            }
        }

        public override void GenerateMesh() {
            vertices = bezier.SampleCubicBezierCurve(segments, width);

            meshFilter = GetComponent<MeshFilter>();
            meshGenerator = meshGenerator?? new MeshGenerator();
            meshGenerator.Clear();

            var mVertices = new Vector3[vertices.Length * (resolution + 1)];
            var index = 0;
            
            // Generate the vertices
            foreach (var tuple in vertices) {
                for (int t = 0; t <= resolution; t++) {
                    var progress = ((float) t) / ((float) resolution);
                    var pt = Vector3.Lerp(tuple.item1, tuple.item2, progress);
                    mVertices[index] = pt;
                    index++;
                }
            }

            GenerateTriangles(vertices.Length);

            meshGenerator.AddVertices(mVertices);
            meshGenerator.AddTriangle(triangles);

            var mesh = meshGenerator.CreateMesh();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.name = name;

            meshFilter.sharedMesh = mesh;
        }
    }
}

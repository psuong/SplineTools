using CommonStructures;
using Curves.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Curves {

    public class CatmullRomMesh : BaseMesh {

        public CatmullRom catmullRom;
        public float width = 1f;
        public int resolution = 1;
        public int segments = 10;

        private Tuple<Vector3, Vector3>[] vertices;
        private List<Vector3> meshVertices;

        protected override void GenerateTriangles() {
            var ySize = segments * catmullRom.SplineCount;
            triangles = new int[resolution * 6 * ySize];
            for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
                for (int x = 0; x < resolution; x++, ti += 6, vi++) {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + resolution + 1;
                    triangles[ti + 5] = vi + resolution + 2;
                }
            }
        }

        public override void GenerateMesh() {
            vertices = catmullRom.SampleCatmullRomSpline(segments, width);

            meshGenerator = meshGenerator?? new MeshGenerator();
            meshFilter = GetComponent<MeshFilter>();
            meshVertices = new List<Vector3>();

            meshGenerator.Clear();

            var size = vertices.Length;

            for (int y = 0, i = 0; y < size; y++) {
                var tuple = vertices[y];
                for (int x = 0; x <= resolution; x++, i++) {
                    var t = (float) x / (float) resolution;
                    var pt = Vector3.Lerp(tuple.item1, tuple.item2, t);
                    meshVertices.Add(pt);
                }
            }

            GenerateTriangles();
            meshGenerator.AddVertices(meshVertices.ToArray());
            meshGenerator.AddTriangles(triangles);

            var total = catmullRom.GetTotalSplineDistance(segments);
            meshGenerator.AddUVs(meshVertices.Count, resolution, segments * catmullRom.SplineCount, total);

            var mesh = meshGenerator.CreateMesh();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            mesh.name = name; meshFilter.sharedMesh = mesh;
        }
    }
}

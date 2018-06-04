using CommonStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Curves {

    using Curves.Utility;

    public class BezierMesh : BaseMesh {

        [SerializeField]
        private Bezier bezier;
        [SerializeField]
        private float t = 100f;
        [SerializeField, Range(90f, 270f)]
        private float angle = 180f;
        [SerializeField]
        private float radius = 1f;
        [SerializeField]
        private int resolution = 1;

        // Store the vertices for the mesh.
        private Tuple<Vector3, Vector3>[] vertices;
        private int[] triangles;

        private void Start() {
            GeneratePoints();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;

            Gizmos.DrawWireSphere(transform.position, 0.1f);

            var p0 = transform.TransformPoint(MathUtility.GetCircleXZPoint(transform.position, radius, angle));
            var p1 = transform.TransformPoint(MathUtility.GetCircleXZPoint(transform.position, radius, angle - 180f));

            Gizmos.DrawWireSphere(transform.InverseTransformPoint(p0), 0.1f);
            Gizmos.DrawWireSphere(transform.InverseTransformPoint(p1), 0.1f);

            GeneratePoints();

            try {
                for (int i = 1; i < vertices.Length; i++) {
                    var start = vertices[i - 1];
                    var end = vertices[i];

                    Gizmos.DrawLine(transform.TransformPoint(end.item1), transform.TransformPoint(start.item1));
                    Gizmos.DrawLine(transform.TransformPoint(end.item2), transform.TransformPoint(start.item2));
                }
            } catch (System.Exception) { }
        }
#endif
        private void GeneratePoints() {
            var bezierPoints = bezier.GetCubicBezierPoints(t);

            vertices = new Tuple<Vector3, Vector3>[bezierPoints.Length];

            for (int i = 0; i < bezierPoints.Length; i++) {
                var left = MathUtility.GetCircleXZPoint(bezierPoints[i], radius, angle);
                var right = MathUtility.GetCircleXZPoint(bezierPoints[i], radius, angle - 180f);

                vertices[i] = Tuple<Vector3, Vector3>.CreateTuple(left, right);
            }
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

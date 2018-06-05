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

        private IList<Vector3> leftHandPoints;
        private IList<Vector3> rightHandPoints;
        private IList<Vector3> actualPoints;

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
                for (int i = 1; i < actualPoints.Count; i++) {
                    Gizmos.DrawLine(actualPoints[i - 1], actualPoints[i]);
                    Gizmos.DrawLine(rightHandPoints[i - 1], rightHandPoints[i]);
                    Gizmos.DrawLine(leftHandPoints[i - 1], leftHandPoints[i]);
                }
            } catch (System.Exception) { }
        }
#endif
        private void GeneratePoints() {
            // var bezierPoints = bezier.GetCubicBezierPoints(t);

            var bezierPoints = bezier.points;
            var controls = bezier.controlPoints;

            leftHandPoints = new List<Vector3>();
            rightHandPoints = new List<Vector3>();
            actualPoints = new List<Vector3>();

            vertices = new Tuple<Vector3, Vector3>[bezierPoints.Length];

            for (int i = 1; i < bezierPoints.Length; i++) {
                var start = bezierPoints[i - 1];
                var end = bezierPoints[i];

                var startR = MathUtility.GetCircleXZPoint(start, radius, angle);
                var endR = MathUtility.GetCircleXZPoint(end, radius, angle);

                var startL = MathUtility.GetCircleXZPoint(start, radius, angle - 180f);
                var endL = MathUtility.GetCircleXZPoint(end, radius, angle - 180f);

                var controlStart = controls[i == 1 ? 0 : i];
                var controlEnd = controls[i == 1 ? i : i + (i - 1)];

                var controlStartR = MathUtility.GetCircleXZPoint(controlStart, radius, angle);
                var controlEndR = MathUtility.GetCircleXZPoint(controlEnd, radius, angle);

                var controlStartL = MathUtility.GetCircleXZPoint(controlStart, radius, angle - 180f);
                var controlEndL = MathUtility.GetCircleXZPoint(controlEnd, radius, angle - 180f);

                var tInterval = 1f / t;

                for (float j = 0; j < t; j += tInterval) {
                    var point = Bezier.GetCubicBezierCurve(start, controlStart, controlEnd, end, j);
                    var rPoint = Bezier.GetCubicBezierCurve(startR, controlStartR, controlEndR, endR, j);
                    var lPoint = Bezier.GetCubicBezierCurve(startL, controlStartL, controlEndL, endL, j);
                    actualPoints.Add(point);
                    rightHandPoints.Add(rPoint);
                    leftHandPoints.Add(lPoint);
                }
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

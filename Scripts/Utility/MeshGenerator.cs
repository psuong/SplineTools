using Curves.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Curves {

    public class MeshGenerator { 
        public IList<Vector3> Vertices {
            get {
                return vertices.AsReadOnly();
            }
        }

        public IList<Vector3> Normals {
            get {
                return normals.AsReadOnly();
            }
        }

        public IList<Vector2> UVs {
            get {
                return uvs.AsReadOnly();
            }
        }

        private List<Vector3> vertices, normals;        // Store coordinates to generate all properties of the mesh
        private List<Vector2> uvs;                      // Store the 2D representation of the UVs.
        private List<int> triangles;                    // Store the triangles to generate the triangle

        public MeshGenerator() {
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            uvs = new List<Vector2>();
            triangles = new List<int>();
        }

        /// <summary>
        /// Stores a normal coordinate into the mesh generator.
        /// </summary>
        /// <param name="normal">A normal to the vertex.</param>
        public void AddNormal(Vector3 normal) {
            normals.Add(normal);
        }

        /// <summary>
        /// Stores a vertex into the mesh generator.
        /// </summary>
        /// <param name="vertex">A vertex on the mesh.</param>
        public void AddVertex(Vector3 vertex) {
            vertices.Add(vertex);
        }

        /// <summary>
        /// Adds all vertices to a list.
        /// </summary>
        /// <param name="vertices">A variable number of vertices to add.</param>
        public void AddVertices(params Vector3[] vertices) {
            this.vertices.AddRange(vertices);
        }

        /// <summary>
        /// Adds all normals to a list.
        /// </summary>
        /// <param name="normals">A variable number of normals to add.</param>
        public void AddNormals(params Vector3[] normals) {
            this.normals.AddRange(normals);
        }

        /// <summary>
        /// Adds all uv coordinates to a list.
        /// </summary>
        /// <param name="uvs">A variable number of uvs to add.</param>
        public void AddUVs(params Vector2[] uvs) {
            this.uvs.AddRange(uvs);
        }

        /// <summary>
        /// Generates a series of UV coordinates mapped relative to the size of the mesh.
        /// The x and y size should match the size parameter.
        /// </summary>
        /// <param name="size">How many UV coordinates should be stored?</param>
        /// <param name="xSize">How many segments should occur along the x axis?</param>
        /// <param name="ySize">How many segments should occur along the y axis?</param>
        public void AddUVs(int size, int xSize, int ySize) {
            var uvs = new Vector2[size];
            for (int y = 0, i = 0; y <= ySize; y++) {
                for (int x = 0; x <= xSize; x++, i++) {
                    uvs[i] = new Vector2((float) x / xSize, (float) y / ySize);
                }
            }
            AddUVs(uvs);
        }

        /// <summary>
        /// Generates a series of UV coordinates with each coordinate equidistant from each other. This avoids
        /// unnecessary stretching but compresses the coordinates together.
        /// </summary>
        /// <param name="size">How many uv coordinates should be stored?</param>
        /// <param name="xSize">How many segments should occur along the x axis?</param>
        /// <param name="ySize">How many segments should occur along the y axis?</param>
        /// <param name="totalDistance">What is the total length of the mesh?</param>
        public void AddUVs(int size, int xSize, int ySize, float totalDistance) {
            var uvs = new Vector2[size];
            for (int y = 0, i = 0; y <= ySize; y++) {
                for (int x = 0; x <= xSize; x++, i++) {
                    // Generate the (UV) coordinate
                    uvs[i] = new Vector2((float) x / xSize, totalDistance);
                }
            }
            AddUVs(uvs);
        }

        /// <summary>
        /// Generates and assigns series of UV coordinates with each coordinate relatively equidistant from each other. This samples 
        /// the distance of the assumed point at the parametric value, t.
        /// </summary>
        /// <param name="size">How many uv coordinates should be stored?</param>
        /// <param name="xSize">How many segments should occur along the x axis?</param>
        /// <param name="ySize">How many segments should occur along the y axis?</param>
        /// <param name="distances">The distance of each vertex from the start of the spline.</param>
        /// <param name="factor">What factor should the sampled distances be multiplied by, by default 1.</param>
        public void AddUVs(int size, int xSize, int ySize, float[] distances, float factor = 1f) {
            var uvs = new Vector2[size];
            for (int y = 0, i = 0; y <= ySize; y++) {
                var t = ((float) y / ySize);
                for (int x = 0; x <= xSize; x++, i++) {
                    var u = ((float) x / xSize);
                    var v = distances.Sample(t) * factor;
                    uvs[i] = new Vector2(u, v);
                }
            }
            AddUVs(uvs);
        }

        // TODO: Function cleanup for adding UVs.
        /// <summary>
        /// Generates and assigns a series of UV coordinates by sampling the distances of the assumed point at a parametric value, t for each
        /// spline in the distance.
        /// </summary>
        /// <param name="size">How many uv coordinates should be stored?</param>
        /// <param name="xSize">How many segments should occur along the x axis?</param>
        /// <param name="ySize">How many segments should occur along the y axis?</param>
        /// <param name="splineCount">The number of splines within the curve.</param>
        /// <param name="distances">The distance of each vertex from the start of the spline.</param>
        /// <param name="factor">The factor should the sampled distances be multiplied by, by default 1.</param>
        public void AddUVs(int size, int xSize, int ySize, int splineCount, float[][] distances, float factor = 1f) {
            var uvs = new Vector2[size];

            // Store the total number of segments.
            var segments = ySize / splineCount;
            var splineCounter = 0;
            var progress = 0;
            for (int y = 0, i = 0; y <= ySize; y++) {
                // Increment which spline to retrieve if y is greater than the (1 based) splineCounter * segments
                if (y > (splineCounter + 1) * segments) {
                    splineCounter++;
                }

                if (y % segments == 1 && y > segments) {
                    progress = 0;
                } else if (y > 0) {
                    progress++;
                }

                // progress = (y > 0 && y % segments == 1) ? 0 : progress++;

                var t = (float) progress / segments;
                Debug.LogFormat("y: {0}, SplineCounter: {1}, t: {2}, Progress: {3}", y, splineCounter, t, progress);
                try {
                    float[] splineDistances = distances[splineCounter];
                    for (int x = 0; x <= xSize; x++, i++) {
                        var u = ((float) x / xSize);
                        var v = splineDistances.Sample(t) * factor;
                        uvs[i] = new Vector2(u, v);
                    }

                } catch (System.IndexOutOfRangeException) { }
            }
            AddUVs(uvs);
        }

        /// <summary>
        /// Adds all indices of a triangle to a list.
        /// </summary>
        /// <param name="triangles">An array of indices to add.</param>
        public void AddTriangles(params int[] triangles) {
            this.triangles.AddRange(triangles);
        }

        /// <summary>
        /// Adds a triangle to the mesh.
        /// </summary>
        public void AddTriangle(int p0, int p1, int p2) {
            triangles.Add(p0);
            triangles.Add(p1);
            triangles.Add(p2);
        }

        /// <summary>
        /// Resets all of the vector coordinates in the MeshGenerator.
        /// </summary>
        public void Clear() {
            vertices.Clear();
            uvs.Clear();
            normals.Clear();
            triangles.Clear();
        }

        /// <summary>
        /// Creates a mesh with only the vertices and the triangles.
        /// </summary>
        /// <returns>A mesh object</returns>
        public Mesh CreateDefaultMesh() {
            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }

        /// <summary>
        /// Constructs a mesh object with the normals and uvs.
        /// </summary>
        /// <returns>A mesh object</returns>
        public Mesh CreateMesh() {
            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            if (normals.Count == vertices.Count) {
                mesh.normals = normals.ToArray();
            }

            if (uvs.Count == vertices.Count) {
                mesh.uv = uvs.ToArray();
            }

            mesh.RecalculateBounds();

            return mesh;
        }

    }
}

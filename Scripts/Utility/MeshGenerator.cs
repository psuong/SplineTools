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
                return uvs.AsReadOnly(); }
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

        // TODO: Add the docStrings
        public void AddNormal(Vector3 normal) {
            normals.Add(normal);
        }

        // TODO: Add the docStrings
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
        /// Adds all normals to a list.
        /// </summary>
        /// <param name="uvs">A variable number of uvs to add.</param>
        public void AddUVs(params Vector2[] uvs) {
            this.uvs.AddRange(uvs);
        }

        /// <summary>
        /// Adds all indices of a triangle to a list.
        /// </summary>
        /// <param name="triangles">An array of indices to add.</param>
        public void AddTriangle(params int[] triangles) {
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

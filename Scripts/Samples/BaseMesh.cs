using UnityEngine;

namespace Curves {

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public abstract class BaseMesh : MonoBehaviour, IMesh {

        protected MeshGenerator meshGenerator;
        protected MeshFilter meshFilter;

        protected int[] triangles;

        protected abstract void GenerateTriangles();

        /// <summary>
        /// Creates a mesh and assigns it to the meshFilter.
        /// </summary>
        public abstract void GenerateMesh();
    }
}

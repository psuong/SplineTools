using UnityEngine;

namespace Curves {

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public abstract class BaseMesh : MonoBehaviour, IMesh {

        protected MeshGenerator meshGenerator;
        protected MeshFilter meshFilter;

        /// <summary>
        /// Creates a mesh and assigns it to the meshFilter.
        /// </summary>
        public abstract void GenerateMesh();
    }
}

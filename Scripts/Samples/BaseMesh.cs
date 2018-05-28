using UnityEngine;

namespace Curves {
    
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public abstract class BaseMesh : MonoBehaviour, IMesh {
        
        protected MeshGenerator meshGenerator;
        protected MeshFilter meshFilter;
        
        public abstract void GenerateMesh();
    }
}

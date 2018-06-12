using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {
    
    [CustomEditor(typeof(BezierMesh), true)]
    public class BezierMeshEditor : BaseMeshEditor {
            
        protected override void OnEnable() {
            base.OnEnable();
        }

        protected override void OnDisable() {
            base.OnDisable();
        }
    }
}

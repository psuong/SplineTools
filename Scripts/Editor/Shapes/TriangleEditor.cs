using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(Triangle))]
    public class TriangleEditor : Editor {
        
        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                DrawDefaultInspector();
                DrawMeshGeneratorButton();
            }
        }

        private void DrawMeshGeneratorButton() {
            if (GUILayout.Button(new GUIContent("Generate Mesh"))) {
                Triangle instance = target as Triangle;
                instance.GenerateMesh();
            }
        }
    }
}

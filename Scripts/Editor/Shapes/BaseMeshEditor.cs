using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {
    
    [CustomEditor(typeof(BaseMesh), true)]
    public class BaseMeshEditor : Editor {
        
        protected BaseMesh meshTool;

        protected virtual void OnEnable() {
            meshTool = target as BaseMesh;
        }

        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();

                DrawDefaultInspector();
                if (GUILayout.Button("Generate Mesh")) {
                    meshTool.GenerateMesh();
                }
                if (changeCheck.changed) {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}

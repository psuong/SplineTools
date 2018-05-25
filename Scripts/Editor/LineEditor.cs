using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(Line))]
    public class LineEditor : Editor {

        private SerializedProperty p0;
        private SerializedProperty p1;
        private TransformData transformData;

        private void OnEnable() {
            p0 = serializedObject.FindProperty("p0");
            p1 = serializedObject.FindProperty("p1");

            // SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        private void OnDisable() {
            // SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView) {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                DrawLine();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawLine() {
            Handles.color = Color.red;
            
            var start = p0.vector3Value;
            var end = p0.vector3Value;

            Handles.DrawLine(p0.vector3Value, p0.vector3Value);
        }
    }
}

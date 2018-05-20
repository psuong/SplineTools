using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {
    
    [CustomEditor(typeof(Line))]
    public class LineEditor : Editor {

        private SerializedProperty p0;
        private SerializedProperty p1;
        
        private void OnEnable() {
            p0 = serializedObject.FindProperty("p0");
            p1 = serializedObject.FindProperty("p1");
        }

        private void OnSceneGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                // Draw the line
                DrawLine();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawLine() {
            Handles.color = Color.green;
            Handles.DrawLine(p0.vector3Value, p1.vector3Value);
        }
    }
}

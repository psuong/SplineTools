using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(Line))]
    public class LineEditor : Editor {

        private const float DotHandleSize = 0.05f;

        private Transform transform;
        private SerializedProperty p0;
        private SerializedProperty p1;

        private bool clampHeight;
        private float cachedStartHeight;
        private float cachedEndHeight;

        private void OnEnable() {
            var line = target as Line;
            transform = line.transform;

            p0 = serializedObject.FindProperty("p0");
            p1 = serializedObject.FindProperty("p1");

            clampHeight = false;

            SetWorldSpaceCoordinates(p0.vector3Value, p1.vector3Value);
        }

        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                DrawDefaultInspector();
                DrawClampHeightButton();
                DrawResetButton();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnSceneGUI() {
            // Draw the line
            DrawLine();
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                DrawLineHandles();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawClampHeightButton() {
            if (GUILayout.Button(clampHeight ? "Clamp Height: Off" : "Clamp Height: On")) {
                clampHeight = !clampHeight;
            }
        }

        private void DrawLine() {
            Handles.color = Color.green;
            var start = transform.TransformPoint(p0.vector3Value);
            var end = transform.TransformPoint(p1.vector3Value);

            Handles.DrawLine(start, end);
        }

        private void DrawLineHandles() {
            var start = transform.TransformPoint(p0.vector3Value);
            var end = transform.TransformPoint(p1.vector3Value);
            start = Handles.PositionHandle(start, Quaternion.identity);
            end = Handles.PositionHandle(end, Quaternion.identity);

            p0.vector3Value = transform.InverseTransformPoint(start);
            p1.vector3Value = transform.InverseTransformPoint(end);
        }

        private void DrawResetButton() {
            if (GUILayout.Button("Reset")) {
                Reset();
            }
        }

        private void SetWorldSpaceCoordinates(Vector3 start, Vector3 end) {
            p0.vector3Value = transform.TransformPoint(start);
            p1.vector3Value = transform.TransformPoint(end);
        }

        private void SetLocalSpaceCoordinates() {
            p0.vector3Value = transform.InverseTransformPoint(p0.vector3Value);
            p1.vector3Value = transform.InverseTransformPoint(p1.vector3Value);
        }

        private void Reset() {
            try {
                p0.vector3Value = p1.vector3Value = Vector3.zero;
            } catch (System.NullReferenceException) {}
        }
    }
}

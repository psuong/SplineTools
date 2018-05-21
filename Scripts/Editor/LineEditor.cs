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
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                // Draw the line
                DrawLine();
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

            var snap = Vector3.one * DotHandleSize;
            var size = HandleUtility.GetHandleSize(p0.vector3Value) * DotHandleSize;

            var start = p0.vector3Value;
            var end = p1.vector3Value;

            if (!clampHeight) {
                cachedStartHeight = start.y;
                cachedEndHeight = end.y;
            }

            var startPoint = clampHeight ? new Vector3(start.x, cachedStartHeight, start.z) : start;
            var endPoint = clampHeight ? new Vector3(end.x, cachedEndHeight, end.z) : end;

            var newStart = Handles.FreeMoveHandle(startPoint, Quaternion.identity, size, snap, Handles.DotHandleCap);
            var newEnd = Handles.FreeMoveHandle(endPoint, Quaternion.identity, size, snap, Handles.DotHandleCap);

            SetWorldSpaceCoordinates(newStart, newEnd);

            Handles.DrawLine(p0.vector3Value, p1.vector3Value);
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

        private void Reset() {
            p0.vector3Value = p1.vector3Value = Vector3.zero;
        }
    }
}

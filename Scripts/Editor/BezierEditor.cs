using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {
    [CustomEditor(typeof(Bezier))]
    public class BezierEditor : Editor {

        private const float HandleSize = 0.075f;

        private SerializedProperty points;
        private SerializedProperty controlPoints;
        
        private void OnEnable() {
            points = serializedObject.FindProperty("points");
            controlPoints = serializedObject.FindProperty("controlPoints");
        }

        private void OnSceneGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                DrawBezierCurve();
                DrawPoints();
                DrawControlPoints();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawBezierCurve() {
            Handles.color = Color.green;
            var size = points.arraySize;
            for (int i = 1; i < size; i++) {
                var start = points.GetArrayElementAtIndex(i - 1);
                var end = points.GetArrayElementAtIndex(i);

                var controlStart = controlPoints.GetArrayElementAtIndex(i == 1 ? 0 : i);
                var controlEnd = controlPoints.GetArrayElementAtIndex(i == 1 ? i : i + (i - 1));

                Handles.DrawBezier(start.vector3Value, end.vector3Value, controlStart.vector3Value, controlEnd.vector3Value, Color.red, null, HandleUtility.GetHandleSize(Vector3.zero) * 0.5f);
            }
        }

        private void DrawPoints() {
            var size = points.arraySize;
            for (int i = 0; i < size; i++) {
                var element = points.GetArrayElementAtIndex(i);
                var snapSize = Vector3.one * HandleSize;
                element.vector3Value = Handles.FreeMoveHandle(element.vector3Value, Quaternion.identity, HandleSize, snapSize, Handles.DotHandleCap);
            }
        }

        private void DrawControlPoints() {
            var size = controlPoints.arraySize;
            for (int i = 0; i < size; i++) {
                var element = controlPoints.GetArrayElementAtIndex(i);
                var snapSize = Vector3.one * HandleSize;
                element.vector3Value = Handles.FreeMoveHandle(element.vector3Value, Quaternion.identity, HandleSize, snapSize, Handles.DotHandleCap);
            }
        }
    }
}

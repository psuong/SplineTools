using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(Line))] public class LineEditor : SceneViewEditor {

        private const float HandleSize = 0.15f;

        private SerializedProperty p0;
        private SerializedProperty p1;

        protected override void OnEnable() {
            base.OnEnable();

            jsonPath = System.IO.Path.Combine(jsonDirectory, string.Format("{0}.json", (target as Line).name));

            p0 = serializedObject.FindProperty("p0");
            p1 = serializedObject.FindProperty("p1");
        }

        protected override void OnDisable() {
            base.OnDisable();
        }

        protected override void OnSceneGUI(SceneView sceneView) {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                LoadTransformData();
                serializedObject.Update();
                DrawLine();
                DrawLineHandles();
                DrawTransformHandle();
                serializedObject.ApplyModifiedProperties();
                SaveTransformData();
            }
        }

        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                LoadTransformData();
                serializedObject.Update();
                DrawTransformField();
                DrawDefaultInspector();
                
                VectorListUtility.ResetHeight(0f, p0, p1);

                serializedObject.ApplyModifiedProperties();
                SaveTransformData();

                if (changeCheck.changed) {
                    SceneView.RepaintAll();
                }
            }
        }

        private void DrawLine() {
            Handles.color = Color.red;

            var trs = transformData.TRS;

            var start = trs.MultiplyPoint3x4(p0.vector3Value);
            var end = trs.MultiplyPoint3x4(p1.vector3Value);

            Handles.DrawLine(start, end);
        }

        private void DrawLineHandles() {
            Handles.color = Color.green;
            var trs = transformData.TRS;
            var start = trs.MultiplyPoint3x4(p0.vector3Value);
            var end = trs.MultiplyPoint3x4(p1.vector3Value);

            var snapSize = Vector3.one * HandleSize;

            var startPosition = Handles.FreeMoveHandle(start, Quaternion.identity, HandleSize * 2, snapSize * 2, Handles.DotHandleCap);
            var endPosition = Handles.FreeMoveHandle(end, Quaternion.identity, HandleSize * 2, snapSize * 2, Handles.DotHandleCap);

            p0.vector3Value = trs.inverse.MultiplyPoint3x4(startPosition);
            p1.vector3Value = trs.inverse.MultiplyPoint3x4(endPosition);
        }
    }
}

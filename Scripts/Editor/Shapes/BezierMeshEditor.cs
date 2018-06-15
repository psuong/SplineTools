using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(BezierMesh), true)]
    public class BezierMeshEditor : BaseMeshEditor {

        private Editor customEditor;
        private SerializedProperty bezierProperty;
        private Transform transform;
        private bool foldoutState;


        protected override void OnEnable() {
            base.OnEnable();
            bezierProperty = serializedObject.FindProperty("bezier");
            transform = ((MonoBehaviour)target).transform;

            customEditor = Editor.CreateEditor(bezierProperty.objectReferenceValue);

            onSceneCallback += RedrawMesh;
            onSceneCallback += DrawSceneViewEditor;
            onInspectorCallback += DrawBezierEditor;
            onInspectorCallback += RedrawMesh;
        }

        protected override void OnDisable() {
            onSceneCallback -= RedrawMesh;
            onSceneCallback -= DrawSceneViewEditor;
            onInspectorCallback -= DrawBezierEditor;
            onInspectorCallback -= RedrawMesh;
        }

        private void DrawBezierEditor() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();

                foldoutState = EditorGUILayout.Foldout(foldoutState, "Bezier");
                if (foldoutState) {
                    customEditor.OnInspectorGUI();
                }

                if (changeCheck.changed) {
                    bezierProperty.serializedObject.ApplyModifiedProperties();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawBezierCurve(Vector3[] points, Vector3[] controlPoints) {
            try {
                var size = points.Length;

                for (int i = 1; i < size; i++) {
                    var start = points[i - 1];
                    var end = points[i];
                    var index = i * 2;

                    var controlStart = controlPoints[index - 2];
                    var controlEnd = controlPoints[index - 1];

                    Handles.DrawBezier(
                            transform.TransformPoint(start),
                            transform.TransformPoint(end),
                            transform.TransformPoint(controlStart),
                            transform.TransformPoint(controlEnd),
                            Color.red,
                            null,
                            HandleUtility.GetHandleSize(Vector3.one) * 0.5f);
                }
            } catch (System.NullReferenceException) { }
        }

        private void DrawPointHandles(Vector3[] points, Color handlesColor) {
            var size = points.Length;
            var handleSize = 0.15f;
            Handles.color = handlesColor;

            for(int i = 0; i < size; i++) {
                var elem = points[i];

                var point = transform.TransformPoint(elem);
                var snapSize = Vector3.one * handleSize;
                point = Handles.FreeMoveHandle(point, Quaternion.identity, handleSize * 2, snapSize * 2, Handles.DotHandleCap);
                points[i] = transform.InverseTransformPoint(point);
            }
        }

        private void DrawSceneViewEditor() {
            try {
                var bezierMesh = meshTool as BezierMesh;
                var bezier = bezierMesh.bezier;
                DrawBezierCurve(bezier.points, bezier.controlPoints);
                DrawPointHandles(bezier.points, Color.green);
                DrawPointHandles(bezier.controlPoints, Color.cyan);
            } catch (System.NullReferenceException) { }
        }

        private void RedrawMesh() {
            meshTool.GenerateMesh();
        }
    }
}

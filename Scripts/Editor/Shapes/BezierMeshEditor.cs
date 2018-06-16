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

        private void DrawSceneViewEditor() {
            try {
                var bezierMesh = meshTool as BezierMesh;
                var bezier = bezierMesh.bezier;

                DrawBezierCurve(bezier.points, bezier.controlPoints);

                BezierEditor.DrawHandlePoints(bezier.points, Color.green, transform);
                BezierEditor.DrawHandlePoints(bezier.controlPoints, Color.cyan, transform);
            } catch (System.NullReferenceException) { }
        }

        private void RedrawMesh() {
            meshTool.GenerateMesh();
        }
    }
}

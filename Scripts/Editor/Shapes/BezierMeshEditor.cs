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
                if (foldoutState && bezierProperty.objectReferenceValue != null) {
                    customEditor.OnInspectorGUI();
                }

                if (changeCheck.changed) {
                    bezierProperty.serializedObject.ApplyModifiedProperties();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        } 

        private void DrawSceneViewEditor() {
            try {
                var bezierMesh = meshTool as BezierMesh;
                var points = bezierMesh.bezier.points;
                
                BezierEditor.DrawCubicBezierCurve(points, transform, Color.red);
                BezierEditor.DrawHandlePoints(points, Color.green, transform);
            } catch (System.NullReferenceException) { }
        }

        private void RedrawMesh() {
            meshTool.GenerateMesh();
        }
    }
}

using Curves;
using Curves.Utility;
using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(CatmullRomMesh))]
    public class CatmullRomMeshEditor : BaseMeshEditor {

        private Editor customEditor;
        private SerializedProperty catmullRomProperty;
        private Transform transform;
        private bool foldoutState;

        protected override void OnEnable() {
            base.OnEnable();
            catmullRomProperty = serializedObject.FindProperty("catmullRom");
            transform = ((MonoBehaviour)target).transform;

            customEditor = Editor.CreateEditor(catmullRomProperty.objectReferenceValue);
            (customEditor as SceneViewEditor).ShowTransformField = false;

            onInspectorCallback += DrawCatmullRomEditor;
            onInspectorCallback += RedrawMesh;
            onSceneCallback += DrawSceneViewEditor;
            onSceneCallback += RedrawMesh;
        }
        
        protected override void OnDisable() {
            onInspectorCallback -= DrawCatmullRomEditor;
            onInspectorCallback -= RedrawMesh;
            onSceneCallback -= DrawSceneViewEditor;
            onSceneCallback -= RedrawMesh;
        }

        private void DrawCatmullRomEditor() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                foldoutState = EditorGUILayout.Foldout(foldoutState, "Catmull Rom");
                
                if (foldoutState && catmullRomProperty.objectReferenceValue != null) {
                    customEditor.OnInspectorGUI();
                }

                if (changeCheck.changed) {
                    catmullRomProperty.serializedObject.ApplyModifiedProperties();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawSceneViewEditor() {
            try {
                var catmullRomMesh = meshTool as CatmullRomMesh;
                var points = catmullRomMesh.catmullRom.points;

                CatmullRomEditor.DrawCatmullRomSpline(catmullRomProperty.objectReferenceValue as CatmullRom, transform, Color.red);
                VectorHandles.DrawHandlePoints(points, Color.green, transform, 0.1f);
            } catch (System.NullReferenceException) { }
        }

        private void RedrawMesh() {
            meshTool.GenerateMesh();
        }
    }
}

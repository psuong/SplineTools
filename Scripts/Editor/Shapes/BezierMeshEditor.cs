using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {
    
    [CustomEditor(typeof(BezierMesh), true)]
    public class BezierMeshEditor : BaseMeshEditor {

        private Editor customEditor;
        private SerializedProperty bezierProperty;
        private bool foldoutState;

        protected override void OnEnable() {
            base.OnEnable();
            
            bezierProperty = serializedObject.FindProperty("bezier");
            customEditor = Editor.CreateEditor(bezierProperty.objectReferenceValue);

            onSceneCallback += RedrawMesh;
            onInspectorCallback += DrawBezierEditor;
            onInspectorCallback += RedrawMesh;
        }

        protected override void OnDisable() {
            base.OnDisable();
            onSceneCallback -= RedrawMesh;
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
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawSceneViewEditor() {
            // TODO: Draw the scene view of the scriptable.
        }

        private void RedrawMesh() {
            meshTool.GenerateMesh();
        }
    }
}

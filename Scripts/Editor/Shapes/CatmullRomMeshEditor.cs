using Curves;
using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(CatmullRomMesh))]
    public class CatmullRomMeshEditor : BaseMeshEditor {

        private Editor customEditor;
        private SerializedProperty catmullRomProperty;
        private bool foldoutState;

        protected override void OnEnable() {
            base.OnEnable();
            catmullRomProperty = serializedObject.FindProperty("catmullRom");

            customEditor = Editor.CreateEditor(catmullRomProperty.objectReferenceValue);
            onInspectorCallback += DrawCatmullRomEditor;
        }
        
        protected override void OnDisable() {
            onInspectorCallback -= DrawCatmullRomEditor;           
        }

        private void DrawCatmullRomEditor() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                foldoutState = EditorGUILayout.Foldout(foldoutState, "Bezier");
                
                if (foldoutState && catmullRomProperty.objectReferenceValue != null) {
                    customEditor.OnInspectorGUI();
                }

                if (changeCheck.changed) {
                    catmullRomProperty.serializedObject.ApplyModifiedProperties();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}

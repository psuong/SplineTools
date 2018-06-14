using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    public delegate void SceneHandler();
    public delegate void InspectorHandler();
    public delegate void ChangeHandler();
    public delegate void UndoHandler();

    [CustomEditor(typeof(BaseMesh), true)]
    public class BaseMeshEditor : Editor {
        
        protected BaseMesh meshTool;
        protected ChangeHandler onChangeCallback;
        protected InspectorHandler onInspectorCallback;
        protected SceneHandler onSceneCallback;

        protected virtual void OnEnable() {
            meshTool = target as BaseMesh;
        }

        protected virtual void OnDisable() { }

        protected virtual void OnSceneGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                
                if (onSceneCallback != null) {
                    onSceneCallback();
                }

                if (onSceneCallback != null) {
                    onSceneCallback();
                }

                if (changeCheck.changed) {
                    if (onChangeCallback != null) {
                        onChangeCallback();
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();

                DrawDefaultInspector();

                if (onInspectorCallback != null) {
                    onInspectorCallback();
                }

                DrawMeshGenerationButton();

                if (changeCheck.changed) {
                    if (onChangeCallback != null) {
                        onChangeCallback();
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        protected virtual void RegenerateMesh() {
            if (meshTool) {
                meshTool.GenerateMesh();
            }
        }

        protected void DrawMeshGenerationButton() {
            if (GUILayout.Button(new GUIContent("Generate Mesh", "Creates a mesh and assigns it to the MeshFilter."))) {
                meshTool.GenerateMesh();
            }
        }
    }
}

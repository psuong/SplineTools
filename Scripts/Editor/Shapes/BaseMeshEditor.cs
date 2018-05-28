using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    public delegate void SceneHandler();
    public delegate void InspectorHandler();

    [CustomEditor(typeof(BaseMesh), true)]
    public class BaseMeshEditor : Editor {
        
        protected BaseMesh meshTool;
        protected SceneHandler onSceneCallback;
        protected InspectorHandler onInspectorCallback;

        protected virtual void OnEnable() {
            meshTool = target as BaseMesh;
        }

        protected virtual void OnDisable() {
            
        }

        protected virtual void OnSceneGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                
                if (onSceneCallback != null) {
                    onSceneCallback();
                }

                if (changeCheck.changed) {
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

                if (GUILayout.Button("Generate Mesh")) {
                    meshTool.GenerateMesh();
                }
                if (changeCheck.changed) {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}

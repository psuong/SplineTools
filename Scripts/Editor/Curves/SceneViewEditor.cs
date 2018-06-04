using Curves.Utility;
using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    public abstract class SceneViewEditor : Editor {

        protected TransformData transformData;
        protected string jsonDirectory;
        protected string jsonPath;

        protected virtual void OnEnable() {
            jsonDirectory = System.IO.Path.Combine(Application.dataPath, "Scripts", "Editor", "Configurations");
            SceneView.onSceneGUIDelegate = OnSceneGUI;
        }

        protected virtual void OnDisable() {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        protected abstract void OnSceneGUI(SceneView sceneView);

        protected void DrawTransformField() {
            EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);
            transformData.position = EditorGUILayout.Vector3Field(new GUIContent("Position"), transformData.position);
            transformData.rotation = EditorGUILayout.Vector3Field(new GUIContent("Rotation"), transformData.rotation);
            transformData.scale = EditorGUILayout.Vector3Field(new GUIContent("Scale"), transformData.scale);
            transformData.showTransformData = EditorGUILayout.Toggle("Show Transform", transformData.showTransformData);
        }

        protected void DrawTransformHandle() {
            if (transformData.showTransformData) {
                transformData.position = Handles.PositionHandle(transformData.position, Quaternion.Euler(transformData.rotation));
                transformData.rotation = Handles.RotationHandle(Quaternion.Euler(transformData.rotation), transformData.position).eulerAngles;
                transformData.scale = Handles.ScaleHandle(transformData.scale, transformData.position, Quaternion.Euler(transformData.rotation), transformData.scale.magnitude);
            }
        }

        protected void LoadTransformData() {
            if (System.IO.File.Exists(jsonPath)) {
                var jsonRep = System.IO.File.ReadAllText(jsonPath);
                transformData = JsonUtility.FromJson<TransformData>(jsonRep);
            } else {
                transformData = TransformData.CreateTransformData();
                System.IO.Directory.CreateDirectory(jsonDirectory);
                System.IO.File.WriteAllText(jsonPath, JsonUtility.ToJson(transformData));
            }
        }

        protected void SaveTransformData() {
            var json = JsonUtility.ToJson(transformData);
            System.IO.File.WriteAllText(jsonPath, json);
        }

        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}

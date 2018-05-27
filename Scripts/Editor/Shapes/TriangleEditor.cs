using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(Triangle))]
    public class TriangleEditor : Editor {

        private const float HandleSize = 0.015f;

        private SerializedProperty vertices;
        private SerializedProperty normals;
        private SerializedProperty uvs;
        private SerializedProperty triangleIndices;

        private Triangle triangle;

        private void OnEnable() {
            triangle = target as Triangle;

            vertices = serializedObject.FindProperty("vertices");
            normals = serializedObject.FindProperty("normals");
            uvs = serializedObject.FindProperty("uvs");
            triangleIndices = serializedObject.FindProperty("triangleIndices");
        }

        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                DrawDefaultInspector();
                DrawMeshGeneratorButton();
                triangle.GenerateMesh();
                if (changeCheck.changed) {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void OnSceneGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();

                DrawVertexHandles();

                if (changeCheck.changed) {
                    triangle.GenerateMesh();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawMeshGeneratorButton() {
            if (GUILayout.Button(new GUIContent("Generate Mesh"))) {
                Triangle instance = target as Triangle;
                instance.GenerateMesh();
            }
        }

        private void DrawVertexHandles() {
            var size = vertices.arraySize;
            for (int i = 0; i < size; i++) {
                var element = vertices.GetArrayElementAtIndex(i);

                var position = triangle.transform.TransformPoint(element.vector3Value);
                position = Handles.FreeMoveHandle(position, Quaternion.identity, HandleSize, Vector3.one * HandleSize, Handles.DotHandleCap);
                
                element.vector3Value = triangle.transform.InverseTransformPoint(position);
            }
        }
    }
}

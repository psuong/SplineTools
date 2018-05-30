using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Curves.EditorTools {
    
    [CustomEditor(typeof(QuadMesh))]
    public class QuadMeshEditor : BaseMeshEditor {

        private const float HandleSize = 0.015f;

        private SerializedProperty points;

        private ReorderableList pointsList;
        private QuadMesh quad;
        private string[] labels = { "Bottom Left", "Bottom Right", "Top Left", "Top Right" };
        
        protected override void OnEnable() {
            base.OnEnable();

            quad = target as QuadMesh;

            points = serializedObject.FindProperty("points");
            pointsList = new ReorderableList(serializedObject, points);

            onInspectorCallback += pointsList.DoLayoutList;
            onSceneCallback += DrawPointHandles;
            onChangeCallback += RegenerateMesh;
        }

        protected override void OnDisable() {
            onInspectorCallback -= pointsList.DoLayoutList;
            onSceneCallback -= DrawPointHandles;
            onChangeCallback -= RegenerateMesh;
        }

        private void DrawPointHandles() {
            Handles.color = Color.green;
            var size = points.arraySize;
            var transform = (target as MonoBehaviour).transform;
            for (int i = 0; i < size; i++) {
                var point = points.GetArrayElementAtIndex(i);
                var position = transform.TransformPoint(point.vector3Value);
                position = Handles.FreeMoveHandle(position, Quaternion.identity, HandleSize, Vector3.one * HandleSize, Handles.DotHandleCap);
                point.vector3Value = transform.InverseTransformPoint(position);
            }
        }

        private void DrawPointElement(Rect r, int i, bool isActive, bool isFocused) {
            var element = points.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(r, element, new GUIContent(labels[i]));
        }

        private void RegenerateMesh() {
            if (quad) {
                quad.GenerateMesh();
            }
        }
    }
}

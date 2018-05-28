using UnityEditor;
using UnityEngine;


namespace Curves.EditorTools {
    
    [CustomEditor(typeof(Quad))]
    public class QuadMeshEditor : BaseMeshEditor {

        private const float HandleSize = 0.015f;

        private SerializedProperty points;
        private Curves.Quad quad;
        
        protected override void OnEnable() {
            base.OnEnable();

            quad = target as Quad;

            points = serializedObject.FindProperty("points");
            onSceneCallback += DrawPointHandles;
            onChangeCallback += RegenerateMesh;
        }

        protected override void OnDisable() {
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

        private void RegenerateMesh() {
            if (quad) {
                quad.GenerateMesh();
            }
        }
    }
}

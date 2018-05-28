using UnityEditor;
using UnityEngine;


namespace Curves.EditorTools {
    
    [CustomEditor(typeof(Quad))]
    public class QuadMeshEditor : BaseMeshEditor {

        private SerializedProperty points;
        
        protected override void OnEnable() {
            base.OnEnable();

            points = serializedObject.FindProperty("points");

            onSceneCallback += DrawPoints;
        }

        protected override void OnDisable() {
            onSceneCallback -= DrawPoints;
        }

        private void DrawPoints() {
            var size = points.arraySize;
            var transform = (target as MonoBehaviour).transform;
            for (int i = 0; i < size; i++) {
                var point = points.GetArrayElementAtIndex(i);
                var position = transform.TransformPoint(point.vector3Value);
                position = Handles.FreeMoveHandle(position, Quaternion.identity, 0.03f, Vector3.one * 0.03f, Handles.DotHandleCap);
                point.vector3Value = transform.InverseTransformPoint(position);
            }
        }
    }
}

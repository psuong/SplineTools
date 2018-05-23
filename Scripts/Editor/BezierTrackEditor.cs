using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Curves.EditorTools {
    
    [CustomEditor(typeof(BezierTrack))]
    public class BezierTrackEditor : Editor {

        private const float HandleSize = 0.07f;

        private SerializedProperty points;
        private SerializedProperty controlPoints;

        private ReorderableList pointsList;
        private ReorderableList controlPointsList;
    
        private void OnEnable() {
            points = serializedObject.FindProperty("points");
            controlPoints = serializedObject.FindProperty("controlPoints");

            pointsList = new ReorderableList(serializedObject, points);
            controlPointsList = new ReorderableList(serializedObject, controlPoints, true, true, false, false);

            // Register the sceneview
            SceneView.onSceneGUIDelegate += OnSceneGUI;

            pointsList.drawHeaderCallback = DrawPointHeader;
            pointsList.drawElementCallback = DrawPointElement;
            pointsList.elementHeightCallback = ElementHeight;

            controlPointsList.drawHeaderCallback = DrawControlPointHeader;
            controlPointsList.drawElementCallback = DrawControlPointElement;
            controlPointsList.elementHeightCallback = ElementHeight;
        }

        private void OnDisable() {
            // Remove the sceneview SceneView.onSceneGUIDelegate -= OnSceneGUI;
            pointsList.drawHeaderCallback -= DrawPointHeader;
            pointsList.drawElementCallback -= DrawPointElement;
            pointsList.elementHeightCallback -= ElementHeight;

            controlPointsList.drawHeaderCallback -= DrawControlPointHeader;
            controlPointsList.drawElementCallback -= DrawControlPointElement;
            controlPointsList.elementHeightCallback -= ElementHeight;
        }

#region List Callbacks
        private void DrawPointHeader(Rect r) {
            EditorGUI.LabelField(r, new GUIContent("Points", "What are the main points that define the bezier curve?"));
        }

        private void DrawControlPointHeader(Rect r) {
            EditorGUI.LabelField(r, new GUIContent("Control Points", "What are the main control points that affect the bezier curve?"));
        }

        private void DrawPointElement(Rect r, int i, bool isActive, bool isFocused) {
            DrawVectorElement(points, r, i, isActive, isFocused);
        }

        private void DrawControlPointElement(Rect r, int i, bool isActive, bool isFocused) {
            DrawVectorElement(controlPoints, r, i, isActive, isFocused);
        }

        private void DrawVectorElement(SerializedProperty prop, Rect r, int i, bool isActive, bool isFocused) {
            var elem = prop.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(r, elem, GUIContent.none);
        }

        private float ElementHeight(int i) {
            return EditorGUIUtility.singleLineHeight;
        }
#endregion

#region Curve Rendering
        private void DrawHandlePoints(SerializedProperty property, Color handlesColor) {
            try {
                var size = property.arraySize;
                Handles.color = handlesColor;

                for(int i = 0; i < size; i++) {
                    var elem = property.GetArrayElementAtIndex(i);
                    var point = elem.vector3Value;
                    var snapSize = Vector3.one * HandleSize;
                    Handles.FreeMoveHandle(point, Quaternion.identity, HandleSize * 2, snapSize * 2, Handles.CircleHandleCap);
                    elem.vector3Value = Handles.FreeMoveHandle(point, Quaternion.identity, HandleSize, snapSize, Handles.DotHandleCap);
                }
            } catch (System.NullReferenceException) {}
        }
#endregion

        private void OnSceneGUI(SceneView sceneView) {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                DrawHandlePoints(points, Color.green);
                DrawHandlePoints(controlPoints, Color.cyan);
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                DrawDefaultInspector();
                pointsList.DoLayoutList();
                controlPointsList.DoLayoutList();

                serializedObject.Update();
                // TODO: Add the reorderable list functionality of updating values
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

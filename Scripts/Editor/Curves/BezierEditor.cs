using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(Bezier))] 
    public class BezierEditor : SceneViewEditor { 
        private const float HandleSize = 0.07f;

        private SerializedProperty points;
        private SerializedProperty controlPoints;

        private ReorderableList pointsList;
        private ReorderableList controlPointsList;
    
        protected override void OnEnable() {
            base.OnEnable();
            jsonPath = System.IO.Path.Combine(jsonDirectory, string.Format("{0}.json", (target as Bezier).name));

            points = serializedObject.FindProperty("points");
            controlPoints = serializedObject.FindProperty("controlPoints");

            pointsList = new ReorderableList(serializedObject, points);
            controlPointsList = new ReorderableList(serializedObject, controlPoints, true, true, false, false);

            pointsList.drawHeaderCallback = DrawPointHeader;
            pointsList.drawElementCallback = DrawPointElement;
            pointsList.elementHeightCallback = ElementHeight;
            pointsList.onAddCallback = AddPointListCallback;
            pointsList.onRemoveCallback = RemovePointsListCallback;
            pointsList.onCanRemoveCallback = CanRemovePointElement;

            controlPointsList.drawHeaderCallback = DrawControlPointHeader;
            controlPointsList.drawElementCallback = DrawControlPointElement;
            controlPointsList.elementHeightCallback = ElementHeight;
        }

        protected override void OnDisable() {
            base.OnDisable();

            pointsList.drawHeaderCallback -= DrawPointHeader;
            pointsList.drawElementCallback -= DrawPointElement;
            pointsList.elementHeightCallback -= ElementHeight; 
            pointsList.onAddCallback -= AddPointListCallback;
            pointsList.onRemoveCallback -= RemovePointsListCallback;
            pointsList.onCanRemoveCallback -= CanRemovePointElement;

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

        private void AddPointListCallback(ReorderableList list) {
            points.arraySize += 1;
            controlPoints.arraySize += 2;
        }

        private void RemovePointsListCallback(ReorderableList list) {
            points.arraySize -= 1;
            controlPoints.arraySize -= 2;
        }

        private bool CanRemovePointElement(ReorderableList list) {
            return points.arraySize > 2;
        }
#endregion

#region Curve Rendering
        private void DrawHandlePoints(SerializedProperty property, Color handlesColor) {
            try {
                var size = property.arraySize;
                Handles.color = handlesColor;

                for(int i = 0; i < size; i++) {
                    var elem = property.GetArrayElementAtIndex(i);
                    
                    var trs = transformData.TRS;

                    var point = trs.MultiplyPoint3x4(elem.vector3Value);
                    var snapSize = Vector3.one * HandleSize;
                    var position = Handles.FreeMoveHandle(point, Quaternion.identity, HandleSize * 2, snapSize * 2, Handles.CircleHandleCap);
                    position = Handles.FreeMoveHandle(point, Quaternion.identity, HandleSize, snapSize, Handles.DotHandleCap);

                    elem.vector3Value = trs.inverse.MultiplyPoint3x4(position);
                }
            } catch (System.NullReferenceException) {}
        }

        private void DrawCubicBezierCurve(Color bezierColor) {
            try {
                var size = points.arraySize;

                for (int i = 1; i < size; i++) {
                    var start = points.GetArrayElementAtIndex(i - 1);
                    var end = points.GetArrayElementAtIndex(i);

                    var controlStart = controlPoints.GetArrayElementAtIndex(i == 1 ? 0 : i);
                    var controlEnd = controlPoints.GetArrayElementAtIndex(i == 1 ? i : i + (i - 1));

                    var trs = transformData.TRS;

                    Handles.DrawBezier(
                            trs.MultiplyPoint3x4(start.vector3Value), 
                            trs.MultiplyPoint3x4(end.vector3Value), 
                            trs.MultiplyPoint3x4(controlStart.vector3Value), 
                            trs.MultiplyPoint3x4(controlEnd.vector3Value), 
                            bezierColor,
                            null, 
                            HandleUtility.GetHandleSize(Vector3.one) * 0.75f);
                }
            } catch (System.NullReferenceException) {}
        }
#endregion

        protected override void OnSceneGUI(SceneView sceneView) {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                LoadTransformData();
                serializedObject.Update();
                DrawHandlePoints(points, Color.green);
                DrawHandlePoints(controlPoints, Color.cyan);
                DrawCubicBezierCurve(Color.red);
                DrawTransformHandle();
                serializedObject.ApplyModifiedProperties();
                SaveTransformData();
            }
        }

        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                LoadTransformData();
                DrawDefaultInspector();
                DrawTransformField();

                pointsList.DoLayoutList();
                controlPointsList.DoLayoutList();

                serializedObject.ApplyModifiedProperties();
                SaveTransformData();
                if (changeCheck.changed) {
                    SceneView.RepaintAll();
                }
            }
        }
        
        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}

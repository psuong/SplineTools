using System.Collections.Generic;
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

        private Bezier bezier;
    
        protected override void OnEnable() {
            base.OnEnable();

            bezier = target as Bezier;
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
                    elem.vector3Value = trs.inverse.MultiplyPoint3x4(position);

                    position = Handles.FreeMoveHandle(position, Quaternion.identity, HandleSize, snapSize, Handles.DotHandleCap);
                    elem.vector3Value = trs.inverse.MultiplyPoint3x4(position);
                }
            } catch (System.NullReferenceException) {}
        }

        private void DrawCubicBezierCurve(Color bezierColor) {
            try {
                var bezierPoints = new List<Vector3>();
                var size = points.arraySize;

                for (int i = 1; i < size; i++) {
                    var pts = bezier.GetCubicBezierPoints(20);
                    bezierPoints.AddRange(pts);
                }

                var trs = transformData.TRS;

                Handles.color = Color.red;
                for (int i = 1; i < bezierPoints.Count; i += 2) {
                    var start = trs.MultiplyPoint3x4(bezierPoints[i - 1]);
                    var end = trs.MultiplyPoint3x4(bezierPoints[i]);

                    Handles.DrawLine(start, end);
                }
            } catch (System.NullReferenceException) {}
        }

        private void DrawPoints(Vector3[] points) {
            Handles.color = Color.green;
            foreach (var point in points) {
                var bPoint = transformData.TRS.MultiplyPoint3x4(point);
                Handles.DrawSolidDisc(bPoint, Vector3.up, 0.2f);
            }
        }

        private void SampleDirections(int segments) {
            var pts = bezier.points;
            var cPts = bezier.controlPoints;

            var directions = Bezier.GetTangentsNormalised(segments, pts, cPts);

            var trs = transformData.TRS;
            Handles.color = Color.green;
            foreach (var direction in directions) {
                var start = trs.MultiplyPoint3x4(direction.item1);
                var end = trs.MultiplyPoint3x4(direction.item1 + direction.item2);
                Handles.DrawLine(start, end);
            }
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
 
                SampleDirections(10);

                if (changeCheck.changed) {
                    serializedObject.ApplyModifiedProperties();
                    SaveTransformData();
                    SceneView.RepaintAll();
                }
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

                if (changeCheck.changed) {
                    SaveTransformData();
                    serializedObject.ApplyModifiedProperties();
                    SceneView.RepaintAll();
                }
            }
        }
        
        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}

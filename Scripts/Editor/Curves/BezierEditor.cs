using Curves.Utility;
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
                jsonPath = System.IO.Path.Combine(jsonDirectory, string.Format("{0}.json", target.name));

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
                var cSize = controlPoints.arraySize;
                var pSize = points.arraySize;

                var cPoint = controlPoints.GetArrayElementAtIndex(cSize - 1).vector3Value;
                var pPoint = points.GetArrayElementAtIndex(pSize - 1).vector3Value;

                points.arraySize++;
                controlPoints.arraySize += 2;

                var factor = 5;

                points.GetArrayElementAtIndex(pSize).vector3Value = pPoint + factor *  new Vector3(1, 0, 1);
                controlPoints.GetArrayElementAtIndex(cSize).vector3Value = cPoint + factor *  new Vector3(1, 0, 1);
                controlPoints.GetArrayElementAtIndex(cSize + 1).vector3Value = cPoint + 2 * factor * new Vector3(1, 0, 1);
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
            public static void DrawHandlePoints(Vector3[] points, Color handlesColor, Transform transform) {
                try {
                    var size = points.Length;
                    Handles.color = handlesColor;

                    for(int i = 0; i < size; i++) {
                        var elem = points[i];

                        var point = transform.InverseTransformPoint(elem);
                        var snapSize = Vector3.one * HandleSize;
                        var position = Handles.FreeMoveHandle(point, Quaternion.identity, HandleSize * 2, snapSize * 2, Handles.CircleHandleCap);
                        points[i] = transform.TransformPoint(position);

                        position = Handles.FreeMoveHandle(position, Quaternion.identity, HandleSize, snapSize, Handles.DotHandleCap);
                        points[i] = transform.InverseTransformPoint(position);
                    }
                } catch (System.NullReferenceException) {}                
            }

            public static void DrawHandlePoints(SerializedProperty property, Color handlesColor, TransformData transformData) {
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

            public static void DrawCubicBezierCurve(Color bezierColor, SerializedProperty points, SerializedProperty controlPoints, TransformData transformData) {
                try {
                    var size = points.arraySize;

                    var trs = transformData.TRS;

                    for (int i = 1; i < size; i++) {
                        var start = points.GetArrayElementAtIndex(i - 1).vector3Value;
                        var end = points.GetArrayElementAtIndex(i).vector3Value;
                        var index = i * 2;

                        var controlStart = controlPoints.GetArrayElementAtIndex(index - 2).vector3Value;
                        var controlEnd = controlPoints.GetArrayElementAtIndex(index - 1).vector3Value;

                        Handles.DrawBezier(
                                trs.MultiplyPoint3x4(start),
                                trs.MultiplyPoint3x4(end),
                                trs.MultiplyPoint3x4(controlStart),
                                trs.MultiplyPoint3x4(controlEnd),
                                bezierColor,
                                null,
                                HandleUtility.GetHandleSize(Vector3.one) * 0.5f);
                    }
                } catch (System.NullReferenceException) {}
            }

            public static void DrawCubicBezierCurve(Vector3[] points, Vector3[] controlPoints, Transform transform, Color color) {
                try {
                    var size = points.Length;

                    for (int i = 1; i < size; i++) {
                        var start = points[i - 1];
                        var end = points[i];

                        var index = i * 2;
                        var cStart = controlPoints[index - 2];
                        var cEnd = controlPoints[index - 1];

                        Handles.DrawBezier(
                                transform.TransformPoint(start),
                                transform.TransformPoint(end),
                                transform.TransformPoint(cStart),
                                transform.TransformPoint(cEnd),
                                color,
                                null,
                                HandleUtility.GetHandleSize(Vector3.one) * 0.5f);
                    }
                } catch (System.NullReferenceException) {}
            }

            public static void DrawPoints(Vector3[] points, TransformData transformData) {
                Handles.color = Color.green;
                foreach (var point in points) {
                    var bPoint = transformData.TRS.MultiplyPoint3x4(point);
                    Handles.DrawSolidDisc(bPoint, Vector3.up, 0.2f);
                }
            }

            public static void SampleDirections(int segments, Bezier bezier, TransformData transformData) {
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
                // Only draw the editor if the scriptable object was selected.
                if (Selection.Contains(bezier)) {
                    using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                        LoadTransformData();
                        serializedObject.Update();
                        BezierEditor.DrawHandlePoints(points, Color.green, transformData);
                        BezierEditor.DrawHandlePoints(controlPoints, Color.cyan, transformData);
                        BezierEditor.DrawCubicBezierCurve(Color.red, points, controlPoints, transformData);
                        DrawTransformHandle();

                        BezierEditor.SampleDirections(10, bezier, transformData);

                        if (changeCheck.changed) {
                            serializedObject.ApplyModifiedProperties();
                            SaveTransformData();
                            SceneView.RepaintAll();
                        }
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

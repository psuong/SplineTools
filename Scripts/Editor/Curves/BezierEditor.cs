using Curves.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(Bezier))]
    public class BezierEditor : SceneViewEditor {

        private SerializedProperty points;

        private ReorderableList pointsList;
        private Bezier bezier;
        private int currentIndex;

        protected override void OnEnable() {
            base.OnEnable();

            bezier = target as Bezier;
            jsonPath = System.IO.Path.Combine(jsonDirectory, string.Format("{0}.json", target.name));

            points = serializedObject.FindProperty("points");

            pointsList = new ReorderableList(serializedObject, points);

            pointsList.drawHeaderCallback = DrawPointHeader;
            pointsList.drawElementCallback = DrawPointElement;
            pointsList.elementHeightCallback = ElementHeight;
            pointsList.onAddCallback = AddPointListCallback;
            pointsList.onRemoveCallback = RemovePointsListCallback;
            pointsList.onCanRemoveCallback = CanRemovePointElement;
            pointsList.onSelectCallback = OnSelectPointsList;
        }

        protected override void OnDisable() {
            base.OnDisable();

            pointsList.drawHeaderCallback -= DrawPointHeader;
            pointsList.drawElementCallback -= DrawPointElement;
            pointsList.elementHeightCallback -= ElementHeight;
            pointsList.onAddCallback -= AddPointListCallback;
            pointsList.onRemoveCallback -= RemovePointsListCallback;
            pointsList.onCanRemoveCallback -= CanRemovePointElement;
            pointsList.onSelectCallback -= OnSelectPointsList;
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

        private void DrawVectorElement(SerializedProperty prop, Rect r, int i, bool isActive, bool isFocused) {
            var elem = prop.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(r, elem, GUIContent.none);
        }

        private float ElementHeight(int i) {
            return EditorGUIUtility.singleLineHeight;
        }

        private void AddPointListCallback(ReorderableList list) {
            var size = points.arraySize += 3;
            
            var p0 = points.GetArrayElementAtIndex(size - 4).vector3Value;
            points.GetArrayElementAtIndex(size - 3).vector3Value = p0 + new Vector3(-2.5f, 0, 2.5f);
            points.GetArrayElementAtIndex(size - 2).vector3Value = p0 + new Vector3(2.5f, 0, 7.5f);
            points.GetArrayElementAtIndex(size - 1).vector3Value = p0 + (Vector3.forward * 15f);
        }

        private void RemovePointsListCallback(ReorderableList list) {
            if (currentIndex > 0 && currentIndex % 3 == 0) {
                points.DeleteArrayElementAtIndex(currentIndex);
                points.DeleteArrayElementAtIndex(currentIndex - 1);
                points.DeleteArrayElementAtIndex(currentIndex - 2);
            }
        }

        private bool CanRemovePointElement(ReorderableList list) {
            return points.arraySize > 4;
        }

        private void OnSelectPointsList(ReorderableList list) {
            currentIndex = list.index;
        }
#endregion

#region Curve Rendering
        /// <summary>
        /// Draws a cubic bezier curve relative to a position.
        /// </summary>
        /// <param name="bezierColor">The colour of the bezier curve.</param>
        /// <param name="points">The serialized property containing the array of points.</param>
        /// <param name="transformData">The transform data holding the position, rotation, and scale that affects the bezier curve.</param>
        public static void DrawCubicBezierCurve(Color bezierColor, SerializedProperty points, TransformData transformData) {
            try {
                var size = points.arraySize;

                var trs = transformData.TRS;

                for(int i = 0; i < size - 1; i += 3) {
                    var p0 = points.GetArrayElementAtIndex(i).vector3Value;
                    var c0 = points.GetArrayElementAtIndex(i + 1).vector3Value;
                    var c1 = points.GetArrayElementAtIndex(i + 2).vector3Value;
                    var p1 = points.GetArrayElementAtIndex(i + 3).vector3Value;

                    Handles.DrawBezier(
                        trs.MultiplyPoint3x4(p0),
                        trs.MultiplyPoint3x4(p1),
                        trs.MultiplyPoint3x4(c0),
                        trs.MultiplyPoint3x4(c1),
                        bezierColor,
                        null,
                        HandleUtility.GetHandleSize(Vector3.one));
                }
            } catch(System.NullReferenceException) { }
        }

        /// <summary>
        /// Draws a cubic bezier curve relative to a transform.
        /// </summary>
        /// <param name="points">The bezier reference points.</param>
        /// <param name="transform">The transform that affects the bezier's position.</param>
        /// <param name="colour">The colour of the bezier to draw.</param>
        public static void DrawCubicBezierCurve(Vector3[] points, Transform transform, Color colour) {
            try {
                var size = points.Length;

                for(int i = 0; i < size - 1; i += 3) {
                    var p0 = points[i];
                    var c0 = points[i + 1];
                    var c1 = points[i + 2];
                    var p1 = points[i + 3];

                    Handles.DrawBezier(
                        transform.TransformPoint(p0),
                        transform.TransformPoint(p1),
                        transform.TransformPoint(c0),
                        transform.TransformPoint(c1),
                        colour,
                        null,
                        HandleUtility.GetHandleSize(Vector3.one) * 0.5f);
                }
            } catch(System.NullReferenceException) { }
        }

        /// <summary>
        /// Draws the tangents of a bezier curve.
        /// </summary>
        /// <param name="segments">How many segments should there be in the bezier curve?</param>
        /// <param name="bezier">The bezier to draw.</param>
        /// <param name="transformData">The transform that affects the bezier curve's position.</param>
        public static void SampleDirections(int segments, Bezier bezier, TransformData transformData) {
            var directions = Bezier.GetTangentsNormalised(bezier.points, segments);
            var trs = transformData.TRS;

            Handles.color = Color.green;

            foreach (var direction in directions) {
                var lhs = trs.MultiplyPoint3x4(direction.item1);
                var rhs = trs.MultiplyPoint3x4(direction.item1 + direction.item2);

                Handles.DrawLine(lhs, rhs);
            }
        }
#endregion
        protected override void OnSceneGUI(SceneView sceneView) {
            // Only draw the editor if the scriptable object was selected.
            if(Selection.Contains(bezier)) {
                using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                    LoadTransformData();
                    serializedObject.Update();
                    VectorHandles.DrawHandlePoints(points, Color.green, transformData);
                    DrawTransformHandle();
                    
                    BezierEditor.DrawCubicBezierCurve(Color.red, points, transformData);
                    BezierEditor.SampleDirections(10, bezier, transformData);

                    if(changeCheck.changed) {
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

                if(changeCheck.changed) {
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

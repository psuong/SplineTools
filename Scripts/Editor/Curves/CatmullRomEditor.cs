using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Curves.EditorTools {

    [CustomEditor(typeof(CatmullRom))]
    public class CatmullRomEditor : SceneViewEditor {

        private SerializedProperty points;
        private SerializedProperty loopField;

        private ReorderableList pointsList;
        private CatmullRom catmullRom;

        protected override void OnEnable() {
            base.OnEnable();
            ShowTransformField = true;
            catmullRom = target as CatmullRom;
            jsonPath = System.IO.Path.Combine(jsonDirectory, string.Format("{0}.json", target.name));

            points = serializedObject.FindProperty("points");
            loopField = serializedObject.FindProperty("isLooping");
            pointsList = new ReorderableList(serializedObject, points);

            pointsList.drawHeaderCallback = DrawPointsHeader;
            pointsList.drawElementCallback = DrawPointElement;
            pointsList.elementHeightCallback = ElementHeight;
        }

        protected override void OnDisable() { pointsList.drawHeaderCallback -= DrawPointsHeader;
            pointsList.drawElementCallback -= DrawPointElement;
            pointsList.elementHeightCallback -= ElementHeight;
        }

#region List Callbacks
        private void DrawPointsHeader(Rect r) {
            EditorGUI.LabelField(r, "Points");
        }

        private void DrawPointElement(Rect r, int i, bool isActive, bool isFocused) {
            var elem = points.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(r, elem, GUIContent.none);
        }

        private float ElementHeight(int i) {
            return EditorGUIUtility.singleLineHeight;
        }

        private void DrawIsLoopingField() {
            EditorGUILayout.PropertyField(loopField);
        }
#endregion

#region Curve Rendering
        private void DrawCatmullRomSpline(Color colour) {
            var pts = catmullRom.SampleCatmullRomSpline(10);
            VectorHandles.DrawLines(pts, transformData, Color.red);
        }

        public static void DrawCatmullRomSpline(CatmullRom catmullRom, Curves.Utility.TransformData transformData, Color colour) {
            var values = new List<Vector3>();
            CatmullRom.SampleCatmullRomSpline(catmullRom.points, catmullRom.isLooping, 20, (values as IList<Vector3>));
            VectorHandles.DrawLines(values.ToArray(), transformData, Color.red);
        }

        public static void DrawCatmullRomSpline(CatmullRom catmullRom, Transform transform, Color colour) {
            var values = new List<Vector3>();
            CatmullRom.SampleCatmullRomSpline(catmullRom.points, catmullRom.isLooping, 20, (values as IList<Vector3>));
            VectorHandles.DrawLines(values.ToArray(), transform, colour);
        }
#endregion

        protected override void OnSceneGUI(SceneView sceneView) {
            if (Selection.Contains(catmullRom)) {
                using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                    LoadTransformData();
                    serializedObject.Update();
                    VectorHandles.DrawHandlePoints(points, Color.green, transformData);
                    DrawTransformHandle();

                    DrawCatmullRomSpline(Color.red);

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

                DrawIsLoopingField();
                pointsList.DoLayoutList();
                VectorListUtility.ResetHeight(points);

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

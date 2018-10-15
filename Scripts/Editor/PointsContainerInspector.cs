using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace SplineTools.EditorTools {

    [CustomEditor(typeof(PointsContainer), true)]
    public class PointsContainerInspector : Editor {

        private SerializedProperty samplesProperty;
        private ReorderableList samplesList;
        private PointsContainer container;

        private void OnEnable() {
            container = target as PointsContainer;
            samplesProperty = serializedObject.FindProperty("points");
            samplesList = new ReorderableList(serializedObject, samplesProperty, true, true, true, true);

            samplesList.drawHeaderCallback += (Rect r) => { EditorGUI.LabelField(r, "Points"); };
            samplesList.drawElementCallback += DrawVectorElement;
        }

        private void OnDisable() {
            samplesList.drawElementCallback -= DrawVectorElement;
        }

        private void DrawVectorElement(Rect r, int i, bool isActive, bool isFocused) {
            var element = samplesProperty.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(r, element, GUIContent.none);
        }

        private void ProcessElements(Action<SerializedProperty> action) {
            for (int i = 0; i < samplesProperty.arraySize; i++) {
                var element = samplesProperty.GetArrayElementAtIndex(i);

                action?.Invoke(element);
            }
        }

        private void DisplayGeneratedPoints() {
            if (container.lineSegments > 0) {
                Handles.color = container.lineColor;
                CatmullRomSpline.SampleCatmullRomSpline(ref container.points, container.lineSegments, container.loop, out Vector3[] points);

                CatmullRomSpline.SampleCatmullRomBinormals(ref container.points, container.lineSegments, container.loop, out Vector3[] tangents);
                for (int i = 1; i < points.Length; i++) {
                    Handles.DrawDottedLine(points[i - 1], points[i], container.lineSpacing);
                }

                for (int i = 0; i < points.Length; i++) {
                    // Handles.DrawLine(points[i], points[i] + tangents[i].normalized);
                    Handles.color = container.normalColor;
                    Handles.DrawLine(points[i], tangents[i] + points[i]);
                }
            }
        }

        private void DisplayHeightClampButton() {
            if (GUILayout.Button(new GUIContent("Clamp Height"))) {
                ProcessElements((element) => {
                    var v = element.vector3Value;
                    element.vector3Value = new Vector3(v.x, container.heightClamp, v.z);
                });
            }
        }

        private void OnSceneGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                Handles.color = container.handlesColor;
                serializedObject.Update();

                ProcessElements((element) => {
                    element.vector3Value = Handles.FreeMoveHandle(element.vector3Value, Quaternion.identity, container.handlesSize,
                        Vector3.one * container.handlesSize, Handles.DotHandleCap);
                });

                ProcessElements((element) => {
                    var v = element.vector3Value;
                    element.vector3Value = new Vector3(Mathf.Round(v.x * 100) / 100, Mathf.Round(v.y * 100) / 100, Mathf.Round(v.z * 100) / 100);
                });

                DisplayGeneratedPoints();

                if (changeCheck.changed) {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();

                DrawDefaultInspector();
                samplesList.DoLayoutList();
                DisplayHeightClampButton();

                if (changeCheck.changed) {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}

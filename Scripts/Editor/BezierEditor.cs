﻿using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Curves.EditorTools {
    [CustomEditor(typeof(Bezier))]
    public class BezierEditor : Editor {

        private const float HandleSize = 0.15f;

        private SerializedProperty points;
        private SerializedProperty controlPoints;
        private ReorderableList pointsList;
        private ReorderableList ctrlPtsList;
        private Transform transform;

        private void OnEnable() {
            points = serializedObject.FindProperty("points");
            transform = (target as Bezier).transform;
            controlPoints = serializedObject.FindProperty("controlPoints");

            pointsList = new ReorderableList(serializedObject, points);
            ctrlPtsList = new ReorderableList(serializedObject, controlPoints, true, true, false, false);

            pointsList.drawHeaderCallback = DrawPointHeader;
            pointsList.drawElementCallback = DrawPointElem;
            pointsList.onAddCallback = (ReorderableList list) => {
                controlPoints.arraySize += 2;
                points.arraySize += 1;
            };

            pointsList.onRemoveCallback = (ReorderableList list) => {
                if (points.arraySize > 2) {
                    controlPoints.arraySize -= 2;
                    points.arraySize -= 1;
                }
            };

            ctrlPtsList.drawHeaderCallback = DrawControlPointHeader;
            ctrlPtsList.drawElementCallback = DrawControlPointElem;
        }

        private void OnDisable() {
            pointsList.drawHeaderCallback -= DrawPointHeader;
            pointsList.drawElementCallback -= DrawPointElem;

            ctrlPtsList.drawHeaderCallback -= DrawControlPointHeader;
            ctrlPtsList.drawElementCallback -= DrawControlPointElem;
        }

        public override void OnInspectorGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                DrawDefaultInspector();
                pointsList.DoLayoutList();
                ctrlPtsList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnSceneGUI() {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                serializedObject.Update();
                DrawBezierCurve();
                DrawPoints();
                DrawControlPoints();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawBezierCurve() {
            var size = points.arraySize;
            for (int i = 1; i < size; i++) {
                var start = points.GetArrayElementAtIndex(i - 1);
                var end = points.GetArrayElementAtIndex(i);

                var controlStart = controlPoints.GetArrayElementAtIndex(i == 1 ? 0 : i);
                var controlEnd = controlPoints.GetArrayElementAtIndex(i == 1 ? i : i + (i - 1));

                var ptStart = transform.TransformPoint(start.vector3Value);
                var ptEnd = transform.TransformPoint(end.vector3Value);

                var ctrlStart = transform.TransformPoint(controlStart.vector3Value);
                var ctrlEnd = transform.TransformPoint(controlEnd.vector3Value);

                Handles.DrawBezier(ptStart, ptEnd, ctrlStart, ctrlEnd, Color.red, null, HandleUtility.GetHandleSize(Vector3.zero) * 0.5f);
            }
        }

        private void DrawPoints() {
            Handles.color = Color.cyan;
            var size = points.arraySize;
            for (int i = 0; i < size; i++) {
                var element = points.GetArrayElementAtIndex(i);
                var snapSize = Vector3.one * HandleSize;
                element.vector3Value = Handles.FreeMoveHandle(element.vector3Value, Quaternion.identity, HandleSize, snapSize, Handles.DotHandleCap);
            }
        }

        private void DrawControlPoints() {
            Handles.color = Color.green;
            var size = controlPoints.arraySize;
            for (int i = 0; i < size; i++) {
                var element = controlPoints.GetArrayElementAtIndex(i);
                var snapSize = Vector3.one * HandleSize;
                element.vector3Value = Handles.FreeMoveHandle(element.vector3Value, Quaternion.identity, HandleSize, snapSize, Handles.DotHandleCap);
            }
        }

#region ReorderableList
        private void DrawPointHeader(Rect r) {
            EditorGUI.LabelField(r, "Points", EditorStyles.boldLabel);
        }

        private void DrawControlPointHeader(Rect r) {
            EditorGUI.LabelField(r, "Control Points", EditorStyles.boldLabel);
        }

        private void DrawControlPointElem(Rect r, int i, bool isActive, bool isFocused) {
            DrawVectorElement(controlPoints, r, i, isActive, isFocused);
        }

        private void DrawPointElem(Rect r, int i, bool isActive, bool isFocused) {
            DrawVectorElement(points, r, i, isActive, isFocused);
        }

        private void DrawVectorElement(SerializedProperty prop, Rect r, int i, bool isActive, bool isFocused) {
            var elem = prop.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(r, elem, GUIContent.none);
        }
#endregion
    }
}
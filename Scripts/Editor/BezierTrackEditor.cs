using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Curves.EditorTools {

    // TODO: Allow conversion between local and world space
    // TODO: Add control point + point size control
    [CustomEditor(typeof(BezierTrack))]
    public class BezierTrackEditor : Editor {

        private const float HandleSize = 0.07f;

        private SerializedProperty points;
        private SerializedProperty controlPoints;

        private ReorderableList pointsList;
        private ReorderableList controlPointsList;

        private TransformData transformData;
        private string jsonDirectory;
        private string jsonPath;
    
        private void OnEnable() {
            jsonDirectory = System.IO.Path.Combine(Application.dataPath, "Scripts", "Editor", "Configurations");
            jsonPath = System.IO.Path.Combine(jsonDirectory, string.Format("{0}.json", (target as BezierTrack).name));

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
                    
                    var trs = Matrix4x4.TRS(transformData.position, Quaternion.Euler(transformData.rotation), transformData.scale);

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

                    var trs = Matrix4x4.TRS(transformData.position, Quaternion.Euler(transformData.rotation), transformData.scale);

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

#region Transform
        private void DrawTransformField() {
            EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);
            transformData.position = EditorGUILayout.Vector3Field(new GUIContent("Position"), transformData.position);
            transformData.rotation = EditorGUILayout.Vector3Field(new GUIContent("Rotation"), transformData.rotation);
            transformData.scale = EditorGUILayout.Vector3Field(new GUIContent("Scale"), transformData.scale);
            transformData.showTransformData = EditorGUILayout.Toggle("Show Transform", transformData.showTransformData);
        }

        private void DrawTransformHandle() {
            if (transformData.showTransformData) {
                transformData.position = Handles.PositionHandle(transformData.position, Quaternion.Euler(transformData.rotation));
                transformData.rotation = Handles.RotationHandle(Quaternion.Euler(transformData.rotation), transformData.position).eulerAngles;
                transformData.scale = Handles.ScaleHandle(transformData.scale, transformData.position, Quaternion.Euler(transformData.rotation), transformData.scale.magnitude);
            }
        }

        private void LoadTransformData() {
            if (System.IO.File.Exists(jsonPath)) {
                var jsonRep = System.IO.File.ReadAllText(jsonPath);
                transformData = JsonUtility.FromJson<TransformData>(jsonRep);
            } else {
                transformData = TransformData.CreateTransformData();
                System.IO.Directory.CreateDirectory(jsonDirectory);
                System.IO.File.WriteAllText(jsonPath, JsonUtility.ToJson(transformData));
            }
        }

        private void SaveTransformData() {
            var json = JsonUtility.ToJson(transformData);
            System.IO.File.WriteAllText(jsonPath, json);
        }
#endregion

        private void OnSceneGUI(SceneView sceneView) {
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
            }
        }
        
        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}

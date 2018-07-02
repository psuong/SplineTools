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
           Handles.color = colour;
           for (int i = 1; i < pts.Length; i++) {
               var lhs = pts[i - 1];
               var rhs = pts[i];

               Handles.DrawLine(lhs, rhs);
           }
        }
#endregion

        protected override void OnSceneGUI(SceneView sceneView) {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                LoadTransformData();
                serializedObject.Update();
                VectorHandles.DrawHandlePoints(points, Color.green, transformData);
                DrawTransformHandle();

                DrawCatmullRomSpline(Color.red);

                if (changeCheck.changed) {
                    serializedObject.ApplyModifiedProperties();
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
                
                DrawIsLoopingField();
                pointsList.DoLayoutList();

                if (changeCheck.changed) {
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

using Curves.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Curves.EditorTools {
    
    [CustomEditor(typeof(CatmullRom))]
    public class CatmullRomEditor : SceneViewEditor {

        private SerializedProperty points;

        private ReorderableList pointsList;
        private CatmullRom catmullRom;

        protected override void OnEnable() {
            base.OnEnable();
            jsonPath = System.IO.Path.Combine(jsonDirectory, string.Format("{0}.json", target.name));

            points = serializedObject.FindProperty("points");
            pointsList = new ReorderableList(serializedObject, points);

        }

        protected override void OnDisable() {
            
        }

#region List Callbacks
        private void DrawPointsHeader() {
        }
#endregion

        protected override void OnSceneGUI(SceneView sceneView) {
            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                LoadTransformData();
                serializedObject.Update();
                DrawTransformHandle();

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

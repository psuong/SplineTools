using Curves.Utility;
using UnityEditor;
using UnityEngine;

namespace Curves.EditorTools { 

    public static class VectorHandles {

        /// <summary>
        /// Draws moveable points within the Scene View to edit.
        /// </summary>
        /// <param name="points">A series of points to draw.</param>
        /// <param name="handlesColour">The colour of the points.</param>
        /// <param name= "transform">The relative position/rotation/scale that affect the points' position.</param>
        /// <param name="handleSize">An optional size that defines how big each handle is in the scene.</param>
        public static void DrawHandlePoints(Vector3[] points, Color handlesColour, Transform transform, float handleSize = 0.07f) {
            try {
                var size = points.Length;
                Handles.color = handlesColour;

                for(int i = 0; i < size; i++) {
                    var elem = points[i];

                    var point = transform.TransformPoint(elem);
                    var snapSize = Vector3.one * handleSize;
                    var position = Handles.FreeMoveHandle(point, Quaternion.identity, handleSize * 2, snapSize * 2, Handles.CircleHandleCap);
                    points[i] = transform.InverseTransformPoint(position);

                    position = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, snapSize, Handles.DotHandleCap);
                    points[i] = transform.InverseTransformPoint(position);
                }
            } catch(System.NullReferenceException) { }
        }

        /// <summary>
        /// Draws moveable points within the Scene View to edit.
        /// </summary>
        /// <param name="property">The array/list property to reference to.</param>
        /// <param name="handlesColour">The colour of the handles.</param>
        /// <param name= "transformData">The position/rotation/scale that affects the points within the property's position.</param>
        /// <param name="handleSize">An optional size that defines how big each handle is in the scene.</param>
        public static void DrawHandlePoints(SerializedProperty property, Color handlesColour, TransformData transformData, float handleSize = 0.07f) {
            try {
                var size = property.arraySize;
                Handles.color = handlesColour;

                for(int i = 0; i < size; i++) {
                    var elem = property.GetArrayElementAtIndex(i);

                    var trs = transformData.TRS;

                    var point = trs.MultiplyPoint3x4(elem.vector3Value);
                    var snapSize = Vector3.one * handleSize;
                    var position = Handles.FreeMoveHandle(point, Quaternion.identity, handleSize * 2, snapSize * 2, Handles.CircleHandleCap);
                    elem.vector3Value = trs.inverse.MultiplyPoint3x4(position);

                    position = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, snapSize, Handles.DotHandleCap);
                    elem.vector3Value = trs.inverse.MultiplyPoint3x4(position);
                }
            } catch(System.NullReferenceException) { }
        }

        /// <summary>
        /// Draws each point within an array relative to a transform.
        /// </summary>
        /// <param name="points">The series of Vector3s to draw.</param>
        /// <param name="transformData">The transform that affects the bezier curve's position.</param>
        public static void DrawPoints(Vector3[] points, TransformData transformData) {
            Handles.color = Color.green;
            foreach (var point in points) {
                var bPoint = transformData.TRS.MultiplyPoint3x4(point);
                Handles.DrawSolidDisc(bPoint, Vector3.up, 0.2f);
            }
        }
    }
}

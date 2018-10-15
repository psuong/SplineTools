using UnityEngine;

namespace SplineTools {

    /// <summary>
    /// Stores points along a path for reference.
    /// </summary>
    public class PointsContainer : MonoBehaviour {

        [Tooltip("What is the spline type to generate for previewing in the Editor?")]
        public SplineType type;
        [Tooltip("Should the curve loop?")]
        public bool loop;
        [Tooltip("How many linesegments are there in the CatmullRomSpline?")]
        public int lineSegments;
        [HideInInspector]
        public Vector3[] points = { Vector3.back, Vector3.left, Vector3.forward, Vector3.right };

#if UNITY_EDITOR
        [Header("Editor Options")]
        [Tooltip("What height should the value be at?")]
        public float heightClamp;
        [Range(0.1f, 1f), Tooltip("How far should the lines be?")]
        public float lineSpacing = 0.5f;
        [Tooltip("What color should the clickable handles be?")]
        public Color handlesColor = Color.green;
        [Tooltip("What color should the lines be?")]
        public Color lineColor = Color.red;
#endif
    }
}

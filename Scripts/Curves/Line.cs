using UnityEngine;

namespace Curves {

    /// <summary>
    /// Represents a line to render in the scene. No need to write a function to grab a point
    /// between two points, because that is just a Vector3.Lerp.
    /// </summary>
    [CreateAssetMenu(menuName = "Curves/Line", fileName = "Line")]
    public class Line : ScriptableObject {

        public Vector3 p0, p1;
    }
}

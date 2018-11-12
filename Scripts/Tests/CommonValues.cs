using UnityEngine;

namespace SplineTools.Tests {
    
    /// <summary>
    /// Stores common values which tests can use.
    /// </summary>
    public static class CommonValues {
        
        /// <summary>
        /// A set of points which create a "diamond" shape.
        /// </summary>
        public static readonly Vector3[] Points = { Vector3.back, Vector3.left, Vector3.forward, Vector3.right };
    }
}
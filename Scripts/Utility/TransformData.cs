using UnityEngine;

namespace Curves {
    
    [System.Serializable]
    public struct TransformData {
        
        /// <summary>
        ///  Returns the matrix representation of the transform.
        /// </summary>
        public Matrix4x4 TRS {
            get {
                return Matrix4x4.TRS(
                    position,
                    Quaternion.Euler(rotation),
                    scale
                );
            }
        }

        // Store the position, rotation, and scale to create a fake Transform component
        public Vector3 position, rotation, scale;
        public bool showTransformData;
        
        /// <summary>
        /// Creates the default transform data representation.
        /// </summary>
        /// <returns>The default TransformData json representation</returns>
        public static TransformData CreateTransformData() {
            return new TransformData {
                position = Vector3.zero,
                rotation = Vector3.zero,
                scale = Vector3.one,
                showTransformData = false
            };
        }
        
        /// <summary>
        /// Create a transform data reprsentation with custom parameters.
        /// </summary>
        /// <param name="position">The position in world space.</param>
        /// <param name="rotation">The rotation in euler angles.</param>
        /// <param name="scale">The scale of the transform.</param>
        /// <returns>The TransformData json representation</returns>
        public static TransformData CreateTransformData(Vector3 position, Vector3 rotation, Vector3 scale) {
            return new TransformData {
                position = position,
                rotation = rotation,
                scale = scale,
                showTransformData = false
            };
        }
    }
}

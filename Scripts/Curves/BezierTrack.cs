using UnityEngine;

namespace Curves {
    
    [CreateAssetMenu(menuName = "Curves/Bezier", fileName = "Bezier Curve")]
    public class BezierTrack : ScriptableObject {

        [SerializeField]
        private Vector3[] points = new Vector3[] { Vector3.zero, Vector3.forward };
        [SerializeField]
        private Vector3[] controlPoints = new Vector3[] { new Vector3(-0.25f, 0f, 0.25f), new Vector3(0.25f, 0f, 0.75f) };
    }
}

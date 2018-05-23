using UnityEngine;

namespace Curves {

    // TODO: Add looping functionality of Bezier curves
    
    [CreateAssetMenu(menuName = "Curves/Bezier", fileName = "Bezier Curve")]
    public class BezierTrack : ScriptableObject {

        [SerializeField]
        private Vector3[] points = { Vector3.zero, Vector3.forward * 10f };
        [SerializeField]
        private Vector3[] controlPoints = { new Vector3(-2.5f, 0f, 2.5f), new Vector3(2.5f, 0f, 7.5f) };
    }
}

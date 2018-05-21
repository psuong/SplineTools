using UnityEngine;

namespace Curves {
    
    public class Bezier : MonoBehaviour {
        
        [SerializeField]
        private Vector3[] points = new Vector3[] { Vector3.zero, Vector3.forward * 10f };
        [SerializeField]
        private Vector3[] controlPoints;
    }
}

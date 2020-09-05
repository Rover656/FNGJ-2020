using System;
using UnityEngine;

namespace DefaultNamespace {
    public class WarpPoint : MonoBehaviour {
        public WarpPoint otherSide;
        public Vector3 outputOffset;

        public void PerformTP(Transform enteredThing) {
            var distanceFromCenter = enteredThing.position - otherSide.transform.position;
            enteredThing.position = transform.position + distanceFromCenter + outputOffset * 1.5f;
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            otherSide.PerformTP(other.transform);
        }
    }
}
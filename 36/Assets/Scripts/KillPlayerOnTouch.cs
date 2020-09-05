using System;
using UnityEngine;

namespace DefaultNamespace {
    public class KillPlayerOnTouch : MonoBehaviour {
        public bool ignore;
        private void OnTriggerEnter2D(Collider2D other) {
            if (ignore) return;
            if (other.CompareTag("Player") || other.CompareTag("Enemy")) {
                PlayerController.Instance.Die();
            }
        }
    }
}
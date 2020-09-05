using System;
using UnityEngine;

namespace DefaultNamespace {
    public class PointPellet : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player")) {
                GameManager.Instance.GainPoint();
                Destroy(gameObject);
            }
        }
    }
}
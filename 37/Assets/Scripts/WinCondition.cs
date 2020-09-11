using System;
using UnityEngine;

namespace DefaultNamespace {
    public class WinCondition : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            GameController.instance.Win();
        }
    }
}
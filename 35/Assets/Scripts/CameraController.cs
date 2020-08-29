using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace {
    public class CameraController : MonoBehaviour {
        public float speed;
        private Vector2 _movementVector;

        private Vector3 currentVelocity;

        private void Update() {
            Vector3 destination = _movementVector * speed;
            transform.position += destination * Time.deltaTime;
        }
        
        public void Move(InputAction.CallbackContext ctx) {
            _movementVector = ctx.ReadValue<Vector2>();
        }
    }
}
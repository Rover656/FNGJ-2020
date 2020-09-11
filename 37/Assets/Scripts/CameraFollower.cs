using UnityEngine;

namespace DefaultNamespace {
    public class CameraFollower : MonoBehaviour {
        public Transform player;
        public float smoothTime;
        public float maxSpeed;

        private Vector3 _velocity;

        private void Start() {
            var target = player.position;
            target.z = transform.position.z;
            transform.position = target;
        }
        
        private void FixedUpdate() {
            var target = player.position;
            target.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, smoothTime, maxSpeed);
        }
    }
}
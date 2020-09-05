using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace {
    public class PlayerController : MonoBehaviour {
        public GameObject tailPrefab;
        
        private List<GameObject> tails;

        public Vector2 direction;

        private const float spacing = 1.15f;

        private float _motionTimer;

        public static PlayerController Instance;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            tails = new List<GameObject>();
            direction = new Vector2(1, 0);
            transform.localRotation = Quaternion.Euler(GetRotation(direction));

            for (var i = 0; i < GameManager.Instance.length - 1; i++) {
                var tail = Instantiate(tailPrefab, transform.position, Quaternion.identity);
                if (i == 0) tail.GetComponent<KillPlayerOnTouch>().ignore = true;
                tails.Add(tail);
            }
        }

        private Vector3 GetRotation(Vector2 dir) {
            if (dir.y == 1) {
                return new Vector3(0, 0, 0);
            } else if (dir.y == -1) {
                return new Vector3(0, 0, 180);
            } else if (dir.x == 1) {
                return new Vector3(0, 0, 270);
            } else if (dir.x == -1) {
                return new Vector3(0, 0, 90);
            }

            return Vector3.zero;
        }

        private void Update() {
            _motionTimer += Time.deltaTime;
            {
                Vector2 dir = Vector3.zero;
                var mousePos = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
                if (mousePos.y > 0.7f) {
                    dir = new Vector2(0, 1);
                } else if (mousePos.y < 0.3f) {
                    dir = new Vector2(0, -1);
                } else if (mousePos.x > 0.7f) {
                    dir = new Vector2(1, 0);
                } else if (mousePos.x < 0.3f) {
                    dir = new Vector2(-1, 0);
                }
                GameManager.Instance.directionalIndicator.rectTransform.localRotation = Quaternion.Euler(GetRotation(dir));
            }

            if (_motionTimer >= 0.6f) {
                // Save
                var lastPosition = transform.position;
                var lastRotation = transform.localRotation;
                
                // Move head
                var newPos = transform.position;
                newPos = newPos + new Vector3(direction.x, direction.y) * 1.45f;
                transform.position = newPos;
                
                // lastPosition = lastPosition - new Vector3(direction.x, direction.y) * (1-spacing+0.05f);
                
                // Reset timer
                _motionTimer = 0;
                
                // Rotate head
                transform.localRotation = Quaternion.Euler(GetRotation(direction));
                
                // Process tails
                for (var i = 0; i < tails.Count; i++) {
                    var tmp = lastPosition;
                    lastPosition = tails[i].transform.position;
                    tails[i].transform.position = tmp;

                    var tmpR = lastRotation;
                    lastRotation = tails[i].transform.localRotation;
                    tails[i].transform.localRotation = tmpR;
                }
            }
        }

        public void Grow() {
            // Get last tail position and add to it
            if (tails.Count > 0) {
                var tail = tails[tails.Count - 1];
                Vector3 lastPos = tail.transform.position;
                var newPos = new Vector2(lastPos.x, lastPos.y) - direction;
                var newTail = Instantiate(tailPrefab, newPos, Quaternion.identity);
                tails.Add(newTail);
                newTail.transform.localRotation = tail.transform.localRotation;
            } else {
                var newPos = transform.position - new Vector3(direction.x, direction.y); 
                var newTail = Instantiate(tailPrefab, newPos, Quaternion.identity);
                tails.Add(newTail);
                newTail.transform.localRotation = transform.localRotation;
                newTail.GetComponent<KillPlayerOnTouch>().ignore = true;
            }
        }

        public void ConfirmMove(InputAction.CallbackContext ctx) {
            if (ctx.performed) {
                // Get the region the mouse is in
                var mousePos = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
                if (mousePos.y > 0.7f) {
                    direction = new Vector2(0, 1);
                } else if (mousePos.y < 0.3f) {
                    direction = new Vector2(0, -1);
                } else if (mousePos.x > 0.7f) {
                    direction = new Vector2(1, 0);
                } else if (mousePos.x < 0.3f) {
                    direction = new Vector2(-1, 0);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("PlayerImpassible") || other.gameObject.CompareTag("Enemy")) {
                Die();
            }
        }

        private bool _dead;
        
        public void Die() {
            if (_dead) return;
            _dead = true;
            foreach (var tail in tails)
                Destroy(tail);
            Destroy(gameObject);
            GameManager.Instance.RespawnOrEnd();
        }
    }
}
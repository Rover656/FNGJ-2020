using Pathfinding;
using UnityEngine;

namespace DefaultNamespace {
    public enum EnemyTarget {
        Head,
        Tail,
        AheadOfHead
    }
    
    public class EnemyAI : MonoBehaviour {
        public Pathfinder pathfinder;
        public EnemyTarget target;

        public Vector3 initialLocation;

        private float _moveTimer;

        public float timeout;

        private float _initialTimeout;

        private void Start() {
            initialLocation = transform.position;
            _initialTimeout = timeout;
        }

        public void Reset() {
            timeout = _initialTimeout;
            transform.position = initialLocation;
        }

        private void Update() {
            if (timeout > 0) {
                timeout -= Time.deltaTime;
                return;
            }
            
            _moveTimer += Time.deltaTime;

            if (_moveTimer > 1f) {
                _moveTimer = 0;
                Vector3Int playerGridPos = Vector3Int.zero;

                var controller = PlayerController.Instance;
                switch (target) {
                    case EnemyTarget.Head:
                        playerGridPos = NodeGrid.Instance.grid.WorldToCell(controller.transform.position);
                        break;
                    case EnemyTarget.Tail:
                        playerGridPos = NodeGrid.Instance.grid.WorldToCell(controller.transform.position);
                        break;
                    case EnemyTarget.AheadOfHead:
                        playerGridPos = NodeGrid.Instance.grid.WorldToCell(controller.transform.position + new Vector3(controller.direction.x, controller.direction.y * 2));
                        break;
                }
                
                var myGridPos = NodeGrid.Instance.grid.WorldToCell(transform.position);
                var path = pathfinder.FindPath(new Vector2Int(myGridPos.x, myGridPos.y), new Vector2Int(playerGridPos.x, playerGridPos.y));
                if (path.Count > 0) {
                    transform.position = NodeGrid.Instance.grid.CellToWorld(path[0]) + new Vector3(0.5f, 0.6f);
                }
            }
        }
    }
}
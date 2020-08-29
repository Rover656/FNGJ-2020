using Pathfinding;
using UnityEngine;

namespace DefaultNamespace {
    public class Hero : MonoBehaviour {
        public Pathfinder pathfinder;
        public CellController cellController;

        private float _moveTimer;
        
        private void Update() {
            // Move toward exit door.
            var target = cellController.endCell.transform.position;
            var targetGridPos = NodeGrid.Instance.grid.WorldToCell(target);
            var myGridPos = NodeGrid.Instance.grid.WorldToCell(transform.position);

            var path = pathfinder.FindPath(new Vector2Int(myGridPos.x, myGridPos.y), new Vector2Int(targetGridPos.x, targetGridPos.y));
            if (path.Count > 0) {
                _moveTimer += Time.deltaTime;

                if (_moveTimer >= 1) {
                    _moveTimer = 0;
                    transform.position = NodeGrid.Instance.grid.CellToWorld(path[0]);
                    cellController.UpdateAround(NodeGrid.Instance.grid.WorldToCell(transform.position), this);
                }
            }
        }
    }
}
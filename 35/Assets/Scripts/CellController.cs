using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Animations;

namespace DefaultNamespace {
    public class CellController : MonoBehaviour {
        public Grid grid;

        // Maximum grid extents.
        public int width;
        public int height;

        public Cell[,] cells;

        public Cell startCell;
        public Cell endCell;

        public GameObject cellPrefab;

        public GameObject heroPrefab;

        public bool simulationRunning;

        private float _simulationTimer;

        private Vector3Int offset;

        private List<Hero> _heroes;

        private int _summoned;
        private float _summonTimer;
        
        private void Start() {
            BuildGridCells();
            _heroes = new List<Hero>();
        }

        public void BuildGridCells() {
            cells = new Cell[width, height];

            offset = new Vector3Int(-width / 2, -height / 2, 0);

            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    // Create cell and store
                    cells[x,y] = Instantiate(cellPrefab, transform.position + new Vector3(x + .5f, y + .5f) + offset, Quaternion.identity).GetComponent<Cell>();
                    cells[x,y].gridPos = new Vector3Int(x, y, 0);
                }
            }
        }

        public bool SetCell(Vector3Int pos, CellType type) {
            if (!InRange(pos)) return false;
            pos -= offset;
            return cells[pos.x, pos.y].SetType(type);
        }

        public void BeginSimulation() {
            if (simulationRunning) return;
            // Find start and end cells
            foreach (var cell in cells) {
                if (cell.GetCellType() == CellType.End)
                    endCell = cell;
                else if (cell.GetCellType() == CellType.Start)
                    startCell = cell;
            }

            NodeGrid.Instance.Refresh();

            simulationRunning = true;
            SummonHero();
        }

        private void SummonHero() {
            var hero = Instantiate(heroPrefab, startCell.transform.position, Quaternion.identity).GetComponent<Hero>();
            hero.cellController = this;
            _heroes.Add(hero);
            _summoned++;
        }

        private void Update() {
            if (simulationRunning) {
                if (_summoned < GameState.Instance.heroesThisWave) {
                    _summonTimer += Time.deltaTime;
                    if (_summonTimer > 1f) {
                        _summonTimer = 0;
                        SummonHero();
                    }
                }

                if (_heroes.Count > 0) {
                    var remove = new List<Hero>();
                    foreach (var h in _heroes)
                        if (h == null) remove.Add(h);
                    foreach (var h in remove)
                        _heroes.Remove(h);
                } else if (_heroes.Count == 0 && _summoned >= GameState.Instance.heroesThisWave) {
                    GameState.Instance.Win();
                    EndSimulation();
                    _summoned = 0;
                }
            }
        }

        public void EndSimulation() {
            simulationRunning = false;
            foreach (var hero in _heroes)
                Destroy(hero.gameObject);
            _heroes.Clear();
        }

        public void UpdateAround(Vector3Int cellPos, Hero triggerer) {
            if (!InRange(cellPos)) return;
            var correctedPos = cellPos - offset;
            var cell = cells[correctedPos.x, correctedPos.y];
            cell.ProcessAction(triggerer, true);
            var neighbours = GetNeighbours(cell);
            foreach (var c in neighbours)
                c.ProcessAction(triggerer, false);
        }

        public CellType GetType(Vector3Int cell) {
            if (!InRange(cell)) return CellType.None;
            var corrected = cell - offset;
            return cells[corrected.x, corrected.y].GetCellType();
        }
        
        public List<Cell> GetNeighbours(Cell node) {
            var neighbours = new List<Cell>();

            for (var x = -1; x <= 1; x++) {
                for (var y = -1; y <= 1; y++) {
                    if ((x == 0 && y == 0)) continue;
                    AddNeighbour(node, x, y, ref neighbours);
                }
            }

            return neighbours;
        }

        private void AddNeighbour(Cell node, int x, int y, ref List<Cell> neighbours) {
            var checkX = node.gridPos.x + x;
            var checkY = node.gridPos.y + y;
            var corrected = new Vector3Int(checkX, checkY, 0);
            if (!(corrected.x < 0 || corrected.x > width || corrected.y < 0 || corrected.y > height))
                neighbours.Add(cells[corrected.x, corrected.y]);
        }

        public bool InRange(Vector3Int gridCell) {
            var corrected = gridCell - offset;
            return !(corrected.x < 0 || corrected.x > width || corrected.y < 0 || corrected.y > height);
        }
    }
}
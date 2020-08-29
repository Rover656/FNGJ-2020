using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace DefaultNamespace {
    public enum PlacementMode {
        Floor,
        Wall,
        Cell
    }
    public class PlayerController : MonoBehaviour {
        public Grid grid;
        public Tilemap floors;
        public Tilemap walls;

        public CellController cellController;

        public List<Tile> floorTiles;
        public List<Tile> wallTiles;

        public PlacementMode mode;
        public CellType cellType;

        public HelperDoodad doodad;

        public static bool IsPointerOverUIElement()
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Mouse.current.position.ReadValue();
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
        
        private void Update() {
            if (cellController.simulationRunning) {
                doodad.SetSprite(null);
            } else {
                var mPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mPos.z = 0;
                var gridPos = grid.WorldToCell(mPos);

                if (cellController.InRange(gridPos) && !IsPointerOverUIElement()) {
                    doodad.transform.position = gridPos + new Vector3(0.5f, 0.5f);
                    switch (mode) {
                        case PlacementMode.Floor:
                            doodad.SetSprite(floorTiles[0].sprite);
                            break;
                        case PlacementMode.Wall:
                            doodad.SetSprite(wallTiles[0].sprite);
                            break;
                        case PlacementMode.Cell:
                            var c = cellController.cellPrefab.GetComponent<Cell>();
                            switch (cellType) {
                                case CellType.None:
                                    doodad.SetSprite(null);
                                    break;
                                case CellType.Start:
                                    doodad.SetSprite(c.startDoor);
                                    break;
                                case CellType.End:
                                    doodad.SetSprite(c.endDoor);
                                    break;
                                case CellType.Spikes:
                                    doodad.SetSprite(c.spike);
                                    break;
                                case CellType.Goblin:
                                    doodad.SetSprite(c.goblin);
                                    break;
                                case CellType.Mimic:
                                    doodad.SetSprite(c.mimic);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    if (Mouse.current.leftButton.isPressed && !cellController.simulationRunning) {
                        switch (mode) {
                            case PlacementMode.Floor:
                                if (GameState.Instance.balance >= 5 && floors.GetTile(gridPos) == null && walls.GetTile(gridPos) == null) {
                                    GameState.Instance.balance -= 5;
                                    floors.SetTile(gridPos, floorTiles[Random.Range(0, floorTiles.Count - 1)]);
                                }
                                break;
                            case PlacementMode.Wall:
                                if (GameState.Instance.balance >= 5 && floors.GetTile(gridPos) == null && walls.GetTile(gridPos) == null) {
                                    GameState.Instance.balance -= 5;
                                    walls.SetTile(gridPos, wallTiles[Random.Range(0, wallTiles.Count - 1)]);
                                }
                                break;
                            case PlacementMode.Cell:
                                var cost = Cell.CellCost(cellType);
                                if (GameState.Instance.balance >= cost && cellController.GetType(gridPos) == CellType.None) {
                                    GameState.Instance.balance -= cost;
                                    if (cellController.SetCell(gridPos, cellType))
                                        floors.SetTile(gridPos, floorTiles[Random.Range(0, floorTiles.Count - 1)]);
                                }
                                break;
                        }
                    }
                } else {
                    doodad.SetSprite(null);
                }
            }
        }

        public void Place(InputAction.CallbackContext ctx) {
            
        }

        public void Remove(InputAction.CallbackContext ctx) {
            if (ctx.performed) {
                var mousePosition = Mouse.current.position.ReadValue();
                var mPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mPos.z = 0;
                var gridPos = grid.WorldToCell(mPos);
                
                // Get price to repay
                if (floors.GetTile(gridPos) != null)
                    GameState.Instance.balance += 5;
                floors.SetTile(gridPos, null);
                
                if (walls.GetTile(gridPos) != null)
                    GameState.Instance.balance += 5;
                walls.SetTile(gridPos, null);

                var curType = cellController.GetType(gridPos);
                GameState.Instance.balance += Cell.CellCost(curType);
                cellController.SetCell(gridPos, CellType.None);
            }
        }
    }
}
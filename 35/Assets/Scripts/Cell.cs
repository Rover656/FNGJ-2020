using System;
using UnityEngine;

namespace DefaultNamespace {
    public enum CellType {
        None,
        Start,
        End,
        Spikes,
        Goblin,
        Mimic
    }
    public class Cell : MonoBehaviour {
        public Sprite startDoor;
        public Sprite endDoor;
        public Sprite spike;
        public Sprite spikeClosed;
        public Sprite goblin;
        public Sprite mimic;
        public Sprite mimicClosed;

        public Vector3Int gridPos;

        private CellType type;

        private int hitCounter;

        public static int CellCost(CellType type) {
            switch (type) {
                case CellType.None:
                    break;
                case CellType.Start:
                    return 1;
                case CellType.End:
                    return 1;
                case CellType.Spikes:
                    return 15;
                case CellType.Goblin:
                    return 35;
                case CellType.Mimic:
                    return 100;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return 0;
        }

        public void ProcessAction(Hero hero, bool direct) {
            switch (type) {
                case CellType.None:
                    break;
                case CellType.Start:
                    break;
                case CellType.End:
                    if (direct) {
                        hero.cellController.EndSimulation();
                        GameState.Instance.Lose();
                    }
                    break;
                case CellType.Spikes:
                    if (direct && GetComponent<SpriteRenderer>().sprite == spike) {
                        Destroy(hero.gameObject);
                        SetType(CellType.None);
                    } else if (hero != null) {
                        GetComponent<SpriteRenderer>().sprite = spike;
                    }
                    break;
                case CellType.Goblin: {
                    if (direct) {
                        Destroy(hero.gameObject);
                        hitCounter--;
                    }

                    if (hitCounter <= 0)
                        SetType(CellType.None);
                    break;
                }
                case CellType.Mimic: {
                    if (direct) {
                        Destroy(hero.gameObject);
                        hitCounter--;
                        GetComponent<SpriteRenderer>().sprite = mimic;
                    }
                    if (hitCounter <= 0)
                        SetType(CellType.None);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool SetType(CellType newType) {
            type = newType;
            switch (type) {
                case CellType.None:
                    GetComponent<SpriteRenderer>().sprite = null;
                    return false;
                case CellType.Start:
                    GetComponent<SpriteRenderer>().sprite = startDoor;
                    return true;
                case CellType.End:
                    GetComponent<SpriteRenderer>().sprite = endDoor;
                    return true;
                case CellType.Spikes:
                    GetComponent<SpriteRenderer>().sprite = spikeClosed;
                    return true;
                case CellType.Goblin:
                    hitCounter = 3;
                    GetComponent<SpriteRenderer>().sprite = goblin;
                    return true;
                case CellType.Mimic:
                    hitCounter = 6;
                    GetComponent<SpriteRenderer>().sprite = mimicClosed;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public CellType GetCellType() {
            return type;
        }
    }
}
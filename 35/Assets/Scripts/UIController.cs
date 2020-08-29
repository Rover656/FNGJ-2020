using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class UIController : MonoBehaviour {
        public PlayerController player;

        public Text balanceText;
        public Text heroText;

        public Text winLoseText;

        public void Floor() {
            player.mode = PlacementMode.Floor;
        }
        
        public void Wall() {
            player.mode = PlacementMode.Wall;
        }

        public void StartB() {
            player.mode = PlacementMode.Cell;
            player.cellType = CellType.Start;
        }
        
        public void End() {
            player.mode = PlacementMode.Cell;
            player.cellType = CellType.End;
        }
        
        public void Spike() {
            player.mode = PlacementMode.Cell;
            player.cellType = CellType.Spikes;
        }

        public void Goblin() {
            player.mode = PlacementMode.Cell;
            player.cellType = CellType.Goblin;
        }

        public void Mimic() {
            player.mode = PlacementMode.Cell;
            player.cellType = CellType.Mimic;
        }

        public void RunSimulation() {
            player.cellController.BeginSimulation();
        }

        public void StopSimulation() {
            player.cellController.EndSimulation();
        }
    }
}
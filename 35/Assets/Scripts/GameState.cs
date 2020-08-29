using System.Collections;
using UnityEngine;

namespace DefaultNamespace {
    public class GameState : MonoBehaviour {
        public static GameState Instance;

        public int heroesThisWave = 2;

        public UIController ui;

        public int balance = 150;
        
        private void Update() {
            ui.balanceText.text = "Balance: " + balance;
            ui.heroText.text = "Heroes: " + heroesThisWave;
        }

        private IEnumerator DisappearText() {
            yield return new WaitForSeconds(2f);
            ui.winLoseText.enabled = false;
        }

        public void Win() {
            balance += heroesThisWave * 25;
            heroesThisWave += heroesThisWave / 2;
            ui.winLoseText.enabled = true;
            ui.winLoseText.text = "Wave Won!";
            StartCoroutine(DisappearText());
        }

        public void Lose() {
            ui.winLoseText.enabled = true;
            ui.winLoseText.text = "Wave Lost!";
            StartCoroutine(DisappearText());
        }
        
        private void Awake() {
            Instance = this;
        }
    }
}
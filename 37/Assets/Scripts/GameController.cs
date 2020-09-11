using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class GameController : MonoBehaviour {
        public static GameController instance;
        
        public int colors;
        public PickupColor currentActive;
        private PickupColor _lastActive;

        public GameObject mainMenu;
        public GameObject winScreen;
        public GameObject loseScreen;

        public Text winTimer;
        public Text loseTimer;

        [Serializable]
        public struct ObjectColorFilter {
            public GameObject obj;
            public PickupColor color;
        }

        public List<ObjectColorFilter> filters;

        public Dictionary<PickupColor, GameObject> _dict = new Dictionary<PickupColor, GameObject>();

        private Stopwatch _timer = new Stopwatch();

        private void Awake() {
            instance = this;
            
            // Turn filters into dict
            foreach (var thing in filters) {
                _dict[thing.color] = thing.obj;
                
                // By default all colours are off.
                Hide(thing.color);
            }

            // #if !UNITY_EDITOR
            Time.timeScale = 0f;
            // #endif
        }

        private void Update() {
            if (_lastActive != currentActive) {
                if (_dict.ContainsKey(_lastActive))
                    MakeTranslucent(_lastActive);
                if (_dict.ContainsKey(currentActive))
                    MakeVisible(currentActive);
                _lastActive = currentActive;
            }
        }

        private void MakeVisible(PickupColor color) {
            var root = _dict[color];
            var rootMap = root.GetComponent<Tilemap>();
            MakeOpaque(rootMap);
            foreach (var map in root.GetComponentsInChildren<Tilemap>()) {
                if (map.gameObject.name.Contains("Decorations"))
                    MakeDecorationOpaque(map);
                else MakeOpaque(map);
            }

            root.GetComponent<Collider2D>().enabled = true;
            foreach (var collider in root.GetComponentsInChildren<Collider2D>())
                collider.enabled = true;
        }

        private void MakeTranslucent(PickupColor color) {
            Show(color);
            var root = _dict[color];
            var rootMap = root.GetComponent<Tilemap>();
            MakeTranslucent(rootMap);
            foreach (var map in root.GetComponentsInChildren<Tilemap>())
                MakeTranslucent(map);

            root.GetComponent<Collider2D>().enabled = false;
            foreach (var collider in root.GetComponentsInChildren<Collider2D>())
                collider.enabled = false;
        }

        private void Hide(PickupColor color) {
            var root = _dict[color];
            root.SetActive(false);
        }

        private void Show(PickupColor color) {
            var root = _dict[color];
            root.SetActive(true);
        }

        private void MakeTranslucent(Tilemap tilemap) {
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, .1f);
        }
        
        private void MakeOpaque(Tilemap tilemap) {
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 1f);
        }
        
        private void MakeDecorationOpaque(Tilemap tilemap) {
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0.3f);
        }

        public void AddColor(PickupColor color) {
            // #if !UNITY_EDITOR
            if (colors == 0)
            currentActive = color;
            // #endif
            colors |= (int) color;
            MakeTranslucent(color);
        }

        public bool HasColor(PickupColor color) {
            // #if UNITY_EDITOR
            // return true;
            // #endif
            return (colors & (int) color) != 0;
        }

        public void Died() {
            _timer.Stop();
            Time.timeScale = 0f;
            loseTimer.text = "after: " + _timer.Elapsed.ToString("mm\\:ss\\.ff");
            loseScreen.SetActive(true);
        }

        public void Win() {
            _timer.Stop();
            Time.timeScale = 0f;
            winTimer.text = "in: " + _timer.Elapsed.ToString("mm\\:ss\\.ff");
            winScreen.SetActive(true);
        }

        public void Play() {
            Time.timeScale = 1f;
            mainMenu.SetActive(false);
            _timer.Start();
        }

        public void Restart() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Quit() {
            Application.Quit();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace DefaultNamespace {
    public class GameManager : MonoBehaviour {
        public int lives;
        public int length = 1;
        public int points;

        public Vector2 playerStart;

        public Tilemap walls;

        public GameObject playerPrefab; 
        
        public Image directionalIndicator;

        public Text scoreText;
        public Text livesText;
        public Text lengthText;

        public GameObject pelletPrefab;

        public BoxCollider2D enemyZone;

        public Canvas playCanvas;
        public Canvas deadCanvas;
        public Text deadScoreText;
        public Canvas winCanvas;
        public Text winScoreText;

        public AudioClip die;
        public AudioClip respawn;

        public static GameManager Instance;

        private int totalPellets;

        public void RespawnOrEnd() {
            GetComponent<AudioSource>().PlayOneShot(die);
            if (lives > 0) {
                lives--;
                foreach (var enemy in FindObjectsOfType<EnemyAI>())
                    enemy.Reset();
                StartCoroutine(Respawn());
            } else {
                deadCanvas.gameObject.SetActive(true);
                deadScoreText.text = "You scored: " + points;
            }
        }
        
        private IEnumerator Respawn() {
            yield return new WaitForSeconds(1f);
            Instantiate(playerPrefab, new Vector3(playerStart.x, playerStart.y, 0), Quaternion.identity);
            GetComponent<AudioSource>().PlayOneShot(respawn);
        }

        private void Update() {
            scoreText.text = "Score: " + points;
            livesText.text = "Lives: " + lives;
            lengthText.text = "Length: " + length + "m";

            if (points == totalPellets) {
                Time.timeScale = 0f;
                winCanvas.gameObject.SetActive(true);
                winScoreText.text = "You scored: " + points;
            }
        }

        public void GainPoint() {
            points++;
            if (points % 8 == 0) {
                PlayerController.Instance.Grow();
                length++;
            }
        }

        public void FillPellets() {
            totalPellets = 0;
            for (var x = walls.cellBounds.min.x; x < walls.cellBounds.max.x; x++) {
                for (var y = walls.cellBounds.min.x; y < walls.cellBounds.max.x; y++) {
                    var tile = walls.GetTile(new Vector3Int(x, y, 0));
                    if (tile == null) {
                        // TODO: Look for super pellet before spawning
                        var worldPos = walls.layoutGrid.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f);
                        if (!enemyZone.OverlapPoint(worldPos) && !Physics2D.OverlapPoint(worldPos)) {
                            Instantiate(pelletPrefab, worldPos, Quaternion.identity);
                            totalPellets++;
                        }
                    }
                }
            }
        }

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            FillPellets();
            Time.timeScale = 0f;
        }

        public void Play() {
            Time.timeScale = 1f;
            playCanvas.gameObject.SetActive(false);
        }

        public void Retry() {
            SceneManager.LoadScene(0);
        }

        public void Quit() {
            Application.Quit();
        }
    }
}
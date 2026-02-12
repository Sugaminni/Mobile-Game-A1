using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public PlayerController player;
    public Transform rock;

    [Header("UI")]
    public Text scoreText; 
    public GameObject gameOverPanel;

    [Header("Spawn Settings")]
    public Vector3 playerSpawn = new Vector3(0f, 0.5f, 0f);
    public float rockSpawnY = 0.5f;
    public float spawnRangeX = 6f;
    public float spawnRangeZ = 6f;

    private int score;
    private bool gameOver;

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        score = 0;
        gameOver = false;
        UpdateScoreUI();

        // Ensure player has reference back to this manager
        if (player != null) player.gameManager = this;

        RespawnRock();
    }

    public void OnRockHit()
    {
        if (gameOver) return;

        score++;
        UpdateScoreUI();
        RespawnRock();
    }

    public void GameOver()
    {
        if (gameOver) return;

        gameOver = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        // Reloads the current scene to reset everything 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RespawnRock()
    {
        if (rock == null) return;

        float x = Random.Range(-spawnRangeX, spawnRangeX);
        float z = Random.Range(-spawnRangeZ, spawnRangeZ);

        rock.position = new Vector3(x, rockSpawnY, z);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
}

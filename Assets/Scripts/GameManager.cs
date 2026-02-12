using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public PlayerController player;
    public Transform rock;

    [Header("UI")]
    public TMP_Text scoreText;
    public GameObject gameOverPanel;

    [Header("Spawn Settings")]
    public Vector3 playerSpawn = new Vector3(0f, 0.5f, 0f);
    public float rockSpawnY = 0.5f;

    [Header("Ground Bounds")]
    public Transform ground;
    public float wallMargin = 1.5f;
    public float minDistanceFromPlayer = 6f;

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

        // Optional: ensure player starts at your intended spawn
        if (player != null) player.ResetPlayer(playerSpawn);

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
        if (rock == null || ground == null) return;

        // Scales the spawn area based on the ground's size
        float halfWidth = (10f * ground.localScale.x) * 0.5f;
        float halfDepth = (10f * ground.localScale.z) * 0.5f;

        float minX = -halfWidth + wallMargin;
        float maxX = halfWidth - wallMargin;
        float minZ = -halfDepth + wallMargin;
        float maxZ = halfDepth - wallMargin;

        Vector3 candidate;
        int safety = 0;

        do
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);

            // Spawn relative to the ground's center so it works even if ground isn't at (0,0,0)
            candidate = new Vector3(ground.position.x + x, rockSpawnY, ground.position.z + z);

            safety++;
            if (safety > 50) break; // prevents infinite loop
        }
        while (player != null && Vector3.Distance(candidate, player.transform.position) < minDistanceFromPlayer);

        rock.position = candidate;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
}

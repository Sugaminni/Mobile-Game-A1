using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveForce = 12f;

    [Header("Refs")]
    public GameManager gameManager;

    private Rigidbody rb;
    private bool isAlive = true;

    // UI input values (for mobile buttons)
    private float uiH = 0f;
    private float uiV = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;

        // Keyboard input for testing (WASD / Arrow Keys)
        // If UI buttons are being pressed, use those values instead
        float h = Mathf.Abs(uiH) > 0.01f ? uiH : Input.GetAxis("Horizontal");
        float v = Mathf.Abs(uiV) > 0.01f ? uiV : Input.GetAxis("Vertical");

        Vector3 force = new Vector3(h, 0f, v) * moveForce;
        rb.AddForce(force, ForceMode.Acceleration);

        // Fail condition: fell off the platform
        if (transform.position.y < -5f)
        {
            isAlive = false;
            if (gameManager != null) gameManager.GameOver();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isAlive) return;

        if (other.collider.CompareTag("Rock"))
        {
            if (gameManager != null) gameManager.OnRockHit();
        }
    }

    // Called by UI Buttons 
    public void SetHorizontal(float value)
    {
        uiH = value;
    }

    public void SetVertical(float value)
    {
        uiV = value;
    }

    public void ResetPlayer(Vector3 spawnPos)
    {
        isAlive = true;
        transform.position = spawnPos;
        rb.linearVelocity = Vector3.zero; 
        rb.angularVelocity = Vector3.zero;
    }
}

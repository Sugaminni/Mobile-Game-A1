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

    // UI input values
    private float uiH = 0f;
    private float uiV = 0f;

    // Control modes
    public enum ControlMode { DPad, Tilt, Joystick }

    [Header("Control Mode")]
    public ControlMode controlMode = ControlMode.DPad;

    [Header("Tilt Settings")]
    public float tiltSensitivity = 1.5f;
    public float tiltDeadZone = 0.08f;

    private Vector3 tiltZero; // calibration

    [Header("Control UI References")]
    public GameObject dpadUI;
    public GameObject joystickUI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Save initial device tilt as baseline
        tiltZero = Input.acceleration;

        // Make sure the correct UI is visible on start
        ApplyControlUIVisibility();
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;

        float h = 0f;
        float v = 0f;

        // Keyboard input for testing (WASD / Arrow Keys)
        // If UI buttons are being pressed, use those values instead
        if (controlMode == ControlMode.DPad)
        {
            h = Mathf.Abs(uiH) > 0.01f ? uiH : Input.GetAxis("Horizontal");
            v = Mathf.Abs(uiV) > 0.01f ? uiV : Input.GetAxis("Vertical");
        }
        else if (controlMode == ControlMode.Tilt)
        {
            Vector3 a = Input.acceleration - tiltZero;

            h = ApplyDeadZone(a.x) * tiltSensitivity;
            v = ApplyDeadZone(a.y) * tiltSensitivity; // if forward/back feels reversed, flip the sign

            h = Mathf.Clamp(h, -1f, 1f);
            v = Mathf.Clamp(v, -1f, 1f);
        }
        else if (controlMode == ControlMode.Joystick)
        {
            // Joystick will also drive uiH/uiV (just like DPad)
            h = uiH;
            v = uiV;
        }

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

    // Called by UI Buttons to switch control modes
    public void SetControlModeDPad()
    {
        controlMode = ControlMode.DPad;

        // Stop leftover joystick values
        uiH = 0f;
        uiV = 0f;

        ApplyControlUIVisibility();
    }

    public void SetControlModeTilt()
    {
        controlMode = ControlMode.Tilt;
        uiH = 0f;
        uiV = 0f;

        ApplyControlUIVisibility();
    }

    public void SetControlModeJoystick()
    {
        controlMode = ControlMode.Joystick;

        ApplyControlUIVisibility();
    }

    // Called by a UI button to recalibrate the tilt
    public void CalibrateTilt()
    {
        tiltZero = Input.acceleration;
    }

    private float ApplyDeadZone(float value)
    {
        if (Mathf.Abs(value) < tiltDeadZone) return 0f;
        return value;
    }

    private void ApplyControlUIVisibility()
    {
        // Show only the UI that matches the current control mode
        if (dpadUI != null)
            dpadUI.SetActive(controlMode == ControlMode.DPad);

        if (joystickUI != null)
            joystickUI.SetActive(controlMode == ControlMode.Joystick);
    }

    public void ResetPlayer(Vector3 spawnPos)
    {
        isAlive = true;
        transform.position = spawnPos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}

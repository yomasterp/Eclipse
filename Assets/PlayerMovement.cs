using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float dashDamage = 1f;

    [Header("Look")]
    public float mouseSensitivity = 2f;

    // expose as read-only
    public bool IsDashing { get; private set; }

    // UI
    public TMP_Text speedText;
    public TMP_Text dashText;

    // internal state
    private CharacterController controller;
    private float rotationX;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;
    public bool canLook = true;

    private AudioSource footstepAudio;
    private AudioSource audioSource;
    public AudioClip dashSound;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // footsteps + SFX
        var sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            footstepAudio = sources[0];
            audioSource = sources[1];
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleDashCooldown();
        HandleMovement();

        if (canLook)
            HandleMouseLook();

        // update UI
        if (speedText != null)
            speedText.text = $"Speed: {speed:F1}";
        if (dashText != null)
            dashText.text = $"Dash:  {dashSpeed:F1}";
    }

    void HandleDashCooldown()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }

    void HandleMovement()
    {
        // Dashing
        if (IsDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
                EndDash();

            if (footstepAudio != null && footstepAudio.isPlaying)
                footstepAudio.Stop();
            return;
        }

        // Regular movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        // footsteps
        if (move.sqrMagnitude > 0.01f)
        {
            if (footstepAudio != null && !footstepAudio.isPlaying)
                footstepAudio.Play();
        }
        else if (footstepAudio != null && footstepAudio.isPlaying)
            footstepAudio.Stop();

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) &&
            move.sqrMagnitude > 0.01f &&
            dashCooldownTimer <= 0f)
        {
            StartDash(move.normalized);
        }
    }

    void StartDash(Vector3 direction)
    {
        IsDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = direction;

        if (dashSound != null && audioSource != null)
            audioSource.PlayOneShot(dashSound);
    }

    void EndDash()
    {
        IsDashing = false;
    }

    // Called automatically by CharacterController on collisions
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!IsDashing) return;

        var eh = hit.collider.GetComponent<EnemyHealth>();
        if (eh != null)
        {
            eh.TakeDamage(dashDamage);
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashSpeed * 0.1f);
    }
}

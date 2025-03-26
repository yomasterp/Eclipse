using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float mouseSensitivity = 2f;
    public float dashDamage = 1f;
    public float dashHitRadius = 1.5f;

    private CharacterController controller;
    private float rotationX = 0f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleDashCooldown();
        HandleMovement();
        HandleMouseLook();

        if (isDashing)
        {
            CheckDashHit();
        }
    }

    void HandleDashCooldown()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    void HandleMovement()
    {
        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f) isDashing = false;
            return;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftShift) && move != Vector3.zero && dashCooldownTimer <= 0f)
        {
            StartDash(move.normalized);
        }
    }

    void StartDash(Vector3 direction)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = direction;
    }

    void CheckDashHit()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, dashHitRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(dashDamage);
                }
            }
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
        Gizmos.DrawWireSphere(transform.position, dashHitRadius);
    }
}
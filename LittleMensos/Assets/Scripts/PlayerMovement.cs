using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4.5f;
    public float sprintSpeed = 7f;
    public float crouchSpeedMultiplier = 0.5f;

    public float acceleration = 18f;
    public float deceleration = 22f;

    private Vector3 currentVelocity;

    [Header("Jump")]
    public float jumpForce = 5f;
    public bool isGrounded;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private bool jumpHeld;

    [Header("Dash")]
    public float dashForce = 10f;
    public float dashDuration = 0.15f;
    public float dashPause = 0.05f;

    [Header("Crouch")]
    public float crouchScaleY = 0.6f;
    public bool isCrouched;

    [Header("Game Limits Z")]
    public float minZ = -3f;
    public float maxZ = 3f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private Vector3 originalScale;

    private bool isSprinting;
    private bool isDashing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ApplyBetterJump();
    }

    // ===== MOVEMENT CORE =====

    void ApplyMovement()
    {
        if (isDashing) return;

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if (isCrouched)
            targetSpeed *= crouchSpeedMultiplier;

        Vector3 targetVelocity = inputDir * targetSpeed;

        float accel = inputDir.magnitude > 0 ? acceleration : deceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            accel * Time.fixedDeltaTime
        );

        Vector3 targetPos = rb.position + currentVelocity * Time.fixedDeltaTime;

        // profundidad limitada tipo Inside
        targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);

        rb.MovePosition(targetPos);
    }

    // ===== JUMP FEELING =====

    void ApplyBetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    // ===== INPUT SYSTEM =====

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (isCrouched) return;
        isSprinting = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isCrouched)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jumpHeld = true;
        }

        if (context.canceled)
            jumpHeld = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing)
            StartCoroutine(Dash());
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            isCrouched = true;
            isSprinting = false;

            transform.localScale = new Vector3(
                originalScale.x,
                crouchScaleY,
                originalScale.z
            );
        }
        else if (context.canceled)
        {
            isCrouched = false;
            transform.localScale = originalScale;
        }
    }

    // ===== DASH =====

    IEnumerator Dash()
    {
        isDashing = true;
        currentVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(dashPause); // micro anticipaci�n

        Vector3 dashDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        rb.AddForce(dashDir * dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    // ===== GROUND CHECK =====

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}

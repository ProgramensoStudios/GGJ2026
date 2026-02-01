using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4.5f;
    public float sprintSpeed = 7f;
    public float crouchSpeedMultiplier = 0.5f;
    public float maskSpeedMultiplier = 1.5f;
    public bool canMove;

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
    private bool airDashed;

    public float dashPause = 0.05f;

    [Header("Crouch")]
    public float crouchScaleY = 0.6f;
    public bool isCrouched;

    [Header("Stamina")]
    public float maxStamina = 5f;
    public float staminaDrain = 1.2f;
    public float staminaRecovery = 0.8f;
    public float staminaCooldown = 10f;
    public float recoverThreshold = 0.6f;

    private float currentStamina;
    private float staminaTimer;
    private bool isTired;

    [Header("Interaction")]
    public float interactRange = 2f;
    public Transform faceTransform;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Game Limits Z")]
    public float minZ = -3f;
    public float maxZ = 3f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private Vector3 originalScale;

    private bool isSprinting;
    private bool isDashing;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
        currentStamina = maxStamina;
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        HandleStamina();
        ApplyMovement();
        RotateTowardsMovement();
        ApplyBetterJump();
    }

    // ===== MOVEMENT CORE =====
    void ApplyMovement()
    {
        if (isDashing) return;

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        float staminaFactor = isTired ? 0.85f : 1f;
        float targetSpeed = (isSprinting ? sprintSpeed : walkSpeed) * staminaFactor;

        // Si tiene máscara activa, aumenta velocidad
        if (MaskManager.Instance.CanDash() || MaskManager.Instance.CanClimb())
        {
            targetSpeed *= maskSpeedMultiplier;
        }

        if (isCrouched)
            targetSpeed *= crouchSpeedMultiplier;

        Vector3 targetVelocity = inputDir * targetSpeed;
        float accel = inputDir.magnitude > 0 ? acceleration : deceleration;

        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accel * Time.fixedDeltaTime);

        Vector3 targetPos = rb.position + currentVelocity * Time.fixedDeltaTime;
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
        if (isCrouched || isTired) return;
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
        if (context.performed && !isDashing && !isTired && MaskManager.Instance.CanDash())
        StartCoroutine(Dash());
    }

    void CheckTreeClimb()
    {
        if (!MaskManager.Instance.CanClimb()) return;
        Debug.Log("trepa");
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            isCrouched = true;
            isSprinting = false;

            transform.localScale = new Vector3(originalScale.x, crouchScaleY, originalScale.z);
        }
        else if (context.canceled)
        {
            isCrouched = false;
            transform.localScale = originalScale;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started) Interact();
    }

    public void OnMask(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            MaskManager.Instance.CycleMask();
        }
    }


    IEnumerator Dash()
    {
        isDashing = true;

        currentVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(dashPause);

        Vector3 dashDir;

        if (!isGrounded)
        {
            if (airDashed) yield break;
            airDashed = true;
            // DASH AÉREO: adelante + arriba
            dashDir = (transform.forward + Vector3.up * 0.7f).normalized;
        }
        else
        {
            // DASH TERRESTRE NORMAL
            dashDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            if (dashDir == Vector3.zero)
                dashDir = transform.forward;
        }

        rb.AddForce(dashDir * dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(dashDuration);

        // amortiguamos velocidad para que no se descontrole
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x * 0.3f,
            rb.linearVelocity.y * 0.6f,
            rb.linearVelocity.z * 0.3f
        );

        isDashing = false;
    }


    // ===== ROTATION =====
    void RotateTowardsMovement()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        if (inputDir.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(inputDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    // ===== STAMINA =====
    void HandleStamina()
    {
        if (isSprinting && !isCrouched && !isDashing && !isTired && moveInput.magnitude > 0)
        {
            currentStamina -= staminaDrain * Time.fixedDeltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isTired = true;
                isSprinting = false;
                staminaTimer = staminaCooldown;
            }
        }
        else
        {
            staminaTimer -= Time.fixedDeltaTime;
            if (staminaTimer <= 0 && currentStamina < maxStamina)
            {
                currentStamina += staminaRecovery * Time.fixedDeltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }
        }

        if (isTired && currentStamina >= maxStamina * recoverThreshold)
        {
            isTired = false;
        }
    }

    // ===== INTERACT =====
    public void Interact()
    {
        if (faceTransform == null) return;

        RaycastHit hit;
        Vector3 origin = faceTransform.position;
        Vector3 direction = faceTransform.forward;

        if (Physics.Raycast(origin, direction, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                Debug.Log($"<color=green>Interacted with: {hit.collider.name}</color>");
            }
        }
    }

    // ===== GROUND CHECK =====
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            airDashed = false;
        }
    }

    // ===== GIZMOS =====
    private void OnDrawGizmosSelected()
    {
        if (faceTransform == null) return;

        Gizmos.color = Color.yellow;
        Vector3 origin = faceTransform.position;
        Vector3 direction = faceTransform.forward;

        Gizmos.DrawLine(origin, origin + direction * interactRange);
        Gizmos.DrawSphere(origin + direction * interactRange, 0.05f);
    }
}

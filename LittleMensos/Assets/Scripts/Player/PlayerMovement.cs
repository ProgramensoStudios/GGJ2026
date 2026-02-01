using System.Collections;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
    public bool isGrounded;

    [Header("Jump Anticipation")]
    public float jumpForce = 5f;
    public float jumpAnticipationDelay = 0.08f; // animación primero
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private bool jumpHeld;
    private bool jumpInProgress;
    private Coroutine jumpRoutine;


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

    [Header("UI - Stamina")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaFillImage;


    [Header("Interaction")]
    public float interactRange = 2f;
    public Transform faceTransform;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Game Limits Z")]
    public float minZ = -3f;
    public float maxZ = 3f;

    [Header("Teleport")]
    public float teleportDuration = 1f; // adjustable time
    private bool isTeleporting = false;
    private Transform currentTeleport;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private int speedHash;
    private int verticalVelocityHash;
    private int isGroundedHash;
    private int isCrouchedHash;
    private int dashHash;
    private int jumpTriggerHash;


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

        speedHash = Animator.StringToHash("Speed");
        verticalVelocityHash = Animator.StringToHash("VerticalVelocity");
        isGroundedHash = Animator.StringToHash("isGrounded");
        isCrouchedHash = Animator.StringToHash("isCrouching");
        dashHash = Animator.StringToHash("Dash");
        jumpTriggerHash = Animator.StringToHash("Jump");

        currentStamina = maxStamina;

        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }


    private void FixedUpdate()
    {
        if (!canMove) return;

        HandleStamina();
        ApplyMovement();
        RotateTowardsMovement();
        ApplyBetterJump();
        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        float speed = currentVelocity.magnitude;

        animator.SetFloat("Speed", speed);
        animator.SetFloat(verticalVelocityHash, rb.linearVelocity.y);
        animator.SetBool(isGroundedHash, isGrounded);
        animator.SetBool(isCrouchedHash, isCrouched);
       
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
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
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
        if (context.performed && isGrounded && !isCrouched && !jumpInProgress)
        {
          
            animator.SetTrigger(jumpTriggerHash);

           
            jumpInProgress = true;
            jumpHeld = true;

           
            jumpRoutine = StartCoroutine(DelayedJumpForce());
        }

        if (context.canceled)
            jumpHeld = false;
    }


    private IEnumerator DelayedJumpForce()
    {
        yield return new WaitForSeconds(jumpAnticipationDelay);

        // Limpia velocidad vertical (evita frame muerto)
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false;
    }



    public void OnMaskAction(InputAction.CallbackContext context)
    {
        Debug.Log("A");
        if (context.performed && !isDashing && !isTired && MaskManager.Instance.CanDash())
        StartCoroutine(Dash());

        if (context.performed && !isTeleporting && currentTeleport!=null && MaskManager.Instance.CanClimb())
            StartTeleport();

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
        }
        else if (context.canceled)
        {
            isCrouched = false;
        }
    }

    /*
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started) Interact();
    }
    */


    public void OnMask(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            MaskManager.Instance.CycleMask();
        }
    }

    public void StartTeleport()
    {
        if (currentTeleport.childCount == 0)
        {
            Debug.LogWarning("Teleport object has no children!");
            return;
        }

        Debug.Log("DALE");
        SFXManager.Instance.Play("Teleporter", transform.position);
        Transform targetPoint = currentTeleport.GetChild(0);
        StartCoroutine(MoveToPosition(targetPoint.position, teleportDuration));
    }


    IEnumerator Dash()
    {
        SFXManager.Instance.Play("Dash", transform.position);
        isDashing = true;
        animator.SetTrigger(dashHash);

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

        // UI UPDATE
        staminaSlider.value = currentStamina;
        UpdateStaminaColor();
    }
    void UpdateStaminaColor()
    {
        float percent = currentStamina / maxStamina;

        if (percent > 0.6f)
            staminaFillImage.color = Color.green;
        else if (percent > 0.3f)
            staminaFillImage.color = Color.yellow;
        else
            staminaFillImage.color = Color.red;
    }


    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration) 
    { 
        isTeleporting = true;
        canMove = false;
        Vector3 startPosition = transform.position; 
        float elapsed = 0f; 
        while (elapsed < duration) { elapsed += Time.deltaTime; float t = elapsed / duration; transform.position = Vector3.Lerp(startPosition, targetPosition, t); 
            yield return null; 
        } 
        transform.position = targetPosition; 
        isTeleporting= false; canMove = true; 
    }

    // TRIGGERS
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Teleport"))
        {
            currentTeleport = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Teleport") && other.transform == currentTeleport)
        {
            currentTeleport = null;
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
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            jumpInProgress = false;
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

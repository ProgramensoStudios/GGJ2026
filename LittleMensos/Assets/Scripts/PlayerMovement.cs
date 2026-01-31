using System.Collections;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4.5f;
    public float sprintSpeed = 7f;
    public float crouchSpeedMultiplier = 0.5f;
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
    public float dashPause = 0.05f;

    [Header("Crouch")]
    public float crouchScaleY = 0.6f;
    public bool isCrouched;

    [Header("Stamina")]
    public float maxStamina = 5f;
    public float staminaDrain = 1.2f;     // por segundo corriendo
    public float staminaRecovery = 0.8f;  // por segundo caminando
    public float staminaCooldown = 10f;  // delay antes de recuperar
    public float recoverThreshold = 0.6f; // 60% para dejar de estar cansado

    private float currentStamina;
    private float staminaTimer;
    private bool isTired;

    [Header("Interaction")]
    public float interactRange = 2f; // distancia del raycast
    public Transform faceTransform;  // asigna el transform de la "carita" del player

    [Header("Rotation")]
    public float rotationSpeed = 10f; // cu�n r�pido gira hacia la direcci�n de movimiento

    [Header("Game Settings")]
    [SerializeField] private bool maskOn;
    [SerializeField] private GameObject mask;


    [Header("Game Limits Z")]
    public float minZ = -3f;
    public float maxZ = 3f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private Vector3 originalScale;

    private bool isSprinting;
    private bool isDashing;

    public System.Action<bool> OnMaskStateChanged;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
        currentStamina = maxStamina;
    }

    void FixedUpdate()
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

        if (maskOn)
        {
            targetSpeed *= maskSpeedMultiplier; 
        }

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
        if (context.performed && !isDashing && !isTired && maskOn)
        StartCoroutine(Dash());
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
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
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Interact();
        }
    }

    public void OnMask(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnMaskOnOff();
        }
    }


    // ===== DASH =====

    IEnumerator Dash()
    {
        isDashing = true;

        // corta TODO
        currentVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(dashPause); 

        Vector3 dashDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        // si no hay input, dash hacia adelante
        if (dashDir == Vector3.zero)
            dashDir = transform.right;

        rb.AddForce(dashDir * dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(dashDuration);

        // mata el exceso de velocidad
        rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.3f, rb.linearVelocity.y, rb.linearVelocity.z * 0.3f);

        isDashing = false;
    }

    void RotateTowardsMovement()
    {
        // Si no hay input, no giramos
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        if (inputDir.sqrMagnitude < 0.01f) return;

        // Calculamos la rotaci�n deseada
        Quaternion targetRotation = Quaternion.LookRotation(inputDir, Vector3.up);

        // Rotaci�n suave
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }



    void HandleStamina()
    {
        // Drenar stamina SOLO si puede sprintar
        if (isSprinting && !isCrouched && !isDashing && !isTired && moveInput.magnitude > 0)
        {
            currentStamina -= staminaDrain * Time.fixedDeltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isTired = true;
                isSprinting = false;
                staminaTimer = staminaCooldown;
                Debug.Log("<color=red> Toy cansado jfe </color>");
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

        // SOLO aqu� se recupera del cansancio
        if (isTired && currentStamina >= maxStamina * recoverThreshold)
        {
            isTired = false;
            Debug.Log("<color=blue> Toy recuperado</color>");
        }
    }

    public void OnMaskOnOff()
    {
        if (faceTransform == null) return;
        RaycastHit hit;
        Vector3 origin = faceTransform.position;
        Vector3 direction = faceTransform.forward;

        if (maskOn)
        {
            AddRemoveMask(mask);
        }

        if (Physics.Raycast(origin, direction, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Mask"))
            {
                Debug.Log("<color=green>Estoy tocando un objeto interactuable: " + hit.collider.name + "</color>");
                AddRemoveMask(hit.collider.gameObject);
            }
        }
    }

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
                Debug.Log("<color=green>Estoy tocando un objeto interactuable: " + hit.collider.name + "</color>");
            }
        }
    }

    void AddRemoveMask(GameObject maskCurrent)
    {
        Debug.Log("mask");
        if (!maskOn)
        {
            Debug.Log("puesta");
            mask = maskCurrent;
            maskCurrent.transform.parent = faceTransform;
            maskCurrent.transform.position = faceTransform.position;
            mask.GetComponent<Collider>().enabled = false;
            maskOn = true;
        }
        else
        {
            Debug.Log("quitada");
            maskCurrent.transform.parent = null;
            mask.GetComponent<Collider>().enabled = true;
            mask = null;
            maskOn = false;
        }

        OnMaskStateChanged?.Invoke(maskOn);
    }


    // ===== GROUND CHECK =====

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnDrawGizmosSelected()
    {
        if (faceTransform == null) return;

        Gizmos.color = Color.yellow;

        Vector3 origin = faceTransform.position;
        Vector3 direction = faceTransform.forward;

        // Dibuja una l�nea desde la cara hacia adelante
        Gizmos.DrawLine(origin, origin + direction * interactRange);

        // Dibuja un peque�o punto al final del raycast
        Gizmos.DrawSphere(origin + direction * interactRange, 0.05f);
    }

}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CelestePlayerController : MonoBehaviour
{
    // STATE CONSTANTS
    public const int STATE_IDLE = 0;
    public const int STATE_RUN = 1;
    public const int STATE_JUMP = 2;
    public const int STATE_FALL = 3;
    public const int STATE_CLIMB = 4;
    public const int STATE_DASH = 5;

    [Header("Movement")]
    public float moveSpeed = 9f;
    public float acceleration = 160f;
    public float deceleration = 140f;
    public float airAcceleration = 100f;
    public float turnSpeed = 15f; // Slerp speed

    [Header("Jump")]
    public float jumpForce = 11.5f;
    public float gravity = 48f;
    public float fallGravityMultiplier = 3f;
    public float maxFallSpeed = 25f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.15f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 0.3f;
    public bool canDash = true;
    public TrailRenderer dashTrail;



    [Header("Wall Climb")]
    public float climbSpeed = 4f;
    public float wallCheckDistance = 0.6f;
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 10f;
    public float wallJumpUpForce = 12f;
    public LayerMask wallLayer;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.25f;
    public float groundCheckOffset = 0.05f;
    public LayerMask groundLayer;

    // References
    private CharacterController cc;
    private Animator animator;

    // Runtime State
    public Vector3 velocity;
    private float currentHorizontalSpeed;
    private bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpPressTime;
    private bool isDashing = false;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;
    private bool isClimbing;
    private bool isTouchingWall;
    private Vector3 wallNormal;

    // Input
    private float inputH;
    private float inputV;
    private bool jumpPressed;
    private bool jumpHeld;
    private bool dashPressed;
    private bool grabHeld;
    private bool inputActive = true;


    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!inputActive)
        {
            UpdateAnimator(); // Still update animations so she doesn't freeze awkwardly
            ApplyFinalMovement(); // Apply gravity if she's in the air
            return;
        }

        ReadInput();
        GroundCheck();
        WallCheck();

        if (isDashing)
        {
            UpdateDash();
        }
        else if (isClimbing)
        {
            UpdateClimb();
        }
        else
        {
            UpdateMovement();
            UpdateJump();
            UpdateDashInput();
        }

        ApplyGravity();
        ApplyFinalMovement();
        UpdateFacing();
        UpdateAnimator();
    }

    void ReadInput()
    {
        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump")) { jumpPressed = true; lastJumpPressTime = Time.time; }
        jumpHeld = Input.GetButton("Jump");
        dashPressed = Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.RightShift);
        grabHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(0);
    }

    void GroundCheck()
    {
        Vector3 checkPos = transform.position + Vector3.down * groundCheckOffset;
        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundLayer);
        if (isGrounded) lastGroundedTime = Time.time;
    }

    void WallCheck()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        isTouchingWall = Physics.Raycast(origin, transform.forward, out RaycastHit hit, wallCheckDistance, wallLayer);

        if (isTouchingWall) wallNormal = hit.normal;

        if (isTouchingWall && grabHeld && !isGrounded)
        {
            isClimbing = true;
            velocity.y = 0;
        }
        else
        {
            isClimbing = false;
        }
    }

    void UpdateMovement()
    {
        Vector3 moveInput = new Vector3(inputH, 0, inputV).normalized;
        float targetSpeed = moveInput.magnitude * moveSpeed;
        float accel = isGrounded ? (moveInput.magnitude > 0.1f ? acceleration : deceleration) : airAcceleration;

        currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, targetSpeed, accel * Time.deltaTime);

        velocity.x = moveInput.x * currentHorizontalSpeed;
        velocity.z = moveInput.z * currentHorizontalSpeed;
    }

    void UpdateJump()
    {
        if ((Time.time - lastJumpPressTime < jumpBufferTime) && (isGrounded || Time.time - lastGroundedTime < coyoteTime))
        {
            velocity.y = jumpForce;
            lastJumpPressTime = -100f;
        }
        else if (jumpPressed && isTouchingWall && !isGrounded)
        {
            velocity.y = wallJumpUpForce;
            velocity += wallNormal * wallJumpForce;
            lastJumpPressTime = -100f;
        }

        if (!jumpHeld && velocity.y > 0) velocity.y *= 0.5f;
        jumpPressed = false;
    }

    void UpdateDashInput()
    {
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (dashPressed && canDash && dashCooldownTimer <= 0) StartDash();
    }

    void StartDash()
    {
        if (isDashing) return; // Prevent double dashing

        StartCoroutine(DashSequence());
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        if (dashTrail) dashTrail.emitting = true;

        Vector3 dir = new Vector3(inputH, inputV, 0); // Logic for 8-way dash
        if (dir.magnitude < 0.1f) dir = transform.forward;
        dashDirection = dir.normalized;
        velocity = dashDirection * dashSpeed;

        StartCoroutine(DashHitstop());
    }
    IEnumerator DashSequence()
    {
        isDashing = true;

        // 1. Enable Trail only now
        if (dashTrail != null)
        {
            dashTrail.emitting = true;
        }

        // 2. Tutorial Secret: Reset velocity for a clean burst
        velocity = dashDirection * dashSpeed;

        // 3. Optional: Camera FOV shift (as seen in tutorial)
        // Camera.main.fieldOfView = 85f; 

        yield return new WaitForSeconds(dashDuration);

        // 4. End Dash
        isDashing = false;

        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        // 5. Momentum: Don't stop dead, bleed off speed slowly
        velocity *= 0.5f;
        // Camera.main.fieldOfView = 60f;
    }

    IEnumerator DashHitstop()
    {
        Time.timeScale = 0.05f;
        animator.speed = 0;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1f;
        animator.speed = 1;
    }

    void UpdateDash()
    {
        dashTimer -= Time.deltaTime;
        velocity = dashDirection * dashSpeed;
        if (dashTimer <= 0)
        {
            isDashing = false;
            velocity *= 0.4f;
            if (dashTrail) dashTrail.emitting = false;
        }
    }

    void UpdateClimb()
    {
        // 1. Freeze horizontal movement while climbing
        velocity.x = 0;
        velocity.z = 0;

        // 2. Only move vertically if W or Up Arrow is pressed
        // We use inputV which you already capture in ReadInput()
        velocity.y = inputV * climbSpeed;

        // 3. Wall Jump logic (Check if Space is pressed while climbing)
        if (jumpPressed)
        {
            velocity.y = wallJumpUpForce;
            velocity += wallNormal * wallJumpForce;
            isClimbing = false;
            return;
        }

        // 4. Let go if the grab key (Shift) is released
        if (!grabHeld)
        {
            isClimbing = false;
        }
    }

    void ApplyGravity()
    {
        if (isDashing || isClimbing) return;
        float grav = (velocity.y < 0) ? gravity * fallGravityMultiplier : gravity;
        velocity.y -= grav * Time.deltaTime;
        if (isTouchingWall && velocity.y < -wallSlideSpeed) velocity.y = -wallSlideSpeed;
        velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
    }

    void UpdateFacing()
    {
        if (isDashing || isClimbing) return;
        Vector3 faceDir = new Vector3(velocity.x, 0, velocity.z);
        if (faceDir.magnitude > 0.2f)
        {
            Quaternion targetRot = Quaternion.LookRotation(faceDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }

    void ApplyFinalMovement() => cc.Move(velocity * Time.deltaTime);

    void UpdateAnimator()
    {
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsClimbing", isClimbing);
        animator.SetBool("IsDashing", isDashing);
        animator.SetFloat("VerticalVelocity", velocity.y);
        animator.SetFloat("InputX", inputH);
        animator.SetFloat("InputZ", inputV);
        animator.SetFloat("Speed", new Vector3(velocity.x, 0, velocity.z).magnitude);

        animator.SetBool("IsClimbing", isClimbing);

        if (isClimbing)
        {
            // We calculate 'ClimbSpeed' based on whether W/S or Up/Down is held
            // If inputV is 0, ClimbSpeed becomes 0, which freezes the animation frame
            float animationPlaybackSpeed = Mathf.Abs(inputV) > 0.1f ? 1.0f : 0.0f;
            animator.SetFloat("ClimbSpeed", animationPlaybackSpeed);
        }
    }
    public void SetInputActive(bool active)
    {
        inputActive = active;
        if (!active)
        {
            inputH = 0;
            inputV = 0;
            jumpPressed = false;
            dashPressed = false;
            grabHeld = false;
            velocity = Vector3.zero;
            currentHorizontalSpeed = 0f;
        }
    }
    // This satisfies the EndTrigger's request and forces an Idle state
    public void PlayBreathingAnimation()
    {
        // First, turn off player control
        SetInputActive(false);

        // Reset velocity and animation parameters to force the Idle state
        velocity = Vector3.zero;
        currentHorizontalSpeed = 0f;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("InputX", 0f);
            animator.SetFloat("InputZ", 0f);
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsClimbing", false);
            animator.SetBool("IsDashing", false);
        }
    }
    public void UnlockDash()
    {
        canDash = true;

        // Optional: Reset dash cooldown so they can use it immediately
        dashCooldownTimer = 0f;

        Debug.Log("Dash Ability Unlocked!");
    }
}
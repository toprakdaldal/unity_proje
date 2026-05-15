using UnityEngine;

<<<<<<< HEAD
[RequireComponent(typeof(Rigidbody2D))]
=======
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("── Hareket ──")]
    [SerializeField] float moveSpeed       = 8f;
    [SerializeField] float acceleration    = 14f;
    [SerializeField] float deceleration    = 18f;
    [SerializeField] float airAcceleration = 7f;
    [SerializeField] float airDeceleration = 5f;

    [Header("── Zıplama ──")]
    [SerializeField] float jumpForce         = 17f;
    [SerializeField] float doubleJumpForce   = 14f;
    [SerializeField] float wallJumpForceX    = 10f;
    [SerializeField] float wallJumpForceY    = 16f;
    [SerializeField] float wallJumpLockTime  = 0.2f;
    [SerializeField] float jumpCutMultiplier = 0.45f;
    [SerializeField] float fallMultiplier    = 2.8f;
    [SerializeField] float coyoteTime        = 0.12f;
    [SerializeField] float jumpBufferTime    = 0.15f;

    [Header("── Dash ──")]
<<<<<<< HEAD
    [SerializeField] float dashSpeed      = 24f;
    [SerializeField] float dashDuration   = 0.18f;
    [SerializeField] float dashCooldown   = 0.55f;
=======
    [SerializeField] float dashSpeed     = 24f;
    [SerializeField] float dashDuration  = 0.18f;
    [SerializeField] float dashCooldown  = 0.55f;
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    [SerializeField] bool  dashInvincible = true;

    [Header("── Wall Slide ──")]
    [SerializeField] float wallSlideSpeed = 1.8f;

    [Header("── Zemin / Duvar Kontrol ──")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheckRight;
    [SerializeField] Transform wallCheckLeft;
    [SerializeField] float groundCheckRadius = 0.15f;
    [SerializeField] float wallCheckDistance = 0.12f;

    [Header("── İlahi Ateş ──")]
    [SerializeField] float divineFireMax      = 100f;
    [SerializeField] float divineFireRecover  = 8f;
    [SerializeField] float divineFireCost     = 25f;
    [SerializeField] float divineModThreshold = 100f;

    [Header("── Görseller ──")]
    [SerializeField] ParticleSystem dashParticles;
    [SerializeField] ParticleSystem landParticles;
    [SerializeField] ParticleSystem doubleJumpParticles;
    [SerializeField] ParticleSystem divineAuraParticles;

    // Componentler
<<<<<<< HEAD
    Rigidbody2D    rb;
    Animator       anim;
    Collider2D     col;
    SpriteRenderer sr;
=======
    Rigidbody2D       rb;
    Animator          anim;
    CapsuleCollider2D col;
    SpriteRenderer    sr;

#if ENABLE_INPUT_SYSTEM
    PlayerInputActions inputActions;
#endif
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7

    // Durum değişkenleri
    float moveInput;
    bool  isFacingRight = true;

    bool isGrounded;
    bool wasGrounded;
    bool isTouchingWallRight;
    bool isTouchingWallLeft;
    bool isWallSliding;

    bool  canDoubleJump;
    float coyoteTimer;
    float jumpBufferTimer;
    bool  isJumping;
    bool  isWallJumping;
    float wallJumpTimer;

    bool isDashing;
    bool canDash = true;

    float divineFire;
    bool  isDivineMode;

    bool jumpPressed;
    bool jumpHeld;
    bool dashPressed;
    bool attackPressed;
    bool fireAttackPressed;

    // HUD için public getter'lar
    public float DivineFire    => divineFire;
    public float DivineFireMax => divineFireMax;
    public bool  IsGrounded    => isGrounded;
    public bool  IsDashing     => isDashing;

<<<<<<< HEAD
    // Animator hash'leri
=======
    // Animator hash'leri (performans için önbellek)
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    static readonly int H_Speed       = Animator.StringToHash("Speed");
    static readonly int H_IsGrounded  = Animator.StringToHash("IsGrounded");
    static readonly int H_IsFalling   = Animator.StringToHash("IsFalling");
    static readonly int H_IsDashing   = Animator.StringToHash("IsDashing");
    static readonly int H_IsWallSlide = Animator.StringToHash("IsWallSlide");
    static readonly int H_IsDivine    = Animator.StringToHash("IsDivineMode");
    static readonly int H_Jump        = Animator.StringToHash("IsJumping");
    static readonly int H_DoubleJump  = Animator.StringToHash("DoubleJump");
    static readonly int H_WallJump    = Animator.StringToHash("WallJump");
    static readonly int H_Land        = Animator.StringToHash("Land");
    static readonly int H_Attack      = Animator.StringToHash("Attack");
    static readonly int H_FireAttack  = Animator.StringToHash("FireAttack");
    static readonly int H_Hurt        = Animator.StringToHash("Hurt");
    static readonly int H_Death       = Animator.StringToHash("Death");

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
<<<<<<< HEAD
        col  = GetComponent<Collider2D>();
        sr   = GetComponent<SpriteRenderer>();

        divineFire = divineFireMax * 0.5f;
=======
        col  = GetComponent<CapsuleCollider2D>();
        sr   = GetComponent<SpriteRenderer>();

        divineFire = divineFireMax * 0.5f;

#if ENABLE_INPUT_SYSTEM
        inputActions = new PlayerInputActions();
#endif
    }

    void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM
        inputActions.Enable();
        inputActions.Player.Jump.performed       += _ => jumpPressed       = true;
        inputActions.Player.Jump.canceled        += _ => OnJumpReleased();
        inputActions.Player.Dash.performed       += _ => dashPressed       = true;
        inputActions.Player.Attack.performed     += _ => attackPressed     = true;
        inputActions.Player.FireAttack.performed += _ => fireAttackPressed = true;
#endif
    }

    void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM
        inputActions?.Disable();
#endif
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    }

    void Update()
    {
        ReadInput();
        CheckGrounded();
        CheckWalls();
        UpdateTimers();
        UpdateDivineFire();
        ProcessActions();
        UpdateAnimator();
        ClearFrameInput();
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        HandleMovement();
        HandleGravity();
        HandleWallSlide();
    }

    // ── INPUT ──────────────────────────────────────────────────

    void ReadInput()
    {
<<<<<<< HEAD
=======
#if ENABLE_INPUT_SYSTEM
        moveInput = inputActions.Player.Move.ReadValue<Vector2>().x;
        jumpHeld  = inputActions.Player.Jump.IsPressed();
#else
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpHeld  = Input.GetButton("Jump");

        if (Input.GetButtonDown("Jump"))                                  jumpPressed       = true;
        if (Input.GetKeyDown(KeyCode.LeftShift) ||
            Input.GetKeyDown(KeyCode.RightShift))                         dashPressed       = true;
        if (Input.GetKeyDown(KeyCode.Z))                                  attackPressed     = true;
        if (Input.GetKeyDown(KeyCode.X))                                  fireAttackPressed = true;
<<<<<<< HEAD
=======
#endif
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    }

    void ClearFrameInput()
    {
        jumpPressed       = false;
        dashPressed       = false;
        attackPressed     = false;
        fireAttackPressed = false;
    }

    // ── ACTION İŞLEME ─────────────────────────────────────────

    void ProcessActions()
    {
        if (jumpPressed)
        {
            jumpBufferTimer = jumpBufferTime;
            TryJump();
        }

        if (dashPressed && canDash && !isDashing)
            StartCoroutine(DashRoutine());

        if (attackPressed)
<<<<<<< HEAD
            SafeTrigger(H_Attack);

        if (fireAttackPressed && divineFire >= divineFireCost)
        {
            SafeTrigger(H_FireAttack);
=======
            anim.SetTrigger(H_Attack);

        if (fireAttackPressed && divineFire >= divineFireCost)
        {
            anim.SetTrigger(H_FireAttack);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
            divineFire = Mathf.Max(0f, divineFire - divineFireCost);
        }
    }

    // ── HAREKET ───────────────────────────────────────────────

    void HandleMovement()
    {
        if (isWallJumping) return;

        float target = moveInput * moveSpeed;
        float accel  = isGrounded
            ? (Mathf.Abs(target) > 0.01f ? acceleration    : deceleration)
            : (Mathf.Abs(target) > 0.01f ? airAcceleration : airDeceleration);

        rb.AddForceX((target - rb.linearVelocityX) * accel, ForceMode2D.Force);

        if      (moveInput >  0.01f && !isFacingRight) Flip();
        else if (moveInput < -0.01f &&  isFacingRight) Flip();
    }

    void HandleGravity()
    {
        if (isWallSliding)
        {
            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -wallSlideSpeed);
            return;
        }

        if (rb.linearVelocityY < 0f)
            rb.AddForceY(Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime,
                         ForceMode2D.Force);
        else if (rb.linearVelocityY > 0f && !jumpHeld)
            rb.AddForceY(Physics2D.gravity.y * jumpCutMultiplier * Time.fixedDeltaTime,
                         ForceMode2D.Force);
    }

    void HandleWallSlide()
    {
        bool touchingWall = ( isFacingRight && isTouchingWallRight) ||
                            (!isFacingRight && isTouchingWallLeft);

        isWallSliding = touchingWall
                     && !isGrounded
                     && rb.linearVelocityY < 0f
                     && Mathf.Abs(moveInput) > 0.1f;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 s = transform.localScale;
        s.x = isFacingRight ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        transform.localScale = s;
    }

    // ── ZIPPLAMA ──────────────────────────────────────────────

    void TryJump()
    {
<<<<<<< HEAD
        if (isWallSliding)                  { DoWallJump();            return; }
        if (isGrounded || coyoteTimer > 0f) { DoJump(jumpForce); canDoubleJump = true; return; }
=======
        if (isWallSliding)                        { DoWallJump();            return; }
        if (isGrounded || coyoteTimer > 0f)       { DoJump(jumpForce); canDoubleJump = true; return; }
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
        if (canDoubleJump)
        {
            DoJump(doubleJumpForce);
            canDoubleJump = false;
<<<<<<< HEAD
            SafeTrigger(H_DoubleJump);
=======
            anim.SetTrigger(H_DoubleJump);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
            if (doubleJumpParticles != null) doubleJumpParticles.Play();
        }
    }

    void DoJump(float force)
    {
        rb.linearVelocityY = force;
        isJumping          = true;
        coyoteTimer        = 0f;
        jumpBufferTimer    = 0f;
<<<<<<< HEAD
        SafeTrigger(H_Jump);
=======
        anim.SetTrigger(H_Jump);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    }

    void DoWallJump()
    {
        float dir         = isTouchingWallRight ? -1f : 1f;
        rb.linearVelocity = new Vector2(wallJumpForceX * dir, wallJumpForceY);
        isWallJumping     = true;
        wallJumpTimer     = wallJumpLockTime;
        canDoubleJump     = true;
        if ((dir > 0 && !isFacingRight) || (dir < 0 && isFacingRight)) Flip();
<<<<<<< HEAD
        SafeTrigger(H_WallJump);
=======
        anim.SetTrigger(H_WallJump);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    }

    void OnJumpReleased()
    {
        if (rb.linearVelocityY > 0f && isJumping)
            rb.linearVelocityY *= 0.5f;
    }

    // ── DASH ──────────────────────────────────────────────────

    System.Collections.IEnumerator DashRoutine()
    {
        canDash   = false;
        isDashing = true;

        float dir         = moveInput != 0f ? Mathf.Sign(moveInput) : (isFacingRight ? 1f : -1f);
        float prevGravity = rb.gravityScale;
        rb.gravityScale   = 0f;
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);

<<<<<<< HEAD
=======
        anim.SetBool(H_IsDashing, true);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
        if (dashInvincible)        SetInvincible(true);
        if (dashParticles != null) dashParticles.Play();

        yield return new WaitForSeconds(dashDuration);

<<<<<<< HEAD
        rb.gravityScale     = prevGravity;
        rb.linearVelocityX *= 0.35f;
        isDashing           = false;

=======
        rb.gravityScale    = prevGravity;
        rb.linearVelocityX *= 0.35f;
        isDashing           = false;

        anim.SetBool(H_IsDashing, false);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
        if (dashInvincible)        SetInvincible(false);
        if (dashParticles != null) dashParticles.Stop();

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // ── ZEMIN / DUVAR ─────────────────────────────────────────

    void CheckGrounded()
    {
        wasGrounded = isGrounded;
        isGrounded  = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            coyoteTimer   = coyoteTime;
            canDoubleJump = true;
            canDash       = true;
            if (!wasGrounded) OnLand();
        }
        if (!isGrounded && wasGrounded) isJumping = false;
    }

    void OnLand()
    {
        isJumping = false;
<<<<<<< HEAD
        SafeTrigger(H_Land);
=======
        anim.SetTrigger(H_Land);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
        if (landParticles != null) landParticles.Play();
    }

    void CheckWalls()
    {
        isTouchingWallRight = Physics2D.Raycast(
            wallCheckRight.position, Vector2.right, wallCheckDistance, wallLayer);
        isTouchingWallLeft  = Physics2D.Raycast(
            wallCheckLeft.position,  Vector2.left,  wallCheckDistance, wallLayer);
    }

    // ── TIMER'LAR ─────────────────────────────────────────────

    void UpdateTimers()
    {
        coyoteTimer     = Mathf.Max(0f, coyoteTimer     - Time.deltaTime);
        jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - Time.deltaTime);

        if (jumpBufferTimer > 0f && isGrounded && !isJumping)
        {
            TryJump();
            jumpBufferTimer = 0f;
        }

        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0f) isWallJumping = false;
        }
    }

    // ── İLAHİ ATEŞ ────────────────────────────────────────────

    void UpdateDivineFire()
    {
        divineFire = Mathf.Clamp(
            divineFire + divineFireRecover * Time.deltaTime, 0f, divineFireMax);

        bool shouldBeDivine = divineFire >= divineModThreshold;
        if (shouldBeDivine == isDivineMode) return;

        isDivineMode = shouldBeDivine;
<<<<<<< HEAD
=======
        anim.SetBool(H_IsDivine, isDivineMode);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7

        if (divineAuraParticles == null) return;
        if (isDivineMode) divineAuraParticles.Play();
        else              divineAuraParticles.Stop();
    }

<<<<<<< HEAD
=======
    // CombatController tarafından çağrılır (düşman öldürünce)
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    public void AddDivineFire(float amount) =>
        divineFire = Mathf.Clamp(divineFire + amount, 0f, divineFireMax);

    // ── ANİMATOR ──────────────────────────────────────────────

    void UpdateAnimator()
    {
<<<<<<< HEAD
        if (isDashing) return;

        if (isGrounded)
        {
            if (Mathf.Abs(rb.linearVelocityX) > 0.1f)
                anim.Play("Player_Run");
            else
                anim.Play("Player_Idle");
        }
        else if (isWallSliding)
        {
            anim.Play("Player_Idle");
        }
        else
        {
            if (rb.linearVelocityY > 0.1f)
                anim.Play("Player_Jump");
            else
                anim.Play("Player_fall");
        }
=======
        anim.SetFloat(H_Speed,       Mathf.Abs(rb.linearVelocityX));
        anim.SetBool (H_IsGrounded,  isGrounded);
        anim.SetBool (H_IsFalling,   rb.linearVelocityY < -0.1f && !isGrounded);
        anim.SetBool (H_IsWallSlide, isWallSliding);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    }

    // ── HASAR / ÖLÜM ──────────────────────────────────────────

    public void TakeDamage(float knockbackForce = 6f)
    {
        if (isDashing && dashInvincible) return;
<<<<<<< HEAD
        SafeTrigger(H_Hurt);
=======
        anim.SetTrigger(H_Hurt);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
        float dir = isFacingRight ? -1f : 1f;
        rb.linearVelocity = new Vector2(knockbackForce * dir, 4f);
    }

    public void Die()
    {
<<<<<<< HEAD
        SafeTrigger(H_Death);
=======
        anim.SetTrigger(H_Death);
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale   = 0f;
        col.enabled       = false;
        enabled           = false;
<<<<<<< HEAD
=======
#if ENABLE_INPUT_SYSTEM
        inputActions?.Disable();
#endif
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    }

    // ── YARDIMCI ──────────────────────────────────────────────

<<<<<<< HEAD
    void SafeTrigger(int hash)
    {
        foreach (var p in anim.parameters)
            if (p.nameHash == hash && p.type == AnimatorControllerParameterType.Trigger)
            { anim.SetTrigger(hash); return; }
    }

=======
>>>>>>> 3b69d0752b8c4aea98770e5503f477579eb912c7
    void SetInvincible(bool state)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, state);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheckRight != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(wallCheckRight.position, Vector2.right * wallCheckDistance);
            Gizmos.DrawRay(wallCheckLeft.position,  Vector2.left  * wallCheckDistance);
        }
    }
}

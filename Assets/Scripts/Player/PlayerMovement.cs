
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask movingPlatform;

    [SerializeField] private float maxVel;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;

    [SerializeField] private ParticleSystem walkParticles;
    private bool isPlayingWalkParticles = false;

    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator animator;

    [SerializeField] private float jumpCutMultiplier;

    [Header("Helpers")]

    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffer;
    float coyoteTimeCounter = -1;
    float jumpBufferCounter = -1;
    float jumpCooldownCounter = -1;

    float frictionDisabledTime;

    SFXManager sfxManager;
    bool isGrounded;

    [Header("Wall Slide")]
    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public Vector2 wallCheckSize;
    public float wallSlideSpeed;
    float wallSlideDir = 0;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce;
    public float wallJumpForgiveness;
    float wallForgivenessCounter;

    public bool isWallSliding { get { return wallSlideDir != 0; } }


    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = StatsManager.Instance.GetStat(StatType.Speed);
        sr.sprite = GameManager.Instance.character.sprite;
    }

    void HandleFlip()
    {
        if (InputManager.Instance.HorizontalInput != 0)
        {
            bool isFacingRight = InputManager.Instance.HorizontalInput > 0;
            sr.flipX = !isFacingRight;
            walkParticles.transform.rotation = Quaternion.Euler(0, 0, isFacingRight ? 180 : 0);
        }
    }

    void HandleWalkParticles()
    {
        if (isGrounded && !isPlayingWalkParticles && InputManager.Instance.HorizontalInput != 0)
        {
            walkParticles.Play();
            isPlayingWalkParticles = true;
        }

        if ((!isGrounded || InputManager.Instance.HorizontalInput == 0) && isPlayingWalkParticles)
        {
            walkParticles.Stop();
            isPlayingWalkParticles = false;
        }
    }

    void HandleAnimations()
    {
        /*
        State 0 - Idle
        State 1 - Walking
        */

        int state = 0;
        if (InputManager.Instance.HorizontalInput != 0 && isGrounded)
        {
            state = 1;
        }
        else
        {
            state = 0;
        }

        animator.SetInteger("State", state);
    }

    void HandleJump()
    {

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (!isGrounded && rb.linearVelocity.y > 0f && InputManager.Instance.JumpInputReleased && frictionDisabledTime < 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (InputManager.Instance.JumpInput)
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && jumpCooldownCounter < 0f)
        {
            jumpCooldownCounter = jumpBuffer + 0.1f;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            sfxManager.PlaySFX(1);
        }

    }

    void HandleWallSlide()
    {
        bool touchingLeftWall = Physics2D.OverlapBox(leftWallCheck.position, wallCheckSize, 0f, groundLayer);
        bool touchingRightWall = Physics2D.OverlapBox(rightWallCheck.position, wallCheckSize, 0f, groundLayer);

        if (touchingLeftWall && InputManager.Instance.HorizontalInput < 0)
        {
            wallSlideDir = -1;
        }
        else if (touchingRightWall && InputManager.Instance.HorizontalInput > 0)
        {
            wallSlideDir = 1;
        }
        else
        {
            wallSlideDir = 0;
        }

        if (wallSlideDir != 0 && !isGrounded && rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
            wallForgivenessCounter = wallJumpForgiveness;
        }
        else
        {
            wallForgivenessCounter -= Time.deltaTime;
        }
    }

    void HandleWallJump()
    {
        if (wallForgivenessCounter > 0f && InputManager.Instance.JumpInput)
        {

            rb.linearVelocity = new Vector2(-wallSlideDir * wallJumpForce.x, wallJumpForce.y);
            wallForgivenessCounter = 0f;
            sfxManager.PlaySFX(1);
        }
    }

    void Update()
    {
        isGrounded = IsGrounded();
        frictionDisabledTime -= Time.deltaTime;
        HandleFlip();
        HandleWalkParticles();
        HandleJump();
        HandleAnimations();
        HandleWallSlide();
        HandleWallJump();
        Collider2D movingPlat = IsOnMovingPlatform();

        if (movingPlat != null)
        {
            transform.SetParent(movingPlat.transform);
        }
        else
        {
            transform.SetParent(null);
        }
        jumpCooldownCounter -= Time.deltaTime;





        if (frictionDisabledTime < 0f && rb.linearVelocity.magnitude > maxVel)
        {
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxVel);
        }
    }

    public void DisableFriction(float time)
    {
        frictionDisabledTime = time;
    }

    void FixedUpdate()
    {
        if (frictionDisabledTime >= 0f) return;

        float targetSpeed = Input.GetAxisRaw("Horizontal") * moveSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : decceleration;

        float movement = Mathf.Abs(speedDif) * accelRate * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            FindFirstObjectByType<RankManager>().Continue();
        }
    }

    Collider2D IsOnMovingPlatform()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, movingPlatform);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
    }
}

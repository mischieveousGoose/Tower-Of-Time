
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    Rigidbody2D rb;
    public Transform groundCheck;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;
    public LayerMask movingPlatform;

    public float maxVel;
    public float jumpHeight;
    public float acceleration;
    public float decceleration;

    public ParticleSystem walkParticles;
    bool isPlayingWalkParticles = false;

    public SpriteRenderer sr;

    public float jumpCutMultiplier;

    [Header("Helpers")]

    public float coyoteTime;
    public float jumpBuffer;
    float coyoteTimeCounter;
    float jumpBufferCounter;
    float jumpCooldownCounter;

    [Header("Extra Jumps")]
    float frictionDisabledTime;

    SFXManager sfxManager;
    bool isGrounded;



    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        moveSpeed = StatsManager.Instance.GetStat(StatType.Speed);
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

        if (!isGrounded && rb.linearVelocity.y > 0f && InputManager.Instance.JumpInput && frictionDisabledTime < 0f)
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
    }

    void Update()
    {
        isGrounded = IsGrounded();
        frictionDisabledTime -= Time.deltaTime;
        HandleFlip();
        HandleWalkParticles();
        HandleJump();

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




        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && jumpCooldownCounter < 0f)
        {
            jumpCooldownCounter = jumpBuffer + 0.1f;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            sfxManager.PlaySFX(1);
        }

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

        if (collision.gameObject.CompareTag("Fight"))
        {
            FindFirstObjectByType<DialogueManager>().StartDialogue();
            Destroy(collision.gameObject);
        }
    }

    Collider2D IsOnMovingPlatform()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, movingPlatform);
    }
}
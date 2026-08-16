
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



    SpriteRenderer sr;

    public float jumpCutMultiplier;

    [Header("Helpers")]

    public float coyoteTime;
    public float jumpBuffer;
    float coyoteTimeCounter;
    float jumpBufferCounter;
    float jumpCooldownCounter;

    [Header("Extra Jumps")]
    private int extraJumps = 0;
    int extraJumpsRemaining;

    PlayerCombat attacker;

    bool isFlipped = false;
    bool canFlip;

    float frictionDisabledTime;

    SFXManager sfxManager;

    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        attacker = GetComponent<PlayerCombat>();

        extraJumps = 0;
        moveSpeed = StatsManager.Instance.GetStat(StatType.Speed);
        extraJumpsRemaining = extraJumps;

        canFlip = false;


    }

    void Update()
    {
        frictionDisabledTime -= Time.deltaTime;
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            sr.flipX = Input.GetAxisRaw("Horizontal") < -0.1f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canFlip)
        {
            isFlipped = !isFlipped;
        }

        if (isFlipped)
        {
            sr.flipY = true;
            rb.gravityScale = Mathf.Abs(rb.gravityScale) * -1;
            groundCheck.localPosition = new Vector3(groundCheck.localPosition.x, Mathf.Abs(groundCheck.localPosition.y), 0f);
        }
        else
        {
            sr.flipY = false;
            rb.gravityScale = Mathf.Abs(rb.gravityScale);
            groundCheck.localPosition = new Vector3(groundCheck.localPosition.x, Mathf.Abs(groundCheck.localPosition.y) * -1, 0f);
        }

        Collider2D movingPlat = isOnMovingPlatform();

        if (movingPlat != null)
        {
            transform.SetParent(movingPlat.transform);
        }
        else
        {
            transform.SetParent(null);
        }
        jumpCooldownCounter -= Time.deltaTime;

        if (isGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            extraJumpsRemaining = extraJumps; // refill extra jumps whenever grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (!isGrounded() && rb.linearVelocity.y > 0f && Input.GetButtonUp("Jump") && frictionDisabledTime < 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        bool canGroundJump = coyoteTimeCounter > 0f;
        bool canAirJump = !canGroundJump && extraJumpsRemaining > 0;

        if ((canGroundJump || canAirJump) && jumpBufferCounter > 0f && jumpCooldownCounter < 0f)
        {
            jumpCooldownCounter = jumpBuffer + 0.1f;
            jumpBufferCounter = 0f; // consume the buffered input so it can't trigger twice

            if (!canGroundJump)
            {
                extraJumpsRemaining--;
                coyoteTimeCounter = 0f; // prevent a leftover coyote frame from also counting
            }

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

    public bool isGrounded()
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

    Collider2D isOnMovingPlatform()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, movingPlatform);
    }
}
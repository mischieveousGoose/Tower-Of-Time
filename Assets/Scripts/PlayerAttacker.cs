using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    public int damage;
    public float attackVel;
    public float weaponDistance;

    public float attackTime;
    int dashesLeft = 0;
    PlayerMovement movement;
    int airDashes;
    public float attackCooldown;

    PlayerItems items;


    public GameObject weapon;
    Damager wpDamager;

    float attackTimer;
    Camera cam;

    Rigidbody2D rb;

    public bool mouseToAimMode;

    public bool isAttacking => attackTimer > attackCooldown;
    SFXManager sfxManager;

    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();
        items = FindFirstObjectByType<PlayerItems>();
        wpDamager = weapon.GetComponent<Damager>();
        wpDamager.damage *= 1 + (items.CountItem("Sharper Blade") * .25f);

        movement = GetComponent<PlayerMovement>();
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        airDashes = 1 + items.CountItem("Steroids");
    }

    void Update()
    {
        if (movement.isGrounded())
        {
            dashesLeft = airDashes;
        }
        attackTimer -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && dashesLeft > 0 && attackTimer < 0)
        {
            Attack();
        }

        weapon.SetActive(isAttacking);
    }

    void Attack()
    {
        sfxManager.PlaySFX(0);
        attackTimer = attackTime + attackCooldown;
        dashesLeft--;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        float angle;
        Vector2 direction;

        if (mouseToAimMode)
        {
            direction = SnapTo45Degrees(mouseWorld - transform.position, out angle);
        }
        else
        {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
        }

        rb.linearVelocity = (direction * attackVel);

        weapon.transform.rotation = Quaternion.Euler(0, 0, angle);
        weapon.transform.localPosition = (weaponDistance * direction);
    }

    public Vector2 SnapTo45Degrees(Vector2 direction, out float angle)
    {
        angle = 90;
        if (direction == Vector2.zero)
            return Vector2.right;

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 45f) * 45f;

        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }
}

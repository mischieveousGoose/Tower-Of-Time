using Unity.VisualScripting;
using UnityEngine;

public class Sword : Weapon
{
    private PlayerMovement movement;
    private PlayerHealth health;
    private Rigidbody2D rb;

    private bool canDash;
    private float regainDashCooldown;

    public float dashVelocity;
    public GameObject swordPrefab;
    public float attackTime;

    protected override void WeaponUpdate()
    {
        regainDashCooldown -= Time.deltaTime;
        if (movement.IsGrounded() && regainDashCooldown < 0f)
        {
            canDash = true;
        }
    }

    void Start()
    {
        movement = player.GetComponent<PlayerMovement>();
        rb = player.GetComponent<Rigidbody2D>();
        health = player.GetComponent<PlayerHealth>();
    }

    protected override void OnAttack()
    {
        if (!canDash) return;

        canDash = false;
        regainDashCooldown = 0.03f;

        Vector2 direction = GetAimingDirection();
        direction = SnapToDegrees(direction, 45f, out float angle);

        rb.linearVelocity = direction * dashVelocity;
        movement.DisableFriction(attackTime);

        GameObject weapon = Instantiate(swordPrefab, transform);

        weapon.transform.rotation = Quaternion.Euler(0, 0, angle);
        weapon.transform.localPosition = 1.5f * direction;
        weapon.GetComponent<Damager>().damage = damage;

        health.SetIFrames(attackTime);

        Destroy(weapon, attackTime);
    }
}

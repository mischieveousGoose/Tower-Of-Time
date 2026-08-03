using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Weapon : MonoBehaviour
{
    public float damage;
    public float attackCooldown;

    private float attackTimer;
    protected bool canAttack => attackTimer <= 0;

    private Camera cam;
    protected GameObject player;


    private void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }

    private void OnEnable()
    {
        damage *= StatsManager.Instance.GetStat(StatType.Damage);
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnUse()
    {
        if (!canAttack)
            return;

        attackTimer = attackCooldown;
        OnAttack();
    }

    protected virtual void OnAttack()
    {

    }

    public Vector2 SnapToDegrees(Vector2 direction, float snapAngle, out float angle)
    {
        angle = 90;
        if (direction == Vector2.zero)
            return Vector2.right;

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / snapAngle) * snapAngle;

        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }

    public Vector2 GetAimingDirection()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        return (mousePos - transform.position).normalized;
    }
}

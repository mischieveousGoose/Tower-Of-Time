using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingHook : Weapon
{
    public LayerMask grappleLayer;
    public float maxDistance = 15f;
    public float pullSpeed = 30f;
    public float stopDistance = 0.5f;

    private Rigidbody2D rb;
    private LineRenderer lr;
    private Camera myCamera;

    private bool grappling;
    private Vector2 grapplePoint;



    void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        myCamera = Camera.main;

        lr.enabled = false;
    }

    public override void ReleaseAttack()
    {
        StopGrapple();
    }

    protected override void WeaponUpdate()
    {

        if (grappling)
        {
            lr.SetPosition(0, player.transform.position);
            lr.SetPosition(1, grapplePoint);
        }
    }

    void FixedUpdate()
    {
        if (!grappling)
            return;

        Vector2 direction = (grapplePoint - (Vector2)player.transform.position).normalized;
        rb.linearVelocity = direction * pullSpeed;

        if (Vector2.Distance(player.transform.position, grapplePoint) < stopDistance)
        {
            StopGrapple();
        }
    }

    protected override void OnAttack()
    {
        Vector2 mousePos = myCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)player.transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, maxDistance, grappleLayer);

        if (hit.collider != null)
        {
            grapplePoint = hit.point;
            grappling = true;
            lr.enabled = true;
        }
    }

    void StopGrapple()
    {
        grappling = false;
        lr.enabled = false;
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour
{
    public LayerMask grappleLayer;
    public float maxDistance = 15f;
    public float pullSpeed = 30f;
    public float stopDistance = 0.5f;

    private Rigidbody2D rb;
    private LineRenderer lr;
    private Camera cam;

    private bool grappling;
    private Vector2 grapplePoint;

    bool hasHook;

    void Start()
    {
        hasHook = FindFirstObjectByType<PlayerItems>().CountItem("Grappling Hook") > 0;
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        cam = Camera.main;

        lr.enabled = false;
    }

    void Update()
    {
        if (!hasHook) return;

        if (Input.GetMouseButtonDown(1))
            FireGrapple();

        if (Input.GetMouseButtonUp(1))
            StopGrapple();

        if (grappling)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, grapplePoint);
        }
    }

    void FixedUpdate()
    {
        if (!grappling)
            return;

        Vector2 direction = (grapplePoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * pullSpeed;

        if (Vector2.Distance(transform.position, grapplePoint) < stopDistance)
        {
            StopGrapple();
        }
    }

    void FireGrapple()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);

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
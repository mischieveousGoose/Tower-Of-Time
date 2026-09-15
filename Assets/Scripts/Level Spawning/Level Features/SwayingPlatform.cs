using UnityEngine;

public class SwayingPlatform : MonoBehaviour
{
    public Transform lineAnchor;
    LineRenderer lr;
    [Header("Sway settings")]
    public float swayDistance = 2f;   // how far it swings side to side (world units)
    public float swaySpeed = 1f;      // how fast it swings

    [Header("Arc shape (optional)")]
    public bool arcMotion = true;     // curves up at the edges like a pendulum, not just left-right
    public float arcHeight = 0.5f;    // how much it dips/rises during the swing

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, lineAnchor.position);
        float t = Time.time * swaySpeed;
        float x = Mathf.Sin(t) * swayDistance;

        float y = 0f;
        if (arcMotion)
        {
            // Mimics a pendulum's vertical dip: lowest at center, highest at swing extremes
            y = Mathf.Cos(t) * arcHeight;
        }

        transform.position = startPos + new Vector3(x, y, 0f);
    }
}
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 1f;
    public float moveSpeed = 1.5f;

    private TMP_Text text;
    private Color startColor;
    private float timer;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        startColor = text.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Move upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out
        float t = timer / lifetime;
        Color c = startColor;
        c.a = Mathf.Lerp(1f, 0f, t);
        text.color = c;

        // Destroy when finished
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string value)
    {
        text.text = value;
    }

    public void SetColor(Color color)
    {
        text.color = color;
        startColor = color;
    }
}
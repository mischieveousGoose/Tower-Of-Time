using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Camera cam;

    private Vector3 dragOrigin;

    public bool lockX;
    public bool lockY;

    public float zoomStep, minSize, maxSize;

    private void Update()
    {
        PanCamera();
        Zoom();
    }

    private void PanCamera()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            dragOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (lockX) difference.x = 0;
            if (lockY) difference.y = 0;
            difference.z = 0;

            cam.transform.position += difference;
        }
    }

    private void Zoom()
    {
        float zoomAmount = -Mouse.current.scroll.ReadValue().y * zoomStep * Time.deltaTime;
        float newSize = cam.orthographicSize + zoomAmount;
        cam.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
    }


}

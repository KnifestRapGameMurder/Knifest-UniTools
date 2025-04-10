using UnityEngine;

namespace Knifest.UniTools.Controllers
{
    public class SceneViewCameraController : MonoBehaviour
{
    public bool isUpdating = true;
    
    public float rotationSpeed = 3.0f;
    public float panSpeed = 0.5f;
    public float zoomSpeed = 5.0f;
    public float flySpeed = 5.0f;

    [Tooltip("If null -> Camera.main")] public Camera targetCamera;

    private Vector3 _rotationCenter; // Set a pivot point for rotating around the center
    private Vector3 _lastMousePosition;

    private bool _isOrthographic; // Track if the camera is in orthographic mode

    private Camera MainCamera
    {
        get
        {
            if (targetCamera is null) targetCamera = Camera.main;
            return targetCamera;
        }
    }

    private void Update()
    {
        if (isUpdating == false) return;

        if (!IsMouseInGameView()) return;

        // Switch between perspective and orthographic
        if (Input.GetKeyDown(KeyCode.O))
        {
            SwitchCameraMode();
        }

        HandleRotation();
        HandlePan();
        HandleZoom();
        HandleFly();
        _lastMousePosition = Input.mousePosition;
    }

    private void SwitchCameraMode()
    {
        if (_isOrthographic)
        {
            // Switch to perspective mode
            MainCamera.orthographic = false;

            // Adjust Field of View (FoV) to match the current orthographic size
            float aspect = MainCamera.aspect;
            float orthoSize = MainCamera.orthographicSize;
            float fov = 2f * Mathf.Atan(orthoSize / aspect) * Mathf.Rad2Deg;
            // MainCamera.fieldOfView = fov;
            MainCamera.fieldOfView = 60;

            // Set distance based on the current orthographic size (to maintain similar view)
            float distance = MainCamera.transform.position.y;
            MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, distance, MainCamera.transform.position.z);
        }
        else
        {
            // Switch to orthographic mode
            MainCamera.orthographic = true;

            // Adjust orthographic size to match the current field of view
            float fov = MainCamera.fieldOfView;
            float aspect = MainCamera.aspect;
            float orthoSize = Mathf.Tan(fov * Mathf.Deg2Rad / 2f) * 2f * Mathf.Abs(MainCamera.transform.position.y);
            MainCamera.orthographicSize = orthoSize;
        }

        _isOrthographic = !_isOrthographic;
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0))
        {
            var ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            _rotationCenter = groundPlane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
        }

        if (Input.GetMouseButton(1)) // Right mouse button for free rotation
        {
            var delta = Input.mousePosition - _lastMousePosition;
            var rotationX = delta.y * rotationSpeed * Time.deltaTime;
            var rotationY = delta.x * rotationSpeed * Time.deltaTime;

            transform.eulerAngles += new Vector3(-rotationX, rotationY, 0);
        }
        else if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0)) // Alt + Left mouse button for orbiting
        {
            var delta = Input.mousePosition - _lastMousePosition;
            var rotationX = delta.y * rotationSpeed * Time.deltaTime;
            var rotationY = delta.x * rotationSpeed * Time.deltaTime;

            // Rotate around the pivot point
            transform.RotateAround(_rotationCenter, Vector3.up, rotationY);
            transform.RotateAround(_rotationCenter, transform.right, -rotationX);
        }
    }

    private void HandlePan()
    {
        if (!Input.GetMouseButton(2) && (!Input.GetMouseButton(1) || !Input.GetKey(KeyCode.LeftShift))) return;
        var delta = Input.mousePosition - _lastMousePosition;
        var move = new Vector3(-delta.x * panSpeed * Time.deltaTime, -delta.y * panSpeed * Time.deltaTime, 0);
        transform.Translate(move, Space.Self);
    }

    private void HandleZoom()
    {
        if (_isOrthographic)
        {
            // Zoom by changing the orthographic size
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            MainCamera.orthographicSize -= scroll * zoomSpeed;
            MainCamera.orthographicSize = Mathf.Max(1f, MainCamera.orthographicSize); // Prevent going too small
        }
        else
        {
            // Zoom by moving the camera in and out
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            transform.position += transform.forward * (scroll * zoomSpeed);
        }
    }

    private void HandleFly()
    {
        if (!Input.GetMouseButton(1)) return; // Fly mode is active only when holding the right mouse button

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += transform.forward; // Forward
        if (Input.GetKey(KeyCode.S)) move -= transform.forward; // Backward
        if (Input.GetKey(KeyCode.A)) move -= transform.right; // Left
        if (Input.GetKey(KeyCode.D)) move += transform.right; // Right
        if (Input.GetKey(KeyCode.E)) move += transform.up; // Up
        if (Input.GetKey(KeyCode.Q)) move -= transform.up; // Down

        transform.position += move * (flySpeed * Time.deltaTime);
    }

    private static bool IsMouseInGameView()
    {
        var mousePosition = Input.mousePosition;
        return mousePosition.x >= 0 && mousePosition.x <= Screen.width &&
               mousePosition.y >= 0 && mousePosition.y <= Screen.height;
    }
}

}
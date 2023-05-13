using UnityEngine;

public class DragCam : MonoBehaviour
{
    public Transform target; // The object to rotate around
    public float sensitivity = 5.0f; // Mouse sensitivity
    public float distance = 5.0f; // Distance from target object
    public float minYAngle = -20.0f; // Minimum vertical angle
    public float maxYAngle = 80.0f; // Maximum vertical angle
    public float smoothTime = 0.1f; // Time taken to smoothly move to new position/rotation

    private float mouseX, mouseY;
    private float currentX, currentY;
    private float yVelocity = 0.0f;

    void Start()
    {
        // Set initial rotation to look at target object
        transform.LookAt(target.position);

        // Set initial rotation angles
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void Update()
    {
        // Check if right mouse button is pressed
        if (Input.GetMouseButton(1))
        {
            // Get mouse movement
            mouseX = Input.GetAxis("Mouse X") * sensitivity;
            mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            // Rotate camera
            currentX += mouseX;
            currentY -= mouseY;
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

            // Smoothly move camera to new position/rotation
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Mathf.SmoothDamp(0, 1, ref yVelocity, smoothTime));
            transform.position = Vector3.Lerp(transform.position, position, Mathf.SmoothDamp(0, 1, ref yVelocity, smoothTime));
        }
    }
}

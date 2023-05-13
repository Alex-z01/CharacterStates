using System.Threading;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public enum CamMode { FirstPerson, ThirdPerson, TopDown }

    private CamMode _mode;
    public CamMode Mode
    {
        get
        {
            return _mode;
        }
        set
        {
            _mode = value;
            OnModeChange(_mode);
        }
    }

    private Camera _cam;

    public GameObject playerObj;
    public GameObject firstPersonViewPoint;

    public float maxYAngle;

    [Header("First Person")]
    public float mouseSensitivity = 100f;

    [Header("Third Person")]
    public Transform target;
    public float thirdPersonDistance = 5.0f;
    public float thirdPersonHeight = 2.0f;
    public float rotationDamping = 3.0f;
    public float heightDamping = 2.0f;
    public float rotationOffset = 5f;
    [Header("Third Person Drag")]
    private float mouseX, mouseY;
    private float currentX, currentY;
    private float yVelocity = 0.0f;
    public float mouseSensitivityThirdPerson = 2f;
    public float smoothTime = 0.1f; // Time taken to smoothly move to new position/rotation

    [Header("Top Down")]
    public float topDownDistance, topDownHeight;

    private float _xRotation = 0f;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        Mode = CamMode.FirstPerson;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Mode = CamMode.FirstPerson;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Mode = CamMode.ThirdPerson;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Mode = CamMode.TopDown;
        }
    }

    private void LateUpdate()
    {
        CameraLogic();
    }

    private void CameraLogic()
    {
        if (Mode.Equals(CamMode.FirstPerson))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -maxYAngle, maxYAngle);

            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            playerObj.transform.Rotate(Vector3.up * mouseX);

            return;
        }

        if (Mode.Equals(CamMode.ThirdPerson))
        {
            if (Input.GetMouseButton(1))
            {
                // Get mouse movement
                mouseX = Input.GetAxis("Mouse X") * mouseSensitivityThirdPerson;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityThirdPerson;

                // Rotate camera
                currentX += mouseX;
                currentY -= mouseY;
                currentY = Mathf.Clamp(currentY, -maxYAngle, maxYAngle);

                // Smoothly move camera to new position/rotation
                Quaternion targetRotation = Quaternion.Euler(currentY, currentX, 0);
                Vector3 targetPos = targetRotation * new Vector3(0.0f, 0.0f, -thirdPersonDistance) + target.position;

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.SmoothDamp(0, 1, ref yVelocity, smoothTime));
                transform.position = Vector3.Lerp(transform.position, targetPos, Mathf.SmoothDamp(0, 1, ref yVelocity, smoothTime));

                return;
            }

            Vector3 angles = transform.eulerAngles;
            currentX = angles.y;
            currentY = angles.x;

            // Calculate the current rotation angles
            float wantedRotationAngle = target.eulerAngles.y + rotationOffset;
            float wantedHeight = target.position.y + thirdPersonHeight;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            // Smoothly rotate the camera towards the target's rotation
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Smoothly move the camera towards the target's height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Calculate the current position of the camera based on the target's position, distance, and height
            Vector3 targetPosition = target.position - Quaternion.Euler(0, currentRotationAngle, 0) * Vector3.forward * thirdPersonDistance;
            targetPosition.y = currentHeight;

            // Set the camera's position and rotation to the calculated values
            transform.position = targetPosition;
            transform.LookAt(target);

            return;
        }

        if (Mode.Equals(CamMode.TopDown))
        {
            float wantedHeight = target.position.y + topDownHeight;

            float currentHeight = transform.position.y;

            // Smoothly move the camera towards the target's height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Calculate the current position of the camera based on the target's position, distance, and height
            Vector3 targetPosition = target.position - Vector3.forward * topDownDistance;
            targetPosition.y = currentHeight;

            // Set the camera's position and rotation to the calculated values
            transform.position = targetPosition;
            transform.LookAt(target);
        }
    }

    private void OnModeChange(CamMode mode)
    {
        if (mode.Equals(CamMode.FirstPerson))
        {
            _cam.transform.SetParent(playerObj.transform, false);
            _cam.transform.position = firstPersonViewPoint.transform.position;
            _cam.transform.localRotation = Quaternion.identity;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        if (mode.Equals(CamMode.ThirdPerson))
        {
            _cam.transform.parent = null;

            transform.LookAt(target.position);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (mode.Equals(CamMode.TopDown))
        {
            _cam.transform.parent = null;

            transform.LookAt(target.position);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

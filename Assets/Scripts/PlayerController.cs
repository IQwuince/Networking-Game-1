using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Jump / Gravity")]
    [SerializeField] private float jumpHeight = 1.6f;
    [SerializeField] private float gravity = -20f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 120f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 70f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;

    private CharacterController controller;
    private float yaw;
    private float pitch;
    private float verticalVelocity;
    private bool cursorLocked = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        if (playerCamera != null) playerCamera.gameObject.SetActive(IsOwner);
        if (!IsOwner) return;

        yaw = transform.eulerAngles.y;
        SetCursorLock(true);
    }

    private void OnDisable()
    {
        if (IsOwner) SetCursorLock(false);
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursorLock(!cursorLocked);

        // Only rotate view when locked
        if (cursorLocked)
            HandleMouseLook();

        // Always run movement/jump so gravity keeps working
        HandleMovementAndJump();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity * Time.deltaTime;
        pitch -= mouseY * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovementAndJump()
    {
        // If cursor unlocked, block horizontal movement but keep gravity
        float x = cursorLocked ? Input.GetAxisRaw("Horizontal") : 0f;
        float z = cursorLocked ? Input.GetAxisRaw("Vertical") : 0f;

        Vector3 input = new Vector3(x, 0f, z).normalized;
        Vector3 move = (transform.right * input.x + transform.forward * input.z) * moveSpeed;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        // Jump only when locked (optional choice)
        if (cursorLocked && controller.isGrounded && Input.GetButtonDown("Jump"))
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        if (animator != null)
            animator.SetFloat("movement", input.sqrMagnitude > 0.01f ? 1f : 0f);
    }

    private void SetCursorLock(bool shouldLock)
    {
        cursorLocked = shouldLock;
        Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !shouldLock;
    }
}
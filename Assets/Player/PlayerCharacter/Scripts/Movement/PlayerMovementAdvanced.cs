using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using Unity.Netcode;

public class PlayerMovementAdvanced : NetworkBehaviour
{
    [Header("Movement")]
    public float desiredMoveSpeed;
    public float walkSpeed = 7;
    private float lastDesiredMoveSpeed;
    public float moveSpeed;
    

    [Header("bs")]
    public float climbSpeed;
    public float vaultSpeed;

    [Header("sliding speed")]
    public float slideGroundSpeed;
    public float slideSpeed;

    [Header("Wall Running speed")]
    public float wallrunSpeedIncrease;
    public float wallrunSpeed;

    public float hardSpeedCap;
    public float velRevertingSpeed = 2f;
    public float groundDrag;
    public float airMinSpeed;

    [Header("slope")]
    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Dashing")]
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public float maxYSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    public Climbing climbingScript;
    private ClimbingDone climbingScriptDone;
    //public PlayerAudio playerAudio;

    public Transform orientation;

     public float horizontalInput;
     public float verticalInput;

    public bool wasGrounded;
    public bool wasFalling;
    public float startOffFall;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        wallrunning,
        climbing,
        vaulting,
        crouching,
        sliding,
        dashing,
        boosting,
        idle,
        air
    }

    public bool sliding;
    public bool crouching;
    public bool wallrunning;
    public bool dashing;
    public bool climbing;
    public bool vaulting;
    public bool boosting;
    public bool idle;

    public bool freeze;
    public bool unlimited;
    
    public bool restricted;

    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_mode;

    private void Start()
    {
        climbingScriptDone = GetComponent<ClimbingDone>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        if (!IsOwner) return;
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        TextStuff();

        // handle drag
        if (state == MovementState.walking || state == MovementState.crouching || state == MovementState.boosting)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        if (moveSpeed >= 17)
        {
            GameManager.Instance.FastSpeed();
        }
        else if (moveSpeed >= 13 && moveSpeed < 17)
        {
            GameManager.Instance.MediumSpeed();
        }
        else if (moveSpeed >= 8.5 && moveSpeed < 13)
        {
            GameManager.Instance.SlowSpeed();
        }
        else if (moveSpeed >= 0 && moveSpeed < 8.5)
        {
            GameManager.Instance.StartingSpeed();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        MovePlayer();
        if (!wasFalling && isFalling) startOffFall = transform.position.y;
        if (!wasGrounded && grounded)
        {
            //playerAudio.playLandAudio();
        }

        wasGrounded = grounded;
        wasFalling = isFalling;

        wasGrounded = grounded;
        wasFalling = isFalling;
    }

    bool isFalling { get { return (!grounded && rb.linearVelocity.y < 0); } }

    public void destroyPlayer()
    {
        Destroy(gameObject);
    }
    
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey) && horizontalInput == 0 && verticalInput == 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            crouching = true;
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            crouching = false;
        }
    }

    private MovementState lastState;
    bool keepMomentum;
    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            rb.linearVelocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        // Mode - Unlimited
        else if (unlimited)
        {
            state = MovementState.unlimited;
            desiredMoveSpeed = Mathf.Min(999f, hardSpeedCap);
        }

        // Mode - Vaulting
        else if (vaulting)
        {
            state = MovementState.vaulting;
            desiredMoveSpeed = Mathf.Min(vaultSpeed, hardSpeedCap);
        }

        // Mode - Climbing
        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = Mathf.Min(climbSpeed, hardSpeedCap);
        }

        // Mode - Boosting
        else if (boosting)
        {
            state = MovementState.boosting;

            desiredMoveSpeed = Mathf.Min(walkSpeed, hardSpeedCap);
            keepMomentum = true;
        }

        // Mode - Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = Mathf.Min(desiredMoveSpeed += wallrunSpeedIncrease * Time.deltaTime, hardSpeedCap);
        }

        // Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;

            // increase speed by one every second
            if (OnSlope() && rb.linearVelocity.y < 0.1f)
            {
                desiredMoveSpeed = Mathf.Min(slideSpeed, hardSpeedCap);
                keepMomentum = true;
            }

            else
                desiredMoveSpeed = Mathf.Min(slideGroundSpeed, hardSpeedCap);
        }
      
        // Mode - Dashing
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = Mathf.Min(dashSpeedChangeFactor, hardSpeedCap);
        }

        // Mode - Crouching
        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = Mathf.Min(crouchSpeed, hardSpeedCap);
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = Mathf.Min(walkSpeed, hardSpeedCap);
            keepMomentum = true;
        }

        // Mode - Idle
        else if (grounded)
        {
            state = MovementState.idle;

        }
        // Mode - Air
        else
        {
            state = MovementState.air;

            keepMomentum = true;
            if (moveSpeed < airMinSpeed)
            {
                desiredMoveSpeed = Mathf.Min(airMinSpeed, hardSpeedCap);
            }

        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;

        // deactivate keepMomentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / velRevertingSpeed);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                //time += Time.deltaTime * speedIncreaseMultiplier;
                time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
    }

    private void MovePlayer()
    {
        if (climbingScript.exitingWall) return;
        if (climbingScriptDone.exitingWall) return;
        if (restricted) return;

        if (state == MovementState.dashing) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        if(!wallrunning) rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

        // limit y vel
        if (maxYSpeed != 0 && rb.linearVelocity.y > maxYSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxYSpeed, rb.linearVelocity.z);
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        if (boosting)
        {
            //playerAudio.PlaySweetSpotAudio();
        }
        else
        {
            //playerAudio.playJumpAudio();
        }
        exitingSlope = true;

    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void TextStuff()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (OnSlope())
            text_speed.SetText("Speed: " + Round(rb.linearVelocity.magnitude, 1) + " / " + Round(moveSpeed, 1));

        else
            text_speed.SetText("Speed: " + Round(flatVel.magnitude, 1) + " / " + Round(moveSpeed, 1));

        text_mode.SetText(state.ToString());
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
}

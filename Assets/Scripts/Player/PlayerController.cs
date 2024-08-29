using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement Values
    public float walkSpeed = 7f;
    private float currentSpeed = 0f;
    private bool isMoving = false;

    // Running, Friction is a character's stopping power
    public float runSpeed = 10f;
    public float friction = 3f;
    private bool isRunning = false;


    // For dashing, we do this before transitioning into running
    public float dashAccel = 80f;
    public float dashSpeed = 17f;
    private bool initialDash = false;

    // Jumping and Falling
    public float jumpVelocity = 25f;
    public float gravity = 15f;
    public float fastFall = 7f;
    public float airSpeed = 5f;
    public float charWeight = 3f;
    public int numJumps = 2;
    private float currentJumpVelocity = 0;
    private int currentJumpNum = 0;
    private bool inAir = false;

    // If this guy can crouch walk
    public float crouchWalkSpeed = 2f;
    public bool crouchWalk = false;

    // Double Tap
    private float lastKeyDownTime = 0f;
    private float doubleTapTimeWindow = 0.3f;
    private KeyCode lastKeyPressed;

    // Controller Input!
    public float controllerDeadzone = 0.1f;
    public string horizontalAxisName = "Horizontal";
    public string jumpButtonName = "Jump";
    private float lastControllerInputTime = 0f;
    private float lastControllerInputMagnitude = 0f;


    // Update is called once per frame
    void Update()
    {
        inAir = !IsGrounded();
        isMoving = false;
        if (inAir)
        {   
            currentJumpVelocity = Mathf.MoveTowards(currentJumpVelocity, -gravity, gravity * charWeight * Time.deltaTime);      
            transform.position += Vector3.up * currentJumpVelocity * Time.deltaTime;
        }
        else
        {
            currentJumpNum = 0;
        }

        // Check for keyboard input
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Movement(KeyCode.LeftArrow, Vector3.left);
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Movement(KeyCode.RightArrow, Vector3.right);
            isMoving = true;
        }

        // Check for controller input
        float horizontalInput = Input.GetAxis(horizontalAxisName);
        if (Mathf.Abs(horizontalInput) > controllerDeadzone)
        {
            ControllerMovement(horizontalInput);
            isMoving = true;
        }

        // Jump input (works for both keyboard and controller)
        if (Input.GetButtonDown(jumpButtonName))
        {
            if (currentJumpNum < numJumps)
            {
                currentJumpNum += 1;
                Jump();
            }
        }

        if (!isMoving) 
        {
            isRunning = false;
            initialDash = false;
            currentSpeed = 0f;
        }
    }

    void Jump() 
    {
        float timeToPeak = jumpVelocity / gravity;
        currentJumpVelocity = jumpVelocity;
        inAir = true;

        if(currentJumpVelocity > 0) 
           {
              currentJumpVelocity = Mathf.MoveTowards(currentJumpVelocity, 0, gravity * charWeight * Time.deltaTime);
           }
        transform.position += Vector3.up * currentJumpVelocity * Time.deltaTime;
        Debug.Log("Jumping!");
    }

    void Movement(KeyCode key, Vector3 direction)
    {
        float currentTime = Time.time;

        if (Input.GetKeyDown(key))
        {
            if (key == lastKeyPressed && currentTime - lastKeyDownTime < doubleTapTimeWindow && !inAir)
            {
                // Double tap detected, start running
                isRunning = true;
                initialDash = true;
            }
            else
            {
                // First tap or different key, reset running
                isRunning = false;
            }

            lastKeyDownTime = currentTime;
            lastKeyPressed = key;
        }

        // Move the character
            if (isRunning && !inAir)
            {
                if (initialDash)
                {
                    if (currentSpeed < dashSpeed)
                    {
                        currentSpeed = Mathf.MoveTowards(currentSpeed, dashSpeed, dashAccel * Time.deltaTime);
                        if (currentSpeed >= dashSpeed)
                        {
                            Debug.Log("Reached dash speed: " + currentSpeed);
                            initialDash = false;
                        }
                    }
                }
                else
                {
                    // Transition to run speed after initial dash
                    currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeed, dashAccel * Time.deltaTime);
                }
            }

            else if (isRunning && inAir)
            {
                currentSpeed = runSpeed;
            }

            else if (!isRunning && inAir) 
            {
                currentSpeed = airSpeed;
            }

            else
            {
                currentSpeed = walkSpeed;
            }
        

        transform.position += direction * currentSpeed * Time.deltaTime;
        Debug.Log(currentSpeed);

        // Stop running if the key is released
        if (Input.GetKeyUp(key))
        {
            isRunning = false;
        }
    }

    void ControllerMovement(float input)
    {
        Vector3 direction = new Vector3(input, 0, 0);
        float inputMagnitude = Mathf.Abs(input);
        float currentTime = Time.time;

        // Adjust input based on deadzone
        float adjustedMagnitude = Mathf.InverseLerp(controllerDeadzone, 1f, inputMagnitude);
        Vector3 adjustedDirection = direction.normalized * adjustedMagnitude;

        if (adjustedMagnitude > 0)
        {
            // Check for running condition
            if (adjustedMagnitude > 0.9f && !inAir &&
                (currentTime - lastControllerInputTime < doubleTapTimeWindow ||
                 isRunning)) // This allows maintaining the running state
            {
                isRunning = true;
                if (!initialDash && currentSpeed < runSpeed)
                {
                    initialDash = true;
                }
            }
            else if (adjustedMagnitude <= 0.9f)
            {
                isRunning = false;
                initialDash = false;
            }

            // Move the character
            if (isRunning && !inAir)
            {
                if (initialDash)
                {
                    if (currentSpeed < dashSpeed)
                    {
                        currentSpeed = Mathf.MoveTowards(currentSpeed, dashSpeed, dashAccel * Time.deltaTime);
                        if (currentSpeed >= dashSpeed)
                        {
                            Debug.Log("Reached dash speed: " + currentSpeed);
                            initialDash = false;
                        }
                    }
                }
                else
                {
                    // Transition to run speed after initial dash
                    currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeed, dashAccel * Time.deltaTime);
                }
            }
            else if (isRunning && inAir)
            {
                currentSpeed = runSpeed;
            }
            else if (!isRunning && inAir)
            {
                currentSpeed = airSpeed;
            }
            else
            {
                currentSpeed = Mathf.Lerp(0, walkSpeed, adjustedMagnitude);
            }

            transform.position += adjustedDirection * currentSpeed * Time.deltaTime;
            Debug.Log("Controller Speed: " + currentSpeed + " Adjusted Magnitude: " + adjustedMagnitude);

            lastControllerInputTime = currentTime;
            lastControllerInputMagnitude = inputMagnitude;
        }
        else
        {
            // Input is within deadzone, treat as no input
            isRunning = false;
            initialDash = false;
            currentSpeed = 0f;
            lastControllerInputMagnitude = 0f;
        }
    }

    public bool IsGrounded()
    {
        float rayLength = 0.3f; // Slightly longer than the character's collider's distance from the ground
        Vector3 origin = transform.position + Vector3.down * 0.5f; // Adjust based on character's size

        // Cast a ray downwards from the character's feet
        RaycastHit hit;
        Debug.DrawRay(origin, Vector3.down * rayLength, Color.red);
        if (Physics.Raycast(origin, Vector3.down, out hit, rayLength))
        {
            return hit.collider != null && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform") && currentJumpVelocity <= 0);
        }

        return false;
    }
}

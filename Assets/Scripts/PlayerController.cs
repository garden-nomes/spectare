﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public System.Action onDeath;

    public int doubleJumps = 0;

    public GameObject poof;
    public GameObject sidePoof;
    public float poofSpeedThreshold = 6f;
    public string enemyTag = "Enemy";
    public string powerupTag = "Powerup";
    public string spikesTag = "Spikes";
    public float enemyBounceForce = 20f;
    public float enemyDamageInvulnerabilityTime = 2f;
    public float maxJumpHeight = 5f;
    public float maxRiseTime = 0.5f;
    public float maxFallTime = 0.5f;
    public float maxHorizontalSpeed = 5f;
    public float airControl = 0.3f;
    public float wallDrag = 0.1f;
    public float timeToMaxSpeed = 0.5f;
    public float timeToStop = 0.25f;
    public float coyoteTime = 0.1f;
    public float wallJumpGraceTime = 0.1f;
    public float jumpPressedInAirTime = 0.25f;
    public Vector2 velocity;

    private MovementController movementController;
    private SpriteRenderer spriteRenderer;
    public bool IsGrounded => movementController.IsGrounded;
    public bool IsLeftTouching => movementController.IsLeftTouching;
    public bool IsRightTouching => movementController.IsRightTouching;
    private Vector2 movementInput = Vector2.zero;
    private bool isJumpPressed = false;
    private bool isGravityEnabled = true;
    private bool isJumping = false;
    private float coyoteTimer = 0f;
    private float jumpPressedInAirTimer = 0f;
    private float wallJumpGraceTimer = 0f;
    private float invulnerableTimer = 0f;
    private int doubleJumpsRemaining = 0;
    private bool isWallJumpToRight = false;

    private int powerups = 0;
    public int Powerups => powerups;

    void Start()
    {
        movementController = GetComponent<MovementController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementController.OnLand += OnLand;
        invulnerableTimer = enemyDamageInvulnerabilityTime;
    }

    void Update()
    {
        // update input state (input must be handled in Update() and not FixedUpdate())
        movementInput = ReadMovementInput();
        isJumpPressed = IsJumpPressed() || isJumpPressed; // it's possible for multiple Update()'s to occur before a FixedUpdate()
    }

    void FixedUpdate()
    {
        // check current input state
        bool isLeftDown = movementInput.x < 0f;
        bool isRightDown = movementInput.x > 0f;

        // apply horizontal velocity if left or right pressed
        float acceleratingForce = maxHorizontalSpeed / timeToMaxSpeed;
        float brakingForce = maxHorizontalSpeed / timeToStop;
        float airControlForce = acceleratingForce * airControl;

        if (isLeftDown && !isRightDown && velocity.x > -maxHorizontalSpeed)
        {
            bool isStopping = velocity.x > 0f;
            float force = IsGrounded ? (isStopping ? brakingForce : acceleratingForce) : airControlForce;
            velocity.x -= Time.deltaTime * force;
            velocity.x = Mathf.Max(velocity.x, -maxHorizontalSpeed);
        }
        else if (isRightDown && !isLeftDown && velocity.x < maxHorizontalSpeed)
        {
            bool isStopping = velocity.x < 0f;
            float force = IsGrounded ? (isStopping ? brakingForce : acceleratingForce) : airControlForce;
            velocity.x += Time.deltaTime * force;
            velocity.x = Mathf.Min(velocity.x, maxHorizontalSpeed);
        }
        else if (!isRightDown && !isLeftDown && velocity.x != 0f && movementController.IsGrounded)
        {
            // apply braking force
            float step = Time.deltaTime * maxHorizontalSpeed / timeToStop;

            if (Mathf.Abs(velocity.x) < step)
                velocity.x = 0f;
            else
                velocity.x -= step * Mathf.Sign(velocity.x);
        }

        // apply gravity
        if (isGravityEnabled)
        {
            float gravity = 2f * maxJumpHeight / (maxFallTime * maxFallTime);

            velocity.y -= gravity * Time.deltaTime;

            // apply wall drag
            if ((IsLeftTouching || IsRightTouching) && velocity.y < 0f)
                velocity.y -= wallDrag * velocity.y;
        }

        // update wall jump information
        if (movementController.IsLeftTouching || movementController.IsRightTouching)
        {
            wallJumpGraceTimer = wallJumpGraceTime;
            isWallJumpToRight = movementController.IsLeftTouching;
        }

        // jump!
        bool canJump = !isJumping && (movementController.IsGrounded || coyoteTimer < coyoteTime);
        bool shouldJump = isJumpPressed || jumpPressedInAirTimer > 0f;

        if (canJump && shouldJump)
        {
            StartCoroutine(JumpCoroutine());
            jumpPressedInAirTimer = 0f;

            Poof();
        }
        else if (wallJumpGraceTimer > 0f && shouldJump)
        {
            StartCoroutine(JumpCoroutine());
            velocity.x = isWallJumpToRight ? maxHorizontalSpeed : -maxHorizontalSpeed;
            wallJumpGraceTimer = 0f;
            doubleJumpsRemaining = doubleJumps;

            SidePoof(isWallJumpToRight);
        }
        else if (doubleJumpsRemaining > 0 && shouldJump)
        {
            // instantaneous speed change on double jump
            if (isLeftDown)
                velocity.x = -maxHorizontalSpeed;
            else if (isRightDown)
                velocity.x = maxHorizontalSpeed;
            else
                velocity.x = 0f;

            StartCoroutine(JumpCoroutine());
            doubleJumpsRemaining--;

            Poof();
        }
        else if (isJumpPressed)
        {
            // track when jump button is pressed in the air
            jumpPressedInAirTimer = jumpPressedInAirTime;
        }

        // jump event has been handled
        isJumpPressed = false;

        // update position
        bool wasGrounded = movementController.IsGrounded;
        Vector2 delta = velocity * Time.deltaTime;
        Vector2 actualDelta = movementController.Move(delta);
        velocity = actualDelta / Time.deltaTime;

        // track coyote time
        if (movementController.IsGrounded || isJumping)
            coyoteTimer = 0f;
        else
            coyoteTimer += Time.deltaTime;

        // landed
        if (!wasGrounded && movementController.IsGrounded)
        {
            // reset isJumping flag and double jumps
            isJumping = false;
            doubleJumpsRemaining = doubleJumps;

            if (velocity.y < -poofSpeedThreshold)
                Poof();
        }

        // flip sprite
        if (isLeftDown && !isRightDown)
            GetComponent<SpriteRenderer>().flipX = true;
        else if (!isLeftDown && isRightDown)
            GetComponent<SpriteRenderer>().flipX = false;

        // blink while invulnerable
        if (invulnerableTimer > 0f)
        {
            spriteRenderer.enabled = (invulnerableTimer * 3f) % 1f < 0.666f;
            invulnerableTimer -= Time.deltaTime;
        }

        // timers
        if (jumpPressedInAirTimer > 0f)
            jumpPressedInAirTimer -= Time.deltaTime;
        if (wallJumpGraceTimer > 0f)
            wallJumpGraceTimer -= Time.deltaTime;

    }

    private void OnLand(Collider2D other)
    {
        var btn = other.GetComponent<Button>();
        if (btn != null)
            btn.Press();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(powerupTag))
        {
            if (powerups < 4)
                powerups++;

            if (powerups == 4)
                doubleJumps = 1;

            GameObject.Destroy(other.gameObject);
        }
        else if (invulnerableTimer <= 0f && (other.CompareTag(enemyTag) || other.CompareTag(spikesTag)))
        {
            if (powerups > 0)
            {
                if (other.CompareTag(enemyTag))
                {
                    var toOther = other.transform.position - transform.position;
                    velocity = (Vector2) toOther.normalized * -enemyBounceForce;
                }
                else if (other.CompareTag(spikesTag))
                {
                    velocity = new Vector2(0f, enemyBounceForce);
                }

                invulnerableTimer = enemyDamageInvulnerabilityTime;
                powerups = 0;
                doubleJumps = 0;
            }
            else
            {
                onDeath?.Invoke();
            }
        }
    }

    private IEnumerator JumpCoroutine()
    {
        isJumping = true;

        float jumpTimer = 0f;
        isGravityEnabled = false;
        velocity.y = maxJumpHeight / maxRiseTime;

        while (IsJumpDown() && jumpTimer < maxRiseTime)
        {
            yield return null;
            jumpTimer += Time.deltaTime;
        }

        velocity.y = 0f;
        isGravityEnabled = true;
    }

    // read movement from WSAD/Arrow keys/Gamepad
    Vector2 ReadMovementInput()
    {
        bool isLeftPressed = Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed;
        bool isRightPressed = Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed;
        bool isUpPressed = Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed;
        bool isDownPressed = Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed;

        float horizontal = (isLeftPressed ? -1f : 0f) + (isRightPressed ? 1f : 0f);
        float vertical = (isDownPressed ? -1f : 0f) + (isUpPressed ? 1f : 0f);
        var movement = new Vector2(horizontal, vertical);

        if (Gamepad.current != null)
        {
            movement += Gamepad.current.leftStick.ReadValue();
        }

        return movement.normalized;
    }

    // read spacebar or gamepad button press
    bool IsJumpPressed()
    {
        return Keyboard.current.spaceKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);
    }

    bool IsJumpDown()
    {
        return Keyboard.current.spaceKey.isPressed ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed);
    }

    void Poof()
    {
        Instantiate(poof, transform.position + Vector3.up * 0.25f, Quaternion.identity);
    }

    void SidePoof(bool isRight)
    {
        var obj = Instantiate(
            sidePoof,
            transform.position + new Vector3(isRight ? 0.25f : -0.25f, 1f, 0f),
            Quaternion.identity);
    }
}

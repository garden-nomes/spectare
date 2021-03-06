using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public System.Action onDeath;
    public System.Action onDamage;
    public System.Action<Collider2D> onPowerupPickup;
    public System.Action<Collider2D> onKeyPickup;
    public System.Action<Collider2D> onUnlock;

    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip deathSound;
    public GameObject poof;
    public GameObject sidePoof;
    public float poofSpeedThreshold = 6f;
    public string enemyTag = "Enemy";
    public string powerupTag = "Powerup";
    public string spikesTag = "Spikes";
    public string keyPickupTag = "KeyPickup";
    public string lockTag = "Lock";
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
    public bool hasDoubleJump = false;

    private MovementController movementController;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    public bool IsGrounded => movementController.IsGrounded;
    public bool IsLeftTouching => movementController.IsLeftTouching;
    public bool IsRightTouching => movementController.IsRightTouching;
    private bool isGravityEnabled = true;
    private bool isJumping = false;
    private float coyoteTimer = 0f;
    private float jumpPressedInAirTimer = 0f;
    private float wallJumpGraceTimer = 0f;
    private float invulnerableTimer = 0f;
    private int doubleJumpsRemaining = 0;
    private bool isWallJumpToRight = false;

    void Start()
    {
        movementController = GetComponent<MovementController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementController.OnLand += OnLand;
        invulnerableTimer = enemyDamageInvulnerabilityTime;
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        // check current input state
        bool isLeftDown = IsLeftDown();
        bool isRightDown = IsRightDown();
        bool isJumpPressed = IsJumpPressed();

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
            doubleJumpsRemaining = hasDoubleJump ? 1 : 0;

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
            audioSource.PlayOneShot(landingSound);

            // reset isJumping flag and double jumps
            isJumping = false;
            doubleJumpsRemaining = hasDoubleJump ? 1 : 0;

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
            onPowerupPickup?.Invoke(other);
        }
        else if (other.CompareTag(keyPickupTag))
        {
            onKeyPickup?.Invoke(other);
        }
        else if (other.CompareTag(lockTag))
        {
            onUnlock?.Invoke(other);
        }
        else if (invulnerableTimer <= 0f && (other.CompareTag(enemyTag) || other.CompareTag(spikesTag)))
        {
            if (hasDoubleJump)
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
                onDamage?.Invoke();
            }
            else
            {
                audioSource.PlayOneShot(deathSound);
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

        audioSource.PlayOneShot(jumpSound);

        while (IsJumpHeld() && jumpTimer < maxRiseTime)
        {
            yield return null;
            jumpTimer += Time.deltaTime;
        }

        velocity.y = 0f;
        isGravityEnabled = true;
    }

    // input helpers

    bool IsLeftDown()
    {
        return Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed ||
            (Gamepad.current != null &&
                (Gamepad.current.leftStick.left.isPressed ||
                    Gamepad.current.dpad.left.isPressed));
    }

    bool IsRightDown()
    {
        return Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed ||
            (Gamepad.current != null &&
                (Gamepad.current.leftStick.right.isPressed ||
                    Gamepad.current.dpad.right.isPressed));
    }

    bool IsJumpPressed()
    {
        return Keyboard.current.spaceKey.wasPressedThisFrame ||
            (Gamepad.current != null &&
                (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                    Gamepad.current.buttonEast.wasPressedThisFrame ||
                    Gamepad.current.buttonNorth.wasPressedThisFrame ||
                    Gamepad.current.buttonWest.wasPressedThisFrame));
    }

    bool IsJumpHeld()
    {
        return Keyboard.current.spaceKey.isPressed ||
            (Gamepad.current != null &&
                (Gamepad.current.buttonSouth.isPressed ||
                    Gamepad.current.buttonEast.isPressed ||
                    Gamepad.current.buttonNorth.isPressed ||
                    Gamepad.current.buttonWest.isPressed));
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

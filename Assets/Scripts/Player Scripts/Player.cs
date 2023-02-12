using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility.Time;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float turnDuration = .2f;
    [SerializeField] List<Transform> rayOrigins = new List<Transform>();

    [Header("Jump")]
    [SerializeField] float jumpPower = 27f;
    [SerializeField] float gravityScale = 9f;
    [SerializeField] float coyoteTimeDuration = .1f;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float groundCheckRadius = .4f;
    [SerializeField] Transform groundCheckOrigin;
    [SerializeField] LayerMask groundMask;

    [Header("Abilities")]
    [SerializeField] float songRadius = 5f;
    [SerializeField] float songDuration = .8f;
    [SerializeField] LayerMask plantMask;

    [Header("Graphics")]
    [SerializeField] GameObject graphics;

    [Header("Audio")]
    [SerializeField] float stepSoundDuration = .2f;

    public bool IsFacingRight { get; private set; } = true;
    Vector2 moveDirection;

    bool isController = false;
    bool isSticking;

    const float gravity = 9.81f;
    float verticalSpeed = 0f;

    bool hasJump = false;
    bool wasGrounded = false;
    Collider2D currentGround = null;
    bool IsGrounded => currentGround;
    bool CanJump => IsGrounded || !coyoteTimer.IsExpired;

    bool HasAttachedPlatform => fixedJoint.connectedBody != null;

    Timer songTimer = new Timer();
    Timer coyoteTimer = new Timer();

    #region Anim Constants

    const string animMoveSpeed = "MoveSpeed";
    const string animRotation = "Rotation";
    const string animRotationDirection = "RotationDirection";
    const string animIsSticking = "IsSticking";
    const string animJumpStart = "JumpStart";

    #endregion

    #region Components

    InputAction inputAction;
    PlayerControls playerControls;
    Animator anim;
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    FixedJoint2D fixedJoint;
    PlayerAudioManager playerAudio;
    SoundWavesVFX wavesVFX;
    Transform under_platform;

    #endregion


    void Awake()
    {
        playerControls = new PlayerControls();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        fixedJoint = GetComponent<FixedJoint2D>();
        playerAudio = GetComponent<PlayerAudioManager>();
        wavesVFX = GetComponentInChildren<SoundWavesVFX>();

        CheckPoint.Set(rb.position);
    }

    #region Inputs

    void OnEnable()
    {
        inputAction = playerControls.Player.Move;
        inputAction.Enable();
    }

    void OnDisable()
    {
        inputAction.Disable();
    }

    void ChangeControlInput(InputAction.CallbackContext context)
    {
        if (context.control.device is Keyboard && isController)
        {
            isController = false;
            GameManager.KeyboardOrController.Invoke(isController);
        }
        else if (context.control.device is Gamepad && !isController)
        {
            isController = true;
            GameManager.KeyboardOrController.Invoke(isController);
        }
    }

    #endregion

    void Update()
    {
        if ((!IsFacingRight && moveDirection.x > 0f || IsFacingRight && moveDirection.x < 0f) && !isSticking)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        RefreshGround();

        if (IsGrounded)
        {
            if (currentGround.CompareTag("MovingPlatform"))
                LinkPlatform(currentGround.gameObject.GetComponentInParent<Rigidbody2D>());
            else
                UnlinkPlatform();

            if (!wasGrounded)
                playerAudio.PlayLanding();

            verticalSpeed = 0f;
            anim.SetBool(animJumpStart, false);
        }
        else
        {
            UnlinkPlatform();

            verticalSpeed -= gravity * gravityScale * Time.deltaTime;

            if (IsTouchingRoof())
                verticalSpeed = -1f;

            if (wasGrounded && verticalSpeed <= 0f)
                coyoteTimer.Set(coyoteTimeDuration);
        }

        if (!isSticking)
        {
            if (hasJump && CanJump)
                DoJump();

            Vector2 move = new Vector2(moveDirection.x * moveSpeed, verticalSpeed);
            if (HasAttachedPlatform)
            {
                DoMoveAnchor(move);
            }
            else
            {
                DoMoveBody(move);
            }
        }

        #region Movement functions

        void DoJump()
        {
            verticalSpeed = jumpPower;
            coyoteTimer.Set(0f);
            UnlinkPlatform();
            hasJump = false;
            playerAudio.PlayJump();
            anim.SetBool(animJumpStart, true);
        }

        void DoMoveBody(Vector2 move)
        {
            if (IsStuckInCorner())
                move.y = .1f;

            rb.MovePosition(rb.position + move * Time.deltaTime);
        }

        void DoMoveAnchor(Vector2 move)
        {
            var relativeMove = move;
            relativeMove.x *= -1f;
            fixedJoint.anchor = fixedJoint.anchor + relativeMove * Time.deltaTime;
        }

        #endregion

    }

    #region Collision Checks

    bool IsWalkingIntoWall()
    {
        foreach (var origin in rayOrigins)
        {
            var offset = (IsFacingRight ? Vector3.right : Vector3.left) * .5f;
            var startPos = origin.position + offset;

            //Debug.DrawLine(startPos, startPos + offset * .1f, Color.green, 1f);
            var hit = Physics2D.Raycast(startPos, offset, .1f, groundMask);
            if (hit.collider != null)
                return true;
        }
        return false;
    }

    void RefreshGround()
    {
        wasGrounded = IsGrounded;

        var groundSurfaces = GetGroundOverlap();
        switch (groundSurfaces.Length)
        {
            case 0: currentGround = null;
                break;

            case 1: currentGround = groundSurfaces[0];
                break;

            default:
                currentGround = RaycastDownwards().collider;
                // if the raycast is null, keep the closest (?)
                break;
        }

        RaycastHit2D RaycastDownwards()
        {
            Vector2 capsuleBottom = new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y);
            return Physics2D.Raycast(capsuleBottom, -transform.up, groundCheckDistance, groundMask);
        }

        Collider2D[] GetGroundOverlap()
        {
            return Physics2D.OverlapCircleAll(groundCheckOrigin.position, groundCheckRadius, groundMask);
        }
    }

    private bool IsTouchingRoof()
    {
        return RaycastUpwards().collider != null;

        RaycastHit2D RaycastUpwards()
        {
            Vector2 capsuleTop = new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.max.y);
            return Physics2D.Raycast(capsuleTop, transform.up, groundCheckDistance, groundMask);
        }
    }

    Vector2 oldPosition, oldMove;
    bool IsStuckInCorner()
    {
        var delta = rb.position - oldPosition;
        oldPosition = rb.position;
        oldMove = (isSticking ? Vector2.zero : moveDirection);
        return !IsWalkingIntoWall() && (delta.x == 0f && oldMove.x != 0f);
    }

    #endregion

    #region Movement

    public void Move(InputAction.CallbackContext context)
    {
        ChangeControlInput(context);

        if (IsGrounded && Mathf.Approximately(moveDirection.x, 0f))
            StartCoroutine(_SoundStepsLoop());

        moveDirection.x = context.ReadValue<Vector2>().x;
        anim.SetInteger(animMoveSpeed, Mathf.Abs((int)moveDirection.x));
    }

    #region Platform linking

    public void LinkPlatform(Rigidbody2D rb)
    {
        fixedJoint.autoConfigureConnectedAnchor = true;
        fixedJoint.connectedBody = rb;
        fixedJoint.enabled = true;
        fixedJoint.autoConfigureConnectedAnchor = false;
    }

    public void UnlinkPlatform()
    {
        fixedJoint.enabled = false;
        fixedJoint.connectedBody = null;
    }

    public void UnlinkPlatform(Rigidbody2D rb)
    {
        if (fixedJoint.connectedBody == rb)
        {
            fixedJoint.enabled = false;
            fixedJoint.connectedBody = null;
        }
    }

    #endregion

    IEnumerator _SoundStepsLoop()
    {
        while (IsGrounded && !Mathf.Approximately(moveDirection.x, 0f))
        {
            playerAudio.PlaySteps();
            yield return new WaitForSeconds(stepSoundDuration);
        }
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;

        if (IsGrounded)
        {
            anim.SetBool(animRotation, true);
            anim.SetBool(animRotationDirection, IsFacingRight);
        }

        float targetRotation = IsFacingRight ? 0f : 180f;
        graphics.transform.DORotate(new Vector3(0f, targetRotation, 0f), turnDuration);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        ChangeControlInput(context);

        if (context.performed && CanJump)
        {
            hasJump = true;
            anim.SetBool(animJumpStart, true);
        }

        if (context.canceled && verticalSpeed > 0f)
            verticalSpeed *= .5f;
    }

    #endregion

    #region Abilities

    public void PlayGrowSong(InputAction.CallbackContext context)
    {
        ChangeControlInput(context);
        if (context.performed && songTimer.IsExpired)
        {
            CheckActivable(true);
            songTimer.Set(songDuration);
            playerAudio.PlayGrowSong();
        }
    }

    public void PlayShrinkSong(InputAction.CallbackContext context)
    {
        ChangeControlInput(context);
        if (context.performed && songTimer.IsExpired)
        {
            CheckActivable(false);
            songTimer.Set(songDuration);
            playerAudio.PlayShrinkSong();
        }
    }

    private void CheckActivable(bool good)
    {
        wavesVFX.Play(songRadius, songDuration);
        RaycastHit2D[] ActivablePlants = Physics2D.CircleCastAll(transform.position, songRadius, transform.forward, 0, plantMask);

        foreach (RaycastHit2D ray in ActivablePlants)
        {
            try
            {
                //print("Found plant: " + ray.collider.gameObject.name);
                var comp = ray.collider.GetComponentInParent<ExtendableRoot>();
                if (good)
                    comp.Ahead_Root();
                else
                    comp.Retreat_Root();
            }
            catch { }
        }
    }

    public void StickToFloor(InputAction.CallbackContext context)
    {
        ChangeControlInput(context);

        if (context.started && IsGrounded)
        {
            try
            {
                under_platform = currentGround.gameObject.GetComponentInParent<RotatingPlatform>().transform;
                transform.SetParent(under_platform);

                rb.simulated = false;
                isSticking = true;

                playerAudio.PlayStick();
                anim.SetBool(animIsSticking, isSticking);
            }
            catch { }
        }

        if (context.canceled)
        {
            if (isSticking)
            {
                transform.parent = null;
                transform.position = GetUnstickTraslation(transform);
                transform.rotation = Quaternion.identity;
                graphics.transform.rotation = Quaternion.Euler(0, IsFacingRight ? 0 : 180, 0);

                rb.simulated = true;

                verticalSpeed = 0;
                isSticking = false;

                anim.SetBool(animIsSticking, isSticking);
            }
        }

        Vector3 GetUnstickTraslation(Transform trs)
        {
            float angle = Vector2.Angle(trs.up, Vector3.down);
            if (angle < 90)
                return trs.position + trs.up * Mathf.Lerp(2f, 0f, angle / 90);
            else
                return trs.position;
        }
    }

    #endregion

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Killbox"))
        {
            playerAudio.PlayDeathOnSpikes();
            Death();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Killbox"))
        {
            playerAudio.PlayDeathInWater();
            Death();
        }
    }

    void Death()
    {
        transform.position = CheckPoint.LastActivated;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckOrigin.position, groundCheckRadius);
    }
}

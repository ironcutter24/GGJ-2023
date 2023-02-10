using DG.Tweening;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility.Patterns;
using Utility.Time;

public class Player : Singleton<Player>
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
    [SerializeField] LayerMask groundMask;

    [Header("Abilities")]
    [SerializeField] float songRadius = 5f;
    [SerializeField] float songDuration = .8f;
    [SerializeField] LayerMask plantMask;

    [Header("Graphics")]
    [SerializeField] GameObject graphics;

    [Header("Audio")]
    [SerializeField] StudioEventEmitter stepsSFX;
    [SerializeField] float stepSoundDuration = .2f;
    [SerializeField] StudioEventEmitter jumpSFX, landingSFX, stickingSFX, growSongSFX, shrinkSongSFX, deathOnSpikesSFX, deathInWaterSFX;

    public bool IsFacingRight { get; private set; } = true;
    Vector2 moveDirection;

    bool isController = false;
    bool isSticking;

    const float gravity = 9.81f;
    float verticalSpeed = 0f;

    bool hasJump = false, isGrounded = false, wasGrounded = false;
    bool canJump => isGrounded || !coyoteTimer.IsExpired;

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

    private PlayerControls playerControls;
    private InputAction inputAction;
    private Animator anim;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private SoundWavesVFX wavesVFX;
    private FixedJoint2D fixedJoint;
    private Transform under_platform;

    #endregion


    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        wavesVFX = GetComponentInChildren<SoundWavesVFX>();
        anim = GetComponentInChildren<Animator>();
        fixedJoint = GetComponent<FixedJoint2D>();

        CheckPoint.Set(rb.position);
    }

    #region Inputs

    private void OnEnable()
    {
        inputAction = playerControls.Player.Move;
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.Disable();
    }

    private void ChangeControlInput(InputAction.CallbackContext context)
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

    Vector2 oldPosition, oldMove;
    bool shouldKickUpwards = false;
    private void FixedUpdate()
    {
        wasGrounded = isGrounded;
        RaycastHit2D hit;
        isGrounded = IsGrounded(out hit);

        if (!wasGrounded && isGrounded)
            landingSFX.Play();

        if (!isGrounded && wasGrounded && verticalSpeed <= 0f)
            coyoteTimer.Set(coyoteTimeDuration);

        if (hit.collider != null && hit.collider.CompareTag("MovingPlatform"))
        {
            LinkPlatform(hit.collider.gameObject.GetComponentInParent<Rigidbody2D>());
        }
        else
        {
            UnlinkPlatform();
        }

        var delta = rb.position - oldPosition;
        shouldKickUpwards = !IsWalkingIntoWall() && (delta.x == 0f && oldMove.x != 0f);

        if (isGrounded)
        {
            if (hasJump)
            {
                Jump();
                UnlinkPlatform();
            }
            else
            {
                verticalSpeed = 0f;
                anim.SetBool(animJumpStart, false);
            }
        }
        else
        {
            if (hasJump && canJump)
            {
                Jump();
                UnlinkPlatform();
                coyoteTimer.Set(0f);
            }
            else
            {
                verticalSpeed -= gravity * gravityScale * Time.deltaTime;
            }

            if (shouldKickUpwards)
                verticalSpeed = 0f;
        }

        if (IsTouchingRoof())
            verticalSpeed = -1f;

        if (!isSticking)
        {
            var kickBugFixMove = (shouldKickUpwards ? Vector2.up * .1f : Vector2.zero);
            Vector2 move = new Vector2(moveDirection.x * moveSpeed, verticalSpeed);
            oldPosition = rb.position;
            oldMove = moveDirection;

            if (fixedJoint.connectedBody == null)
            {
                rb.MovePosition(rb.position + move * Time.deltaTime + kickBugFixMove);
            }
            else
            {
                move.x = -move.x;
                fixedJoint.anchor = fixedJoint.anchor + move * Time.deltaTime;
            }
        }
        else
        {
            oldMove = Vector2.zero;
        }
    }

    #region Collision Checks

    bool IsWalkingIntoWall()
    {
        foreach (var origin in rayOrigins)
        {
            var offset = (IsFacingRight ? Vector3.right : Vector3.left) * .5f;
            var startPos = origin.position + offset;

            //Debug.DrawLine(startPos, startPos + offset * .1f, Color.green, 1f);

            var hit = Physics2D.Raycast(
                startPos,
                offset,
                .1f,
                groundMask
                );

            if (hit.collider != null)
                return true;
        }
        return false;
    }

    private bool IsGrounded()
    {
        return RaycastDownwards().collider != null;
    }

    private bool IsGrounded(out RaycastHit2D hit)
    {
        hit = RaycastDownwards();
        return hit.collider != null;
    }

    private bool IsTouchingRoof()
    {
        return RaycastUpwards().collider != null;
    }

    RaycastHit2D RaycastDownwards()
    {
        return Physics2D.Raycast(CapsuleBottom, -transform.up, groundCheckDistance, groundMask);
    }

    void CircleOverlapDownwards()
    {
        //Vector2 point = rb.position + new Vector2(player_capsule.);
        //float radius = player_capsule.size.x * .5f;
        //Physics2D.OverlapCircleAll()
    }

    Vector2 CapsuleBottom => new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y);

    RaycastHit2D RaycastUpwards()
    {
        return Physics2D.Raycast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.max.y), transform.up, groundCheckDistance, groundMask);
    }

    #endregion

    #region Movement

    public void Move(InputAction.CallbackContext context)
    {
        ChangeControlInput(context);

        if (isGrounded && Mathf.Approximately(moveDirection.x, 0f))
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
        while (isGrounded && !Mathf.Approximately(moveDirection.x, 0f))
        {
            stepsSFX.Play();
            yield return new WaitForSeconds(stepSoundDuration);
        }
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;

        if (isGrounded)
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

        if (context.performed && canJump)
        {
            hasJump = true;
            anim.SetBool(animJumpStart, true);
        }

        if (context.canceled && verticalSpeed > 0f)
            verticalSpeed *= .5f;
    }

    void Jump()
    {
        verticalSpeed = jumpPower;
        hasJump = false;
        jumpSFX.Play();
    }

    public void StickOnFloor(InputAction.CallbackContext context)
    {
        ChangeControlInput(context);

        RaycastHit2D hit;
        if (context.started && IsGrounded(out hit))
        {
            try
            {
                under_platform = hit.collider.gameObject.GetComponentInParent<RotatingPlatform>().transform;
                transform.SetParent(under_platform);

                rb.simulated = false;
                isSticking = true;

                stickingSFX.Play();
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

    #region Abilities

    public void PlayGoodMusic(InputAction.CallbackContext context)
    {
        ChangeControlInput(context);
        if (context.performed && songTimer.IsExpired)
        {
            CheckActivable(true);
            songTimer.Set(songDuration);
            growSongSFX.Play();
        }
    }

    public void PlayBadMusic(InputAction.CallbackContext context)
    {
        ChangeControlInput(context);
        if (context.performed && songTimer.IsExpired)
        {
            CheckActivable(false);
            songTimer.Set(songDuration);
            shrinkSongSFX.Play();
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
                var comp = ray.collider.GetComponentInParent<Anim_Roots>();
                if (good)
                    comp.Ahead_Root();
                else
                    comp.Retreat_Root();
            }
            catch { }
        }
    }

    #endregion

    void Death()
    {
        transform.position = CheckPoint.LastActivated;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Killbox"))
        {
            deathOnSpikesSFX.Play();
            Death();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Killbox"))
        {
            deathInWaterSFX.Play();
            Death();
        }
    }
}

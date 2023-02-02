using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool is_facing_right { get; private set; } = true;

    [SerializeField]
    Transform rayTop, rayCenter, rayBottom;
    [SerializeField]
    private float movement_speed, jump_power, gravity_scale = 1f, song_radius = 5f, song_duration = .6f;
    [SerializeField]
    [Tooltip("Distance of the raycast to the ground for the jump")]
    private float buffer_check_distance = 0.3f;
    [SerializeField]
    [Tooltip("DO NOT TOUCH")]
    private LayerMask groundLayerMask, plantLayerMask;

    private Vector2 move_direction;

    private SoundWavesVFX wave;
    private Rigidbody2D player_rb;
    private InputAction player_move;
    private Animator playerAnimation;
    private Transform under_platform;
    private PlayerControls player_controls;
    private CapsuleCollider2D player_capsule;

    private float easyTimer = 1;
    private float gravity = 9.81f;
    private float verticalSpeed = 0f;
    private bool is_sticking, playingSong = false;
    private string animRunning = "Running";
    private string animRotating = "Rotating";
    private string animRotatingDirection = "DirectionRotating";

    private void Awake()
    {
        player_controls = new PlayerControls();
        player_rb = GetComponent<Rigidbody2D>();
        player_capsule = GetComponent<CapsuleCollider2D>();
        wave = GetComponentInChildren<SoundWavesVFX>();
        playerAnimation = GetComponent<Animator>();
    }
    private void Start()
    {
    }

    private void OnEnable()
    {
        player_move = player_controls.Player.Move;
        player_move.Enable();
    }

    private void OnDisable()
    {
        player_move.Disable();
    }

    private void Ciao()
    {
        playerAnimation.SetBool(animRotating, false);
        //transform.Rotate(new Vector3(0, 180, 0));
    }

    void Update()
    {
        if (!is_facing_right && move_direction.x > 0f || is_facing_right && move_direction.x < 0f)
        {
            Flip();
        }
        

        if (move_direction.x != 0)
            playerAnimation.SetBool(animRunning, true);
        else
            playerAnimation.SetBool(animRunning, false);

        if (playingSong)
        {
            easyTimer -= Time.deltaTime;
            if (easyTimer <= 0)
            {
                easyTimer = 1;
                playingSong = false;
            }
        }
    }

<<<<<<< Updated upstream
    float gravity = 9.81f;
    float verticalSpeed = 0f;
    private void FixedUpdate()
    {
        //Debug.Log("Is grounded: " + IsGrounded());
=======


    private void FixedUpdate()
    {
        //MoveWithVelocity();
        Debug.Log("Is grounded: " + IsGrounded());
>>>>>>> Stashed changes

        if (IsGrounded())
        {
            if (hasJump)
            {
                verticalSpeed = jump_power;
                hasJump = false;
            }
            else
            {
                verticalSpeed = 0f;
            }
        }
        else
        {
            verticalSpeed -= gravity * gravity_scale * Time.deltaTime;
        }

<<<<<<< Updated upstream
        if (IsTouchingRoof())
        {
            verticalSpeed = -1f;
        }

        if (!is_sticking)
=======
        if (!is_sticking && !playerAnimation.GetBool(animRotating))
>>>>>>> Stashed changes
        {
            Vector2 move = new Vector2(move_direction.x * movement_speed, verticalSpeed);
            player_rb.MovePosition(player_rb.position + move * Time.deltaTime);
        }
    }

<<<<<<< Updated upstream
    #region Collision Checks

    private bool IsGrounded()
=======

    void MoveWithVelocity()
>>>>>>> Stashed changes
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
        return Physics2D.Raycast(new Vector2(player_capsule.bounds.center.x, player_capsule.bounds.min.y), -transform.up, buffer_check_distance, groundLayerMask);
    }

    RaycastHit2D RaycastUpwards()
    {
        return Physics2D.Raycast(new Vector2(player_capsule.bounds.center.x, player_capsule.bounds.max.y), transform.up, buffer_check_distance, groundLayerMask);
    }

    #endregion

    private void Flip()
    {
        is_facing_right = !is_facing_right;
        playerAnimation.SetBool(animRotatingDirection, is_facing_right);
        playerAnimation.SetBool(animRotating, true);
    }

    public void Move(InputAction.CallbackContext context)
    {
        move_direction.x = context.ReadValue<Vector2>().x;
    }

    bool hasJump = false;
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
            hasJump = true;

        if (context.canceled && verticalSpeed > 0f)
            verticalSpeed *= .5f;
    }

    public void StickOnFloor(InputAction.CallbackContext context)
    {
        RaycastHit2D hit;
        if (context.started && IsGrounded(out hit))
        {
            is_sticking = true;
            player_rb.simulated = false;
            try
            {
                under_platform = hit.collider.gameObject.GetComponentInParent<RotatingPlatform>().transform;
            }
            catch { }
            transform.SetParent(under_platform);
        }

        if (context.canceled)
        {
            is_sticking = false;
            player_rb.simulated = true;
            transform.parent = null;
            verticalSpeed = 0;
            transform.rotation = Quaternion.Euler(0, is_facing_right ? 0 : 180, 0);
        }
    }

    public void PlayGoodMusic(InputAction.CallbackContext context)
    {
        if (context.performed && !playingSong)
        {
            CheckActivable(true);
            print("GOOD SONG");
            playingSong = true;
        }
    }

    public void PlayBadMusic(InputAction.CallbackContext context)
    {
        if (context.performed && !playingSong)
        {
            CheckActivable(false);
            print("BAD SONG");
            playingSong = true;
        }
    }

    private void CheckActivable(bool good)
    {
        wave.Play(song_radius, song_duration);
        RaycastHit2D[] ActivablePlants = Physics2D.CircleCastAll(transform.position, song_radius, transform.forward, 0, plantLayerMask);

        foreach (RaycastHit2D ray in ActivablePlants)
        {
            try
            {
                print("Found plant: " + ray.collider.gameObject.name);
                var comp = ray.collider.GetComponentInParent<Anim_Roots>();
                if (good)
                    comp.Ahead_Root();
                else
                    comp.Retreat_Root();
            }
            catch { }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Killbox"))
        {
            // Player death
        }
    }
}

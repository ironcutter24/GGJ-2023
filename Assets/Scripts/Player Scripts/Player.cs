using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool is_facing_right { get; private set; } = true;

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
    private Animator playerAnimator;
    private InputAction player_move;
    private Transform under_platform;
    private PlayerControls player_controls;
    private CapsuleCollider2D player_capsule;

    bool hasJump = false;
    float gravity = 9.81f;
    float verticalSpeed = 0f;
    private float easytimer = 1;
    private bool is_sticking, playingSong = false;
    private string animMoveSpeed = "MoveSpeed";
    private string animRotation = "Rotation";
    private string animRotationDirection = "RotationDirection";
    private string animSticking = "IsSticking";
    private string animStartSticking = "StartStiking";

    GameManager gm;


    private void Awake()
    {
        player_controls = new PlayerControls();
        player_rb = GetComponent<Rigidbody2D>();
        player_capsule = GetComponent<CapsuleCollider2D>();
        wave = GetComponentInChildren<SoundWavesVFX>();
        playerAnimator = GetComponentInChildren<Animator>();
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        player_move = player_controls.Player.Move;
        player_move.Enable();
        gm.lastCheckPointPos = transform.position;
    }

    private void OnDisable()
    {
        player_move.Disable();
    }

    void Update()
    {
        if (!is_facing_right && move_direction.x > 0f || is_facing_right && move_direction.x < 0f)
        {
            Flip();
        }

        if (playerAnimator.GetBool(animRotation))
            transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0, is_facing_right ? 0 : 180, 0), 1);


        if (playingSong)
        {
            easytimer -= Time.deltaTime;
            if (easytimer <= 0)
            {
                easytimer = 1;
                playingSong = false;
            }
        }
    }


    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            if (hasJump)
            {
                verticalSpeed = jump_power;
                hasJump = false;
            }
            else
                verticalSpeed = 0f;
        }
        else
            verticalSpeed -= gravity * gravity_scale * Time.deltaTime;

        if (IsTouchingRoof())
            verticalSpeed = -1f;

        if (!is_sticking)
        {
            Vector2 move = new Vector2(move_direction.x * movement_speed, verticalSpeed);
            player_rb.MovePosition(player_rb.position + move * Time.deltaTime);
        }
    }

    #region Collision Checks

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

        playerAnimator.SetBool(animRotation, true);
        playerAnimator.SetBool(animRotationDirection, is_facing_right);
    }

    public void Move(InputAction.CallbackContext context)
    {
        move_direction.x = context.ReadValue<Vector2>().x;
        playerAnimator.SetInteger(animMoveSpeed, Mathf.Abs((int)move_direction.x));
    }

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
            //player_rb.simulated = false;
            try
            {
                under_platform = hit.collider.gameObject.GetComponentInParent<RotatingPlatform>().transform;
            }
            catch { }
            transform.SetParent(under_platform);
            playerAnimator.SetBool(animSticking, is_sticking);
        }


        if (context.canceled)
        {
            is_sticking = false;
            //player_rb.simulated = true;
            transform.parent = null;
            verticalSpeed = 0;
            playerAnimator.SetBool(animSticking, is_sticking);
            //transform.rotation = Quaternion.Euler(0, is_facing_right ? 0 : 180, 0);
            player_rb.MoveRotation(0);
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Killbox"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

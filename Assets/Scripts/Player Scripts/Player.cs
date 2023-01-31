using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool is_facing_right { get; private set; }

    [SerializeField]
    private float movement_speed, jump_power, music_radius = 5f;
    [SerializeField]
    [Tooltip("Distance of the raycast to the ground for the jump")]
    private float buffer_check_distance = 0.3f;
    [SerializeField]
    [Tooltip("DO NOT TOUCH")]
    private LayerMask groundLayerMask, plantLayerMask;

    private RaycastHit2D hit;
    private Vector2 move_direction;

    private SoundWavesVFX wave;
    private Rigidbody2D player_rb;
    private InputAction player_move;
    private Transform under_platform;
    private PlayerControls player_controls;
    private CapsuleCollider2D player_capsule;

    private float easytimer = 1;
    private bool is_sticking, playngSong = false;

    private void Awake()
    {
        player_controls = new PlayerControls();
        player_rb = GetComponent<Rigidbody2D>();
        player_capsule = GetComponent<CapsuleCollider2D>();
        wave = GetComponentInChildren<SoundWavesVFX>();
    }

    void Start()
    {
        is_facing_right = true;
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

    void Update()
    {
        //Debug.DrawRay(new Vector3(player_capsule.bounds.center.x, player_capsule.bounds.min.y, 0), new Vector3(0, -buffer_check_distance, 0), Color.blue);

        if (!is_facing_right && move_direction.x > 0f || is_facing_right && move_direction.x < 0f)
            Flip();

        if (playngSong)
        {
            easytimer -= Time.deltaTime;
            if (easytimer <= 0)
            {
                easytimer = 1;
                playngSong = false;
            }
        }
    }

    [SerializeField]
    Transform rayTop, rayCenter, rayBottom;
    private void FixedUpdate()
    {
        const float distance = .1f;
        RaycastHit2D topRotationRay = Physics2D.Raycast(rayTop.position, transform.right, distance, groundLayerMask);
        RaycastHit2D centerRotationRay = Physics2D.Raycast(rayCenter.position, transform.right, distance, groundLayerMask);
        RaycastHit2D bottomRotationRay = Physics2D.Raycast(rayBottom.position, transform.right, distance, groundLayerMask);

        if (topRotationRay.collider != null)
            Debug.Log("Layer: " + topRotationRay.collider.gameObject.layer + "\tObj: " + topRotationRay.collider.gameObject.name);

        //Debug.DrawRay(rayTop.position, transform.right * distance, Color.green, Time.deltaTime);
        //Debug.DrawRay(rayBottom.position, transform.right * distance, Color.green, Time.deltaTime);

        if (topRotationRay.collider == null && centerRotationRay.collider == null && bottomRotationRay.collider == null)
        {
            player_rb.velocity = is_sticking ? Vector2.zero : new Vector2(move_direction.x * movement_speed, player_rb.velocity.y);
            Debug.Log("Updating velocity");
        }
    }

    private bool IsGrounded()
    {
        hit = Physics2D.Raycast(new Vector2(player_capsule.bounds.center.x, player_capsule.bounds.min.y), -transform.up, buffer_check_distance, groundLayerMask);
        return hit.collider != null;
    }

    private void Flip()
    {
        is_facing_right = !is_facing_right;
        transform.Rotate(new Vector3(0, 180, 0));
    }

    public void Move(InputAction.CallbackContext context)
    {
        move_direction.x = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !is_sticking)
            player_rb.velocity = new Vector2(move_direction.x, jump_power);

        if (context.canceled && player_rb.velocity.y > 0f && !is_sticking)
            player_rb.velocity = new Vector2(move_direction.x, player_rb.velocity.y * 0.5f);
    }

    public void StickOnFloor(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded())
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
            player_rb.velocity = Vector2.zero;
            transform.rotation = Quaternion.Euler(0, is_facing_right ? 0 : 180, 0);
        }
    }

    public void PlayGoodMusic(InputAction.CallbackContext context)
    {
        if (context.performed && !playngSong)
        {
            CheckActivable(true);
            print("GOOD SONG");
            playngSong = true;
        }
    }

    public void PlayBadMusic(InputAction.CallbackContext context)
    {
        if (context.performed && !playngSong)
        {
            CheckActivable(false);
            print("BAD SONG");
            playngSong = true;
        }
    }

    private void CheckActivable(bool good)
    {
        wave.Play(music_radius);
        RaycastHit2D[] ActivablePlants = Physics2D.CircleCastAll(transform.position, music_radius, transform.forward, 0, plantLayerMask);

        foreach (RaycastHit2D ray in ActivablePlants)
        {
            print(ray.collider.gameObject.name);
            if (good)
            {
                try
                {
                    ray.collider.GetComponentInParent<Anim_Roots>().Ahead_Root();
                }
                catch { }
            }
            else
            {
                try
                {
                    ray.collider.GetComponentInParent<Anim_Roots>().Retreat_Root();
                }
                catch { }
            }
        }
    }
}

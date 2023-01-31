using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float movement_speed, jump_power, music_radius = 5f;
    [SerializeField]
    [Tooltip("Distance of the raycast to the ground for the jump")]
    private float buffer_check_distance = 0.3f;
    [SerializeField]
    [Tooltip("DO NOT TOUCH")]
    private LayerMask layerMask, plantLayerMask;
    public bool is_facing_right { get; private set; }

    private PlayerControls player_controls;
    private InputAction player_move;
    private Vector2 move_direction;
    private Rigidbody2D player_rb;
    private CapsuleCollider2D player_capsule;
    private RaycastHit2D hit;
    private bool is_sticking, playngSong, sidecollision = false;
    private Transform under_platform;

    private float easytimer = 1;


    private void Awake()
    {
        player_controls = new PlayerControls();
        player_rb = GetComponent<Rigidbody2D>();
        player_capsule = GetComponent<CapsuleCollider2D>();
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

    private void FixedUpdate()
    {
        if (!sidecollision)
            player_rb.velocity = is_sticking ? Vector2.zero : new Vector2(move_direction.x * movement_speed, player_rb.velocity.y);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        sidecollision = true;
        if (IsGrounded())
            sidecollision = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        sidecollision = false;
    }

    private bool IsGrounded()
    {
        int layerMaskCombined = ( layerMask ) | ( plantLayerMask );

        hit = Physics2D.Raycast(new Vector2(player_capsule.bounds.center.x, player_capsule.bounds.min.y), -transform.up, buffer_check_distance, layerMaskCombined);
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
            //move_direction.y = jump_power;
            player_rb.velocity = new Vector2(move_direction.x, jump_power);

        if (context.canceled && player_rb.velocity.y > 0f && !is_sticking)
            //move_direction.y = player_rb.velocity.y * 0.5f;
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
        RaycastHit2D[] ActivablePlants = Physics2D.CircleCastAll(transform.position, music_radius, transform.forward, 0, plantLayerMask);

        foreach (RaycastHit2D ray in ActivablePlants)
        {
            print(ray.collider.gameObject.name);
            if (good)
                ray.collider.GetComponent<Anim_Roots>().Ahead_Root();
            else
                ray.collider.GetComponent<Anim_Roots>().Retreat_Root();

            //call Plant Event
        }
    }
}

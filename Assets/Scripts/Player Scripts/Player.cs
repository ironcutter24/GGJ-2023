using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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

    private PlayerControls player_controls;
    private InputAction player_move;
    private Vector2 move_direction;
    private Rigidbody2D player_rb;
    private CapsuleCollider2D player_capsule;
    private bool is_facing_right = true;
    private bool is_sticking, playngSong= false;
    private RaycastHit2D hit;
    private Transform under_platform;

    private void Awake()
    {
        player_controls = new PlayerControls();
        player_rb = GetComponent<Rigidbody2D>();
        player_capsule = GetComponent<CapsuleCollider2D>();
    }

    void Start()
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

    void Update()
    {
        //Debug.DrawRay(new Vector3(player_capsule.bounds.center.x, player_capsule.bounds.min.y, 0), new Vector3(0, -buffer_check_distance, 0), Color.blue);

        if (!is_facing_right && move_direction.x > 0f || is_facing_right && move_direction.x < 0f)
            Flip();
    }

    private void FixedUpdate()
    {
        player_rb.velocity = new Vector2(move_direction.x * movement_speed, player_rb.velocity.y);
    }

    private bool IsGrounded()
    {
        hit = Physics2D.Raycast(new Vector2(player_capsule.bounds.center.x, player_capsule.bounds.min.y), -transform.up, buffer_check_distance, layerMask);
        return hit.collider != null;
    }

    private void Flip()
    {
        is_facing_right = !is_facing_right;
        transform.Rotate(new Vector3(0, 180, 0));
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!is_sticking)
            move_direction = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !is_sticking)
            player_rb.velocity = new Vector2(player_rb.velocity.x, jump_power);

        if (context.canceled && player_rb.velocity.y > 0f && !is_sticking)
            player_rb.velocity = new Vector2(player_rb.velocity.x, player_rb.velocity.y * 0.5f);
    }

    public void StickOnFloor(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded())
        {
            is_sticking = true;
            player_rb.simulated = false;
            under_platform = hit.collider.gameObject.GetComponentInParent<RotatingPlatform>().transform;
            transform.SetParent(under_platform);
            move_direction = Vector2.zero;
        }

        if (context.canceled)
        {
            is_sticking = false;
            player_rb.simulated = true;
            transform.parent = null;
            transform.rotation = Quaternion.Euler(0, is_facing_right ? 0 : 180, 0);
        }
    }

    public void PlayGoodMusic(InputAction.CallbackContext context)
    {
        if (context.performed && !playngSong)
        {
            CheckActivable();
            print("GOOD SONG");
            playngSong = true;
        }
    }

    public void PlayBadMusic(InputAction.CallbackContext context)
    {
        if (context.performed && !playngSong)
        {
            CheckActivable();
            print("BAD SONG");
            playngSong = true;
        }
    }

    private void CheckActivable()
    {
        RaycastHit2D[] ActivablePlants = Physics2D.CircleCastAll(transform.position, music_radius, transform.forward, 0, plantLayerMask);

        foreach (RaycastHit2D ray in ActivablePlants)
        {
            print(ray.collider.gameObject.name);
            //call Plant Event
        }
    }
}

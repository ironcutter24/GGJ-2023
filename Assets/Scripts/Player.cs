using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float movement_speed = 5f;
    [SerializeField]
    private float jump_power = 5f;

    private PlayerControls player_controls;
    private InputAction player_move;
    private Vector2 move_direction;
    private Rigidbody2D player_rb;

    private void Awake()
    {
        player_controls = new PlayerControls();
        player_rb = GetComponent<Rigidbody2D>();
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
        player_rb.velocity = new Vector2(move_direction.x * movement_speed, player_rb.velocity.y);
    }

    private void FixedUpdate()
    {
        /*player_rb.AddForce(move_direction * movement_speed * Time.deltaTime);*/
    }

    public void Move(InputAction.CallbackContext context)
    {
        move_direction = context.ReadValue<Vector2>();

    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
            player_rb.velocity = new Vector2(player_rb.velocity.x, jump_power);

        if (context.canceled && player_rb.velocity.y > 0f)
            player_rb.velocity = new Vector2(player_rb.velocity.x, player_rb.velocity.y * 0.5f);
    }
}

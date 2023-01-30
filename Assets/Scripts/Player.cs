using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float movement_speed = 5f;


    private PlayerControls player_controls;
    private InputAction player_move;
    private Vector2 move_direction;
    private Rigidbody2D player_rb;

    private void Awake()
    {
        player_controls = new PlayerControls();
    }

    void Start()
    {
        player_rb = GetComponent<Rigidbody2D>();
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
        move_direction = player_move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        /*player_rb.AddForce(move_direction * movement_speed * Time.deltaTime);*/ 
        player_rb.velocity= new Vector2(move_direction.x * movement_speed, player_rb.velocity.y);
    }
}

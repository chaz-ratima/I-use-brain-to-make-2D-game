using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum PlayerState {
        IDLE, // make idle state which plays idle animation
        WALK, // make walk state animation play on walk state
        ROLL // make role animation
    }

    public Rigidbody2D rb;
    public Animator animator;
    private Vector3 moveDir;
    private Vector3 rollDir;
    private Vector3 lastMoveDir;
    public float moveSpeed = 5f;
    public float dashAmount = 2f;
    private float rollSpeed;
    public bool isDashing;
    public bool canMove;
    private PlayerState currentState;

    float moveX = 0f;
    float moveY = 0f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        currentState = PlayerState.IDLE;
    }

    void Start() {
        animator = GetComponent<Animator>();
    }

    private void Update() {

        //resets movement
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) {
            moveY = 0f;
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) {
            moveX = 0f;
        }

        MoveInputHandler();
        Dash();
        OnRollPress();

        switch (currentState) {
            case PlayerState.IDLE:
                animator.Play("PlayerIdle");
                canMove = true;
                break;
            case PlayerState.WALK:
                animator.Play("PlayerWalk");
                canMove = true;
                break;
            case PlayerState.ROLL:
                canMove = false;
                Roll();
                break;
        }
    }

    private void FixedUpdate() {
        FUState();
    }

    void MoveInputHandler() {

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            moveX = +1f;
        }

        // Disables diagonal movement
        /*if (Mathf.Abs(moveX) > Mathf.Abs(moveY)) {
            moveY = 0f;
        } else { moveX = 0f; }*/

        moveDir = new Vector3(moveX, moveY).normalized;
        LastDirection();
    }

    void LastDirection() {
        if (canMove) {
            if (moveX != 0 || moveY != 0) {
                currentState = PlayerState.WALK;
                lastMoveDir = moveDir;
                animator.SetFloat("MoveX", moveDir.x);
                animator.SetFloat("MoveY", moveDir.y);
            } else { currentState = PlayerState.IDLE; }
        }
    }

    void Dash() {
        if (Input.GetKeyDown(KeyCode.F)) {
            isDashing = true;
        }
    }

    void OnRollPress() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            rollDir = lastMoveDir;
            rollSpeed = 30f;
            currentState = PlayerState.ROLL;
        }
    }

    void Roll() {
        // Gets rollspeed and drops by multiplier until it hits the minimum then enters new state.
        float rollSpeedDropMultiplier = 5;
        rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

        //animator.Play("PlayerRoll");
        float rollSpeedMinimum = 5f;
        if (rollSpeed < rollSpeedMinimum) {
            canMove = true;
            if (moveX != 0 || moveY != 0) {
                currentState = PlayerState.WALK;
            }
            else { currentState = PlayerState.IDLE; }
        }
    }


    void FUState() {
        // Change of speeds in each states
        switch (currentState) {
            case PlayerState.IDLE:
                rb.velocity = new Vector2(0 , 0);
                break;
            case PlayerState.WALK:
                rb.velocity = moveDir * moveSpeed;

                if (isDashing) {
                    rb.MovePosition(transform.position + lastMoveDir * dashAmount);
                    isDashing = false;
                }
                break;
            case PlayerState.ROLL:
                rb.velocity = rollDir * rollSpeed;
                break;
        }
    }
}

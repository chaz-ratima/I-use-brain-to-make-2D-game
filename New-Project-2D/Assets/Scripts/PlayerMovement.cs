using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum State {
        IDLE, // make idle state which plays idle animation
        WALK, // make walk state animation play on walk state
        ROLL // make role animation
    }

    public Rigidbody2D rb;
    public Animator animator;
    private Vector3 moveDir;
    private Vector3 rollDir;
    private Vector3 lastMoveDir;
    public float moveSpeed = 10f;
    public float dashAmount = 10f;
    private float rollSpeed;
    private bool isDashing;
    private State state;

    float moveX = 0f;
    float moveY = 0f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        state = State.WALK;
    }

    private void Update() {
        switch (state) {
            case State.WALK:
                MoveInputHandler();
                Dash();
                OnRollPress();
                break;
            case State.ROLL:
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

        //resets movement
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) {
            moveY = 0f;
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) {
            moveX = 0f;
        }

        // Disables diagonal movement
        /*if (Mathf.Abs(moveX) > Mathf.Abs(moveY)) {
            moveY = 0f;
        } else { moveX = 0f; }*/

        moveDir = new Vector3(moveX, moveY).normalized;
        LastDirection();
    }

    void LastDirection() {
        if (moveX != 0 || moveY != 0) {
            lastMoveDir = moveDir;
            animator.SetFloat("MoveX", moveDir.x);
            animator.SetFloat("MoveY", moveDir.y);
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
            rollSpeed = 50f;
            state = State.ROLL;
        }
    }

    void Roll() {
        float rollSpeedDropMultiplier = 5;
        rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

        float rollSpeedMinimum = 10f;
        if (rollSpeed < rollSpeedMinimum) {
            state = State.WALK;
        }
    }


    void FUState() {
        switch (state) {
            case State.WALK:
                rb.velocity = moveDir * moveSpeed;

                if (isDashing) {
                    rb.MovePosition(transform.position + lastMoveDir * dashAmount);
                    isDashing = false;
                }
                break;
            case State.ROLL:
                rb.velocity = rollDir * rollSpeed;
                break;
        }
    }
}

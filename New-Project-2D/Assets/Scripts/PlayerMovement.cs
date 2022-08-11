using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    private Vector2 movement;
    public float moveSpeed = 5f;
    private bool dashPressed;
    private bool isDashing;
    public float dashSpeed = 100f;

    void Update() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        dashPressed = Input.GetKeyDown(KeyCode.Space);
        Dash();
    }

    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        if (!isDashing) {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.deltaTime);
        }
    }

    private void Dash() {
        if (dashPressed) {
            isDashing = true;
            rb.MovePosition(rb.position + movement * dashSpeed * Time.deltaTime);
            StartDashTimer();
        }
    }

    void StartDashTimer() {
        StartCoroutine(DashTimer());
    }

    IEnumerator DashTimer() {
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
    }
}

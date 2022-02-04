/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Player : Controller {

    [Space(2), Header("Jumping")]
    /* --- Parameters --- */
    [SerializeField, Range(0f, 100f)] protected float maxJump = 20f;
    [SerializeField, Range(0f, 3f)] protected float maxCharge = 0.5f;
    /* --- Properties --- */
    [SerializeField, ReadOnly] private KeyCode jumpKey = KeyCode.Space; // The key used to jump.
    [SerializeField, ReadOnly] public float charge;
    [SerializeField, ReadOnly] public bool canJump;

    [Space(2), Header("Dashing")]
    /* --- Parameters --- */
    [SerializeField] [Range(0f, 40f)] protected float dashSpeed = 20f;
    [SerializeField] [Range(0f, 0.5f)] protected float dashDuration = 0.1f;
    /* --- Properties --- */
    [SerializeField, ReadOnly] private KeyCode dashKey = KeyCode.J; // The key used to perform the action.
    [SerializeField, ReadOnly] public bool canDash;
    [HideInInspector] protected Coroutine dashTimer = null;

    /* --- Overridden Methods --- */
    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.

        // Get the movement.
        moveDirection = Input.GetAxisRaw("Horizontal");

        // Check if the action is currently being performed.
        CheckJump();
        GetJump();

        // Get the action.
        CheckDash();
        GetDash();

    }

    protected override void AirborneFlag() {
        base.AirborneFlag();
        if (charge != 0f) {
            airborneFlag = Airborne.Charging;
        }
    }

    protected override void ActionFlag() {
        base.ActionFlag();
    }

    /* --- Methods --- */
    private void GetJump() {
        if (Input.GetKey(jumpKey) && canJump) {
            charge += Time.deltaTime;
            moveSpeed = 0f;
            if (charge > maxCharge) {
                charge = maxCharge;
            }
        }

        if (Input.GetKeyUp(jumpKey) && canJump) {
            Jump();
        }
    }

    private void GetDash() {
        if (Input.GetKeyDown(dashKey) && canDash) {
            Dash();
        }
    }


    private void CheckJump() {
        // Check that we're on the ground.
        if (!mesh.feetbox.empty && dashTimer == null) {
            canJump = true;
        }
    }

    private void CheckDash() {
        if (!mesh.feetbox.empty && dashTimer == null) {
            canDash = true;
            actionFlag = ActionState.None;
        }
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, body.velocity.y + maxJump * Mathf.Sqrt(charge / maxCharge));
        charge = 0f;
    }

    private void Dash() {

        Vector2 dashVector = new Vector2(Input.GetAxisRaw("Horizontal"), 0f).normalized;
        // body.velocity = Vector2.zero;
        weight = 0f;
        think = false;
        dashTimer = StartCoroutine(IEDash(dashDuration));
        actionFlag = ActionState.PreAction;

        IEnumerator IEDash(float delay) {
            yield return new WaitForSeconds(0.15f);
            body.velocity = dashVector * dashSpeed;
            actionFlag = ActionState.Action;
            int afterImages = 3;
            for (int i = 0; i < afterImages; i++) {
                AfterImage(mesh, delay, 0.5f);
                yield return new WaitForSeconds(delay / (float)afterImages);
            }
            think = true;
            actionFlag = ActionState.None;
            yield return (dashTimer = null);
        }

    }

    private static void AfterImage(Mesh mesh, float delay, float transparency) {
        SpriteRenderer afterImage = new GameObject("AfterImage", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        // afterImage.transform.SetParent(transform);
        afterImage.transform.position = mesh.transform.position;
        afterImage.transform.localRotation = mesh.transform.localRotation;
        afterImage.transform.localScale = mesh.transform.localScale;
        afterImage.sprite = mesh.spriteRenderer.sprite;
        afterImage.color = Color.white * transparency;
        Destroy(afterImage.gameObject, delay);
    }
}

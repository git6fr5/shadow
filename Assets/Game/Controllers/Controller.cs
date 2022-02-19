/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the actions of a character.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour {

    /* --- Enumerations --- */
    // Direction States
    public enum Direction {
        Right,
        Left
    }
    // Movement States
    public enum Movement {
        Idle,
        Moving
    }
    // Falling States
    public enum Airborne {
        Grounded,
        Charging,
        Rising,
        Falling,
        Landing
    }
    // Action States
    public enum ActionState {
        None,
        PreAction,
        Action,
        Scared
    }

    /* --- Components --- */
    [HideInInspector] public Rigidbody2D body; // Handles physics calculations.
    [SerializeField] protected Mesh mesh; // Handles the collision frame and animation.

    /* --- Parameters --- */
    [Space(2), Header("Parameters")]
    [SerializeField] public float baseSpeed; // The base speed at which this character moves.
    [SerializeField] public float baseWeight; // The base weight that the character has.
    [SerializeField] public float baseAcceleration; // The base acceleration at which this character moves.

    /* --- Properties --- */
    [Space(2), Header("Properties")]
    [SerializeField, ReadOnly] protected float moveSpeed; // The character's movement speed.
    [SerializeField, ReadOnly] protected float moveDirection; // The direction the character is moving.
    [SerializeField, ReadOnly] protected float weight; // The character's movement speed.

    /* --- Switches --- */
    [Space(2), Header("Switches")]
    [SerializeField, ReadOnly] public bool think; // Whether this character is in control.

    /* --- Flags --- */
    [Space(2), Header("Flags")]
    [SerializeField, ReadOnly] public Movement movementFlag = Movement.Idle; // Flags what type of movement this controller is in.
    [SerializeField, ReadOnly] public Direction directionFlag = Direction.Right; // Flags what type of movement this controller is in.
    [SerializeField, ReadOnly] public Airborne airborneFlag = Airborne.Grounded; // Flags what type of movement this controller is in
    [SerializeField, ReadOnly] public ActionState actionFlag = ActionState.None; // Flags what type of movement this controller is in

    /* --- Debug --- */
    [Space(2), Header("Debug")]
    [HideInInspector] private Vector3 prevPosition;
    [SerializeField, ReadOnly] private float calculatedSpeed;
    [SerializeField, ReadOnly] private float jumpHeight;
    [SerializeField, ReadOnly] private float jumpDistance;
    [SerializeField, ReadOnly] private Vector3 jumpStartPosition;

    /* --- Unity --- */
    // Runs once before the first frame.
    private void Start() {
        Init();
    }

    // Runs once every frame.
    private void Update() {
        if (think) {
            Think();
        }
        Flag();
    }

    // Runs once every fixed interval.
    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Move(deltaTime);
        DebugMovementValue(deltaTime);
    }

    /* --- Virtual Methods --- */
    // Runs the initialization logic.
    protected virtual void Init() {

        // Cache these components.
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Set up this switch.
        think = true;

        // Set up these values.
        prevPosition = transform.position;
    }

    // Runs the thinking logic.
    protected virtual void Think() {
        weight = baseWeight;
        moveSpeed = baseSpeed;
        moveDirection = 0f;
    }

    // Checks relevant state flags for this controller.
    private void Flag() {
        DirectionFlag();
        MovementFlag();
        AirborneFlag();
    }

    // Moves this controller based on it's input.
    private void Move(float deltaTime) {
        float targetVelocity = moveSpeed * moveDirection;
        if (Mathf.Abs(targetVelocity - body.velocity.x) >= baseAcceleration * deltaTime) {
            float deltaVelocity = Mathf.Sign(targetVelocity - body.velocity.x) * baseAcceleration * deltaTime;
            body.velocity = new Vector2(body.velocity.x + deltaVelocity, body.velocity.y);
        }
        else {
            body.velocity = new Vector2(targetVelocity, body.velocity.y);
        }
    }

    /* --- Flag Methods --- */
    // Flags which direction the controller is facing.
    private void DirectionFlag() {
        if (moveDirection != 0) {
            directionFlag = (moveDirection > 0) ? Direction.Right : Direction.Left;
        }
    }

    // Flags whether this controller is moving.
    private void MovementFlag() {
        movementFlag = Movement.Idle;
        if (moveDirection != 0 && moveSpeed != 0) {
            movementFlag = Movement.Moving;
        }
    }

    // Flags whether this controller is airborne.
    protected virtual void AirborneFlag() {
        body.gravityScale = weight * GameRules.GravityScale;
        if (mesh.feetbox.empty) {
            airborneFlag = Airborne.Rising;
            if (body.velocity.y < 0) {
                airborneFlag = Airborne.Falling;
            }
        }
        else {
            airborneFlag = Airborne.Grounded;
        }
    }

    // Flags whether this controller is airborne.
    protected virtual void ActionFlag() {
        
    }

    /* --- Debug Methods --- */
    private void DebugMovementValue(float deltaTime) {
        // Calculate the movement speed.
        calculatedSpeed = Mathf.Max(calculatedSpeed, Mathf.Abs((transform.position.x - prevPosition.x)) / deltaTime);
        prevPosition = transform.position;

        // Calculate the jump height and distance.
        if (mesh.feetbox.empty) {
            if (airborneFlag == Airborne.Rising) {
                jumpHeight = transform.position.y - jumpStartPosition.y;
            }
            jumpDistance = Mathf.Abs(transform.position.x - jumpStartPosition.x);
        }
        else {
            jumpStartPosition = transform.position;
        }
    }

}

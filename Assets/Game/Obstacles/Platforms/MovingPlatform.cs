/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteShapeController))]
[RequireComponent(typeof(BoxCollider2D))]
public class MovingPlatform : MonoBehaviour {

    /* --- Static Variables --- */
    public static float SlowSpeed = 0.75f;
    public static float MidSpeed = 1.25f;
    public static float FastSpeed = 2.5f;

    /* --- Components --- */
    protected SpriteShapeRenderer spriteShapeRenderer;
    protected SpriteShapeController shape;
    protected BoxCollider2D box;

    /* --- Parameters --- */
    [SerializeField] protected Transform endPoint = null;
    [SerializeField] protected float speed = 0f;
    [SerializeField] private Transform[] pathPoints = null;

    /* --- Properties --- */
    [SerializeField, ReadOnly] private int pathIndex;
    [SerializeField, ReadOnly] protected Vector3 target = Vector3.zero;
    [SerializeField, ReadOnly] protected List<Controller> container = new List<Controller>();


    /* --- Unity --- */
    // Runs once before the first frame.
    private void Start() {
        Init();
    }

    // Runs once every frame.
    private void Update() {
        Target();
        Effect();
    }

    // Runs once every frame.
    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Move(deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Controller controller = collision.gameObject.GetComponent<Bodybox>()?.controller;
        if (controller != null && !container.Contains(controller)) {
            container.Add(controller);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        Controller controller = collision.gameObject.GetComponent<Bodybox>()?.controller;
        if (controller != null && container.Contains(controller)) {
            container.Remove(controller);
        }
    }

    /* --- Virtual Methods --- */
    protected virtual void Effect() {
        //
    }

    /* --- Methods --- */
    public void Init(Transform endPoint, List<Transform> points, float speed) {
        this.pathPoints = points.ToArray();
        this.endPoint = endPoint;
        this.speed = speed;
        Init();
    }

    // Runs the initialization logic.
    private void Init() {

        shape = GetComponent<SpriteShapeController>();
        spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();

        shape.spline.Clear();
        shape.spline.InsertPointAt(0, new Vector3(-.5f, 0f, 0f));
        shape.spline.InsertPointAt(1, endPoint.localPosition + new Vector3(-.5f, 0f, 0f));
        shape.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        shape.spline.SetTangentMode(1, ShapeTangentMode.Continuous);

        box = GetComponent<BoxCollider2D>();
        box.size = new Vector2(endPoint.localPosition.x, 1f);
        box.offset = new Vector2((endPoint.localPosition.x - 1f) / 2f, 0f);

        target = transform.position;

        pathIndex = 0;
        for (int i = 0; i < pathPoints.Length; i++) {
            pathPoints[i].transform.SetParent(null);
        }

    }

    // Sets the target for this platform.
    private void Target() {
        if (pathPoints == null || pathPoints.Length == 0) {
            return;
        }
        
        if ((target - transform.position).sqrMagnitude < GameRules.MovementPrecision * GameRules.MovementPrecision) {
            pathIndex = (pathIndex + 1) % pathPoints.Length;
            target = pathPoints[pathIndex].position;
        }
        Debug.DrawLine(transform.position, transform.position + Vector3.right, Color.white);

    }

    // Moves this platform.
    private void Move(float deltaTime) {
        Vector3 velocity = (target - transform.position).normalized * speed;
        transform.position += velocity * deltaTime;
        for (int i = 0; i < container.Count; i++) {
            container[i].transform.position += velocity * deltaTime;
        }
    }



}

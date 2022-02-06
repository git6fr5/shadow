/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Bodybox : Container {

    /* --- Parameters --- */
    [SerializeField] private string enemy = "";

    /* --- Properties --- */
    [SerializeField] public Controller controller = null;

    [Space(2), Header("Ground")]
    [HideInInspector] private string ground = GameRules.GroundTag;
    [SerializeField] public bool emptySpace = false; // => CheckSpace();

    // Initializes the script.
    protected override void Init() {
        base.Init(); // Runs the base initialization.
        target = enemy;
        collisionFrame.isTrigger = false;
    }

    void Update() {
        emptySpace = CheckSpace();
    }

    /* --- Overridden Events --- */
    private bool CheckSpace() {
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position + (Vector3)circleCollider.offset, circleCollider.radius * 0.85f);
        for (int i = 0; i < collisions.Length; i++) {
            if (collisions[i].tag == ground) {
                return false;
            }
        }
        return true;
    }

    public override void OnAdd(Collider2D collider) {
    }

    public override void OnRemove(Collider2D collider) {
    }

    void OnDrawGizmos() {
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        Gizmos.DrawWireSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius * 0.85f);
    }

}

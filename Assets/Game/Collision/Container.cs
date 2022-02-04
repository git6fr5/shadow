/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores current collision data
/// For objects labeled with the targeted tag.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Container : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected Collider2D collisionFrame;

    /* --- Properties --- */
    // A container to store all objects currently in contact with.
    [SerializeField] private List<Collider2D> container = new List<Collider2D>();
    [HideInInspector] protected string target;

    /* --- Calls--- */
    public bool empty => container.Count == 0;

    /* --- Unity --- */
    private void Start() {
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        Add(collider);
    }

    private void OnTriggerExit2D(Collider2D collider) {
        Remove(collider);
    }

    /* --- Virtual Methods --- */
    // Initializes the script.
    protected virtual void Init() {
        collisionFrame = GetComponent<Collider2D>();
        collisionFrame.isTrigger = true;
    }

    /* --- Methods --- */
    private void Add(Collider2D collider) {
        if (!container.Contains(collider) && collider.tag == target) {
            container.Add(collider);
            OnAdd(collider);
        }
    }

    private void Remove(Collider2D collider) {
        if (container.Contains(collider)) {
            container.Remove(collider);
            OnRemove(collider);
        }
    }

    /* --- Virtual Events --- */
    public virtual void OnAdd(Collider2D collider) { 
        
    }

    public virtual void OnRemove(Collider2D collider) {

    }


}

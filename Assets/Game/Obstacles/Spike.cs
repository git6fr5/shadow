/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores current collision data
/// For objects labeled with the targeted tag.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Spike : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] public Collider2D collisionFrame;

    /* --- Unity --- */
    private void Start() {
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        Add(collider);
    }

    /* --- Virtual Methods --- */
    // Initializes the script.
    protected virtual void Init() {
        collisionFrame = GetComponent<Collider2D>();
        collisionFrame.isTrigger = true;
    }

    /* --- Methods --- */
    private void Add(Collider2D collider) {
        Player player = collider.GetComponent<Bodybox>()?.controller.GetComponent<Player>();
        if (player) {
            player.Scare(transform.position);
        }
    }

}

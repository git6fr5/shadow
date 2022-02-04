/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Hurtbox : Container {

    /* --- Parameters --- */
    [SerializeField] private string enemy = "";

    /* --- Properties --- */
    [SerializeField] public Controller controller = null;

    // Initializes the script.
    protected override void Init() {
        base.Init(); // Runs the base initialization.
        target = enemy;
    }

    /* --- Overridden Events --- */
    public override void OnAdd(Collider2D collider) {
    }

    public override void OnRemove(Collider2D collider) {

    }

}

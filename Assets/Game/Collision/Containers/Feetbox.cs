/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Feetbox : Container {
    
    // Initializes the script.
    protected override void Init() {
        base.Init(); // Runs the base initialization.
        target = GameRules.GroundTag;
    } 

}

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */

/// <summary>
/// 
/// </summary>
[ExecuteInEditMode]
public class GridPositioner : MonoBehaviour {

    /* --- Parameters --- */
    [SerializeField] public bool snap = true;

    /* --- Unity --- */
    // Runs once every frame.
    private void Update() {
        if (snap) {
            SnapPositions();
        }
    }

    /* --- Methods --- */
    private void SnapPositions() {
        Transform[] transforms = (Transform[])GameObject.FindObjectsOfType((typeof(Transform)));
        for (int i = 0; i < transforms.Length; i++) {
            if (transforms[i].parent == null && transforms[i].GetComponent<Camera>() == null) {
                transforms[i].position = SnapPosition(transforms[i].position);
            }
        }
    }

    private Vector3 SnapPosition(Vector3 position) {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), 0f);
    }

}

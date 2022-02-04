/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Lamp : MonoBehaviour {

    [SerializeField] private float angle = 30f;

    void Update() {



    }

    void OnDrawGizmos() {

        Vector3 a = Quaternion.Euler(0f, 0f, angle) * Vector3.down;
        Vector3 b = Quaternion.Euler(0f, 0f, -angle) * Vector3.down;

        Gizmos.DrawLine(transform.position, a);
        Gizmos.DrawLine(transform.position, b);

    }

}

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Firefly : MonoBehaviour {

    [SerializeField] public float speed;
    [SerializeField] public float radius;

    [SerializeField, ReadOnly] private Vector3 origin;
    [SerializeField, ReadOnly] private Vector3 target;
    
    // Runs once before the first frame.
    void Start() {
        origin = transform.position;
        StartCoroutine(IEPath());
    }

    // Runs once every frame.
    void Update() {
        Vector3 velocity = (target - origin).normalized * speed * Time.deltaTime;
        transform.position += velocity;
        transform.localRotation = velocity.x > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }

    private IEnumerator IEPath() {
        while (true) {
            target = origin + radius * (Vector3)Random.insideUnitCircle;
            yield return new WaitForSeconds(3f);
        }
    }

}

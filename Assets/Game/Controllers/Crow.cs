/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Crow : Controller {

    /* --- Components --- */
    [SerializeField] private Transform eyes;

    /* --- Parameters --- */
    [SerializeField] private float angle = 30f;
    [SerializeField] private float range = 3f;

    /* --- Properties --- */
    [SerializeField] private int precision = 50;
    [HideInInspector] private List<Vector3> lightPath;

    /* --- Overridden Methods --- */
    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.

        float deltaTime = Time.deltaTime;
        bool foundPlayer = CheckForPlayer(deltaTime);
        if (foundPlayer) {
            print("Found Player");
            GameRules.MainPlayer.Scare(mesh.transform.position);
        }
    }

    public bool CheckForPlayer(float deltaTime) {

        lightPath = new List<Vector3>();
        bool foundPlayer = false;
        for (int i = 0; i < precision; i++) {

            // Set up the ray.
            Vector3 start = eyes.position;
            Vector3 baseDirection = directionFlag == Controller.Direction.Right ? Vector3.right : Vector3.left;
            Vector3 direction = Quaternion.Euler(0f, 0f, -angle + i * (2f * angle / precision)) * baseDirection;
            float distance = range;

            // Cast the ray.
            RaycastHit2D hit = Physics2D.Raycast(start, direction, range);
            if (hit.collider != null && hit.collider != mesh.bodybox.collisionFrame) {
                print("Hitting Something " + hit.collider.name);
                distance = (transform.position - (Vector3)hit.point).magnitude;
                Player player = hit.collider.GetComponent<Bodybox>()?.controller.GetComponent<Player>();
                if (player != null) {
                    foundPlayer = true;
                }
            }

            lightPath.Add(distance * direction);
            Debug.DrawLine(start, start + distance * direction, Color.yellow, deltaTime);

        }

        for (int i = 0; i < precision; i++) {

            // Set up the ray.
            Vector3 start = mesh.transform.position;
            Vector3 direction = Quaternion.Euler(0f, 0f, i * 360f / precision) * Vector3.down;
            float distance = 0.5f;

            // Cast the ray.
            RaycastHit2D hit = Physics2D.Raycast(start + direction * distance, -direction, distance);
            if (hit.collider != null) {
                distance = (transform.position - (Vector3)hit.point).magnitude;
                Player player = hit.collider.GetComponent<Bodybox>()?.controller.GetComponent<Player>();
                if (player != null) {
                    foundPlayer = true;
                }
            }

            lightPath.Add(distance * direction);
            Debug.DrawLine(start, start + distance * direction, Color.yellow, deltaTime);

        }

        return foundPlayer;
    }

    /* --- Methods --- */
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

}

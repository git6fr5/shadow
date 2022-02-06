/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// 
/// </summary>
public class Lamp : MonoBehaviour {

    /* --- Static Variables --- */
    // Flicker Shutters.
    public static int MinFlickerShutters = 5;
    public static int MaxFlickerShutters = 20;
    // Flicker Shutter Interval.
    public static float MinFlickerShutterInterval = 0.01f;
    public static float MaxFlickerShutterInterval = 0.05f;
    // Flicker Interval.
    public static float MinFlickerInterval = 1f;
    public static float MaxFlickerInterval = 3f;

    /* --- Components --- */
    [SerializeField] public Light2D glow;
    [SerializeField] public Light2D cone;

    /* --- Parameters --- */
    [SerializeField] private float angle = 30f;
    [SerializeField] private float range = 3f;
    [SerializeField] private int precision = 50;
    [SerializeField] private float glowGrowRate = 0.95f;

    /* --- Properties --- */
    [HideInInspector] private List<Vector3> lightPath;
    [HideInInspector] private Coroutine glowTimer = null;


    /* --- Unity --- */
    private void Start() {
        Flicker();
    }

    private void Update() {
        Glow();
    }

    /* --- Methods --- */
    private void Flicker() {

        StartCoroutine(IEFlicker());
        IEnumerator IEFlicker() {
            while (true) {
                int shutters = Random.Range(MinFlickerShutters, MaxFlickerShutters);
                for (int i = 0; i < 15; i++) {
                    cone.enabled = !cone.enabled;
                    float shutterInterval = Random.Range(MinFlickerShutterInterval, MaxFlickerShutterInterval);
                    yield return new WaitForSeconds(shutterInterval);
                }
                cone.enabled = true;
                float interval = Random.Range(MinFlickerInterval, MaxFlickerInterval);
                yield return new WaitForSeconds(interval);
            }
        }
    }

    private void Glow() {

        // glow.radius *= glowGrowRate;

        if (glowTimer == null) {
            StartCoroutine(IEGlow());
        }

        IEnumerator IEGlow() {
            while (true) {
                glowGrowRate *= -1f;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public bool CheckForPlayer(float deltaTime) {

        lightPath = new List<Vector3>();
        bool foundPlayer = false;
        for (int i = 0; i < precision; i++) {

            // Set up the ray.
            Vector3 start = transform.position;
            Vector3 direction = Quaternion.Euler(0f, 0f, -angle + i * (2f * angle/precision)) * Vector3.down;
            float distance = range;

            // Cast the ray.
            RaycastHit2D hit = Physics2D.Raycast(start, direction, range);
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

        for (int i = 0; i < precision; i++) {

            // Set up the ray.
            Vector3 start = transform.position;
            Vector3 direction = Quaternion.Euler(0f, 0f, i * 360f / precision) * Vector3.down;
            float distance = 0.5f;

            // Cast the ray.
            RaycastHit2D hit = Physics2D.Raycast(start, direction, distance);
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

}

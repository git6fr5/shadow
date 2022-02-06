/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[DefaultExecutionOrder(-1000)]
public class GameRules : MonoBehaviour {

    /* --- Static Tags --- */
    public static string PlayerTag = "Player";
    public static string GroundTag = "Ground";

    /* --- Static Objects --- */
    // Player.
    public static Player MainPlayer;
    public static bool PlayerInLight;
    // Camera.
    public static UnityEngine.Camera MainCamera;

    /* --- Static Variables --- */
    // Mouse.
    public static Vector3 MousePosition => MainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
    // Movement.
    public static float VelocityDamping = 0.95f;
    public static float MovementPrecision = 0.05f;
    public static float GravityScale = 1f;
    // Animation.
    public static float FrameRate = 8f;
    public static float OutlineWidth = 1f / 16f;

    /* --- Debug --- */
    [SerializeField] private float timeScale;
    [SerializeField, ReadOnly] private bool playerInLight;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        Init();
    }

    // Runs once every frame.
    void Update() {

        // Check out of bounds.
        float deltaTime = Time.deltaTime;
        PlayerInLight = InLight(deltaTime);

        // Check out of bounds.
        if (OutOfBounds()) {
            ResetLevel();
        }

        playerInLight = PlayerInLight;
        Time.timeScale = timeScale;

    }

    /* --- Methods --- */
    private void Init() {
        MainPlayer = (Player)GameObject.FindObjectOfType(typeof(Player));
        MainCamera = Camera.main;
    }

    /* --- Static Methods --- */
    private static bool InLight(float deltaTime) {
        Lamp[] lamp = (Lamp[])GameObject.FindObjectsOfType(typeof(Lamp));
        for (int i = 0; i < lamp.Length; i++) {
            if (lamp[i].CheckForPlayer(deltaTime)) {
                return true;
            }
        }
        return false;
    }

    private static bool OutOfBounds() {
        return false;
    }

    private static void ResetLevel() {

    }

}

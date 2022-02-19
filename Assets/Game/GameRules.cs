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
    // Loader.
    public static LevelLoader MainLoader;

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

        if (!CheckRules()) {
            return;
        }

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
        MainLoader = (LevelLoader)GameObject.FindObjectOfType(typeof(LevelLoader));
        MainCamera = Camera.main;
    }

    public static void Init(Player player, LevelLoader loader, Camera camera) {
        MainPlayer = player;
        MainLoader = loader;
        MainCamera = camera;
    }

    public bool CheckRules() {
        if (MainPlayer == null) {
            return false;
        }
        if (MainLoader == null) {
            return false;
        }
        if (MainCamera == null) {
            return false;
        }
        return true;
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
        Vector3 position = MainPlayer.transform.position;
        print(position);
        if (position.x < 0f || position.x > 16f) {
            return true;
        }
        if (position.y > 1f || position.y < -12f) {
            return true;
        }
        return false;
    }

    public static void NextLevel() {
        if (MainLoader != null) {
            if (MainLoader.GetLevelByID(MainLoader.lDtkData, MainLoader.id + 1) != null) {
                MainLoader.id += 1;
            }
            MainLoader.load = true;
        }
        else {
            print("Resetting Level");
        }
    }

    public static void ResetLevel() {
        if (MainLoader != null) {
            MainLoader.load = true;
        }
        else {
            print("Resetting Level");
        }
    }

}

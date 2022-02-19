using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

/// <summary>
/// Stores specific data on how to generate the level.
/// </summary>
public class Environment : MonoBehaviour {

    /* --- Components --- */
    // Entities.
    [SerializeField] public Transform animalParentTransform; // The location to look for the entities.
    [SerializeField] public Transform obstacleParentTransform; // The location to look for the entities.
    [SerializeField] public Transform decorationParentTransform; // The location to look for the entities.
    // Tiles.
    [SerializeField] public RuleTile platformTile; // A set of sprites used to tile the floor of the level.
    [SerializeField] public RuleTile floorTile; // A set of sprites used to tile the floor of the level.
    [SerializeField] public RuleTile borderTile; // A set of sprites used to tile the floor of the level.

    /* --- Properties --- */
    [SerializeField, ReadOnly] public List<Entity> animals; // The set of entities found from the parent transform.
    [SerializeField, ReadOnly] public List<Entity> obstacles; // The set of entities found from the parent transform.
    [SerializeField, ReadOnly] public List<Entity> decorations; // The set of entities found from the parent transform.

    [SerializeField] public AudioSource backgroundMusic;
    [SerializeField] public AudioSource ambience;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        RefreshTiles();
        RefreshEntities();
    }

    /* --- Tile Methods --- */
    public void RefreshTiles() {
        // floorTile = (FloorTile)ScriptableObject.CreateInstance(typeof(FloorTile));
        // floorTile.Init(floorSprites);
        // floorTile = floorSprites;
    }

    /* --- Entity Methods --- */
    // Refreshes the set of entities.
    void RefreshEntities() {
        animals = new List<Entity>();
        obstacles = new List<Entity>();
        decorations = new List<Entity>();
        foreach (Transform child in animalParentTransform) {
            FindAllEntitiesInTransform(child, ref animals);
        }
        foreach (Transform child in obstacleParentTransform) {
            FindAllEntitiesInTransform(child, ref obstacles);
        }
        foreach (Transform child in decorationParentTransform) {
            FindAllEntitiesInTransform(child, ref decorations);
        }
    }

    // Recursively searches through the transform for all entity components.
    void FindAllEntitiesInTransform(Transform parent, ref List<Entity> entityList) {

        // If we've found an entity, don't go any deeper.
        if (parent.GetComponent<Entity>() != null) {
            entityList.Add(parent.GetComponent<Entity>());
        }
        else if (parent.childCount > 0) {
            foreach (Transform child in parent) {
                FindAllEntitiesInTransform(child, ref entityList);
            }
        }
    }

    // Returns the first found entity with a matching ID.
    public Entity GetEntityByVectorID(Vector2Int vectorID, List<Entity> entityList) {
        for (int i = 0; i < entityList.Count; i++) {
            if (entityList[i].vectorID == vectorID) {
                return entityList[i];
            }
        }
        return null;
    }

}

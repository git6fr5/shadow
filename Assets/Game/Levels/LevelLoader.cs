/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using LDtkUnity;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;

/// <summary>
/// Loads a level from the lDtk Data into the level using the environment.
/// </summary>
public class LevelLoader : MonoBehaviour {

    /* --- Static Variables --- */
    // Layer Names
    public static string ControlLayer = "Controls";
    public static string AnimalLayer = "Animals";
    public static string ObstacleLayer = "Obstacles";
    public static string DecorationLayer = "Decorations";
    public static string PlatformLayer = "Platform";
    public static string FloorLayer = "Floor";
    public static string BorderLayer = "Border";

    /* --- Data Structures --- */
    public class LDtkTileData {

        /* --- Properties --- */
        public Vector2Int vectorID;
        public Vector2Int gridPosition;
        public int index;

        /* --- Constructor --- */
        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index = 0) {
            this.vectorID = vectorID;
            this.gridPosition = gridPosition;
            this.index = index;
        }

    }

    /* --- Components --- */
    [SerializeField] public LDtkComponentProject lDtkData;
    [SerializeField] public Level level;

    /* --- Parameters --- */
    [SerializeField] public bool load;
    [SerializeField] public int id;


    /* --- Unity --- */
    // Runs once before the first frame.
    private void Update() {
        if (load) {
            ResetLevel(level);
            OpenLevel(id);
            load = false;
        }
    }

    /* --- Methods --- */
    private void OpenLevel(int id) {
        LDtkLevel ldtkLevel = GetLevelByID(lDtkData, id);
        OpenLevel(ldtkLevel);
    }

    public LDtkLevel GetLevelByID(LDtkComponentProject lDtkData, int id) {

        // Get the json file from the LDtk Data.
        LdtkJson json = lDtkData.FromJson();

        // Read the json data.
        level.gridSize = (int)json.DefaultGridSize;
        level.height = (int)(json.DefaultLevelHeight / json.DefaultGridSize);
        level.width = (int)(json.DefaultLevelWidth / json.DefaultGridSize);

        // Grab the level by the id.
        if (id < json.Levels.Length && id >= 0) {
            return json.Levels[id];
        }
        Debug.Log("Could not find room");
        return null;
    }

    protected void OpenLevel(LDtkLevel ldtkLevel) {

        if (ldtkLevel != null) {

            // Load the entity data.
            int gridSize = level.gridSize;
            List<LDtkTileData> controlData = LoadLayer(ldtkLevel, ControlLayer, gridSize);
            List<LDtkTileData> animalData = LoadLayer(ldtkLevel, AnimalLayer, gridSize);
            List<LDtkTileData> obstacleData = LoadLayer(ldtkLevel, ObstacleLayer, gridSize);
            List<LDtkTileData> decorationData = LoadLayer(ldtkLevel, DecorationLayer, gridSize);
            List<LDtkTileData> platformData = LoadLayer(ldtkLevel, PlatformLayer, gridSize);
            List<LDtkTileData> floorData = LoadLayer(ldtkLevel, FloorLayer, gridSize);
            List<LDtkTileData> borderData = LoadLayer(ldtkLevel, BorderLayer, gridSize);

            // Grab the data from the level.
            List<Entity> animalList = level.environment.animals;
            List<Entity> obstacleList = level.environment.obstacles;
            List<Entity> decorationList = level.environment.decorations;

            Tilemap platformMap = level.platformMap;
            Tilemap floorMap = level.floorMap;
            Tilemap borderMap = level.borderMap;

            // Instatiantate and set up the entities using the data.
            level.animals = LoadEntities(animalData, animalList);
            level.obstacles = LoadEntities(obstacleData, obstacleList);
            level.decorations = LoadEntities(decorationData, decorationList);

            level.platformTilePositions = LoadTiles(level.environment.platformTile, platformMap, platformData);
            level.floorTilePositions = LoadTiles(level.environment.floorTile, floorMap, floorData);
            level.borderTilePositions = LoadTiles(level.environment.borderTile, borderMap, borderData);

            // Generate the shadows.
            level.floorShadow.Generate(0.025f);
            SetControls(level, controlData);

            Player player = (Player)GameObject.FindObjectOfType(typeof(Player));
            Camera camera = Camera.main;
            GameRules.Init(player, this, Camera.main);
        }

    }

    private void ResetLevel(Level level) {

        if (level.animals != null) {
            print("Resetting Entities");
            for (int i = 0; i < level.animals.Count; i++) {
                Destroy(level.animals[i].gameObject);
            }
        }
        level.animals = new List<Entity>();

        if (level.obstacles != null) {
            print("Resetting Obstacles");
            for (int i = 0; i < level.obstacles.Count; i++) {
                if (level.obstacles[i] != null) {
                    Destroy(level.obstacles[i].gameObject);
                }
            }
        }
        level.obstacles = new List<Entity>();

        if (level.decorations != null) {
            print("Resetting Obstacles");
            for (int i = 0; i < level.decorations.Count; i++) {
                if (level.decorations[i] != null) {
                    Destroy(level.decorations[i].gameObject);
                }
            }
        }
        level.decorations = new List<Entity>();

        if (level.platformTilePositions != null) {
            print("Resetting Tiles");
            for (int i = 0; i < level.platformTilePositions.Count; i++) {
                level.platformMap.SetTile(level.platformTilePositions[i], null);
            }
        }

        if (level.floorTilePositions != null) {
            print("Resetting Tiles");
            for (int i = 0; i < level.floorTilePositions.Count; i++) {
                level.floorMap.SetTile(level.floorTilePositions[i], null);
            }
        }

        if (level.borderTilePositions != null) {
            print("Resetting Tiles");
            for (int i = 0; i < level.borderTilePositions.Count; i++) {
                level.borderMap.SetTile(level.borderTilePositions[i], null);
            }
        }

        level.platformTilePositions = new List<Vector3Int>();
        level.floorTilePositions = new List<Vector3Int>();
        level.borderTilePositions = new List<Vector3Int>();

    }

    private LDtkUnity.LayerInstance GetLayer(LDtkUnity.Level ldtkLevel, string layerName) {
        // Itterate through the layers in the level until we find the layer.
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer && layer.Identifier == layerName) {
                return layer;
            }
        }
        return null;
    }

    // Returns the vector ID's of all the tiles in the layer.
    private List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName, int gridSize, List<LDtkTileData> layerData = null) {

        if (layerData == null) { layerData = new List<LDtkTileData>(); }

        LDtkUnity.LayerInstance layer = GetLayer(ldtkLevel, layerName);
        if (layer != null) {
            // Itterate through the tiles in the layer and get the directions at each position.
            for (int index = 0; index < layer.GridTiles.Length; index++) {

                // Get the tile at this index.
                LDtkUnity.TileInstance tile = layer.GridTiles[index];

                // Get the position that this tile is at.
                Vector2Int gridPosition = tile.UnityPx / gridSize;
                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / gridSize;

                // Construct the data piece.
                LDtkTileData tileData = new LDtkTileData(vectorID, gridPosition, index);
                layerData.Add(tileData);
            }

        }
        return layerData;
    }

    private List<Entity> LoadEntities(List<LDtkTileData> entityData, List<Entity> entityList) {

        List<Entity> entities = new List<Entity>();

        for (int i = 0; i < entityData.Count; i++) {
            // Get the entity based on the environment.
            Entity entityBase = level.environment.GetEntityByVectorID(entityData[i].vectorID, entityList);
            if (entityBase != null) {

                // Instantiate the entity
                Vector3 entityPosition = level.GridToWorldPosition(entityData[i].gridPosition);
                Entity newEntity = Instantiate(entityBase.gameObject, entityPosition, Quaternion.identity, level.transform).GetComponent<Entity>();
                newEntity.transform.localPosition = entityPosition;
                newEntity.transform.localRotation = entityBase.transform.localRotation;

                // Set up the entity.
                newEntity.gameObject.SetActive(true);
                newEntity.gridPosition = entityData[i].gridPosition;

                // Add the entity to the list
                entities.Add(newEntity);
            }
        }
        return entities;
    }

    // Set all the tiles in a tilemap.
    private List<Vector3Int> LoadTiles(TileBase tile, Tilemap tilemap, List<LDtkTileData> data) {
        List<Vector3Int> nonEmptyTiles = new List<Vector3Int>();
        for (int i = 0; i < data.Count; i++) {
            Vector3Int tilePosition = level.GridToTilePosition(data[i].gridPosition);
            tilemap.SetTile(tilePosition, tile);
            nonEmptyTiles.Add(tilePosition);
        }

        return nonEmptyTiles;
    }

    private Vector2Int? GetTileID(List<LDtkTileData> data, Vector2Int gridPosition) {
        for (int i = 0; i < data.Count; i++) {
            if (gridPosition == data[i].gridPosition) {
                return (Vector2Int?)data[i].vectorID;
            }
        }
        return null;
    }

    private void SetControls(Level level, List<LDtkTileData> controlData) {

        for (int i = 0; i < level.animals.Count; i++) {
            SetCrows(level, controlData, i);
        }

        // Set the obstacles.
        for (int i = 0; i < level.obstacles.Count; i++) {
            MovingPlatform movingPlatform = level.obstacles[i].GetComponent<MovingPlatform>();
            if (movingPlatform != null) {
                Vector2Int gridPosition = level.obstacles[i].gridPosition;
                SetMovingPlatform(movingPlatform, gridPosition, controlData);
            }

        }

    }

    private void SetCrows(Level level, List<LDtkTileData> controlData, int i) {
        Crow crow = level.animals[i].GetComponent<Crow>();
        if (crow != null) {
            Entity crowEntity = level.animals[i];
            for (int j = 0; j < controlData.Count; j++) {
                if (crowEntity.gridPosition == controlData[j].gridPosition) {
                    if (controlData[j].vectorID.x == 0) {
                        crow.directionFlag = Controller.Direction.Right;
                    }
                    if (controlData[j].vectorID.x == 2) {
                        crow.directionFlag = Controller.Direction.Left;
                    }
                }
            }
        }
    }

    private void SetMovingPlatform(MovingPlatform movingPlatform, Vector2Int gridPosition, List<LDtkTileData> controlData) {

        // Get the control point.
        LDtkTileData controlPoint = null;
        for (int i = 0; i < controlData.Count; i++) {
            if (controlData[i].gridPosition == gridPosition) {
                controlPoint = controlData[i];
            }
        }
        if (controlPoint == null) {
            return;
        }
        print("found control point");

        // Get the speed.
        int speedID = controlPoint.vectorID.y;
        float speed = 0f;
        if (speedID == 0) {
            // slow
            speed = MovingPlatform.SlowSpeed;
        }
        else if (speedID == 1) {
            // mid
            speed = MovingPlatform.MidSpeed;
        }
        else if (speedID == 2) {
            // fast
            speed = MovingPlatform.FastSpeed;
        }
        else {
            return;
        }

        // Get the direction.
        int directionID = controlPoint.vectorID.x;
        Vector2Int direction = Vector2Int.zero;
        if (directionID == 0) {
            direction = Vector2Int.right;
        }
        else if (directionID == 1) {
            direction = Vector2Int.up;
        }
        else if (directionID == 2) {
            direction = Vector2Int.left;
        }
        else if (directionID == 3) {
            direction = Vector2Int.down;
        }
        else {
            return;
        }
        print("found direction");

        // Find the second point.
        // Super inefficient.
        LDtkTileData beaconPoint = null;
        Vector2Int beaconID = new Vector2Int(4, 0);
        Vector2Int nextPosition = gridPosition + direction;
        int recursions = 0;
        while (beaconPoint == null && recursions < 50) {
            for (int i = 0; i < controlData.Count; i++) {
                if (controlData[i].gridPosition == nextPosition) {
                    if (controlData[i].vectorID == beaconID) {
                        // Found beacon.
                        beaconPoint = controlData[i];
                        break;
                    }
                }
            }
            recursions += 1;
            nextPosition += direction;
        }
        if (beaconPoint == null) {
            return;
        }
        print("found beacon");

        // Raycast out to find the different blocks.
        MovingPlatform prevPlatform = movingPlatform;
        int size = 1;
        bool didNotFindAnything = false;
        List<MovingPlatform> destroyThesePlatforms = new List<MovingPlatform>();
        recursions = 0;

        while (!didNotFindAnything && recursions < 50) {
            RaycastHit2D[] hits = Physics2D.RaycastAll(prevPlatform.transform.position + Vector3.right * 0.5f, Vector3.right * 0.25f, 1f);
            print(prevPlatform.transform.position);
            didNotFindAnything = true;
            for (int i = 0; i < hits.Length; i++) {
                MovingPlatform nextPlatform = hits[i].collider.GetComponent<MovingPlatform>();
                if (nextPlatform != null && nextPlatform != prevPlatform) {
                    size += 1;
                    prevPlatform = nextPlatform;
                    destroyThesePlatforms.Add(nextPlatform);
                    recursions += 1;
                    didNotFindAnything = false;
                    break;
                }
            }
        }
        for (int i = 0; i < destroyThesePlatforms.Count; i++) {
            Destroy(destroyThesePlatforms[i].gameObject);
        }
        destroyThesePlatforms = null;

        Transform endPoint = new GameObject("End Point").transform;
        endPoint.SetParent(movingPlatform.transform);
        endPoint.localPosition = Vector3.right * size;

        // Create the path points.
        Transform pointA = new GameObject("Point A").transform;
        pointA.position = level.GridToWorldPosition(controlPoint.gridPosition);
        Transform pointB = new GameObject("Point B").transform;
        pointB.position = level.GridToWorldPosition(beaconPoint.gridPosition);
        if (direction.x > 0) {
            pointB.position -= new Vector3(direction.x, 0f, 0f) * (size - 1);
        }


        List<Transform> points = new List<Transform>();
        points.Add(pointA);
        points.Add(pointB);

        movingPlatform.Init(endPoint, points, speed);

    }

}

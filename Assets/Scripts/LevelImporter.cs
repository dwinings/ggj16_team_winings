using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.IO;

namespace Wisp.ElementalDefense {
  public class LevelImporter : AssetPostprocessor {
    private enum Prefab { PEDESTAL, ENTRANCE, EXIT, WALL, FLOOR, GREEN_TOWER, WHITE_TOWER, BLUE_TOWER, ORANGE_TOWER }

    // Save the width and the height of a layer, we need it for a few places.
    private static int width;
    private static int height;

    // These are all prefabs we can instantiate
    private static List<GameObject> floorTiles = new List<GameObject>();
    private static List<GameObject> wallTiles = new List<GameObject>();
    private static GameObject cameraPrefab;
    private static GameObject canvasPrefab;
    private static GameObject pathfindingPrefab;
    private static GameObject pedestalPrefab;
    private static GameObject entrancePrefab;
    private static GameObject exitPrefab;
    private static GameObject levelHolder;
    private static GameObject boardHolder;
    private static GameObject orangeTowerStats;
    private static GameObject blueTowerStats;
    private static GameObject whiteTowerStats;
    private static GameObject greenTowerStats;
    private static GameObject baseTower;

    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
      foreach (var assetPath in importedAssets) {
        if (!assetPath.EndsWith(".tmx")) {
          return;
        }
        ImportTMXLevel(assetPath);
      }
    }

    // This method shows all the directory structure dependencies the level importer has! DON'T MOVE THESE PREFABS WITHOUT UPDATING THIS.
    private static void PopulatePrefabs() {
      cameraPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Main Camera.prefab") as GameObject;
      canvasPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Canvas.prefab") as GameObject;
      pathfindingPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Pathfinding.prefab") as GameObject;
      pedestalPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Towers/TowerPedestal.prefab") as GameObject;
      exitPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/FloorTiles/EnemyExit.prefab") as GameObject;
      entrancePrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/FloorTiles/EnemySpawn.prefab") as GameObject;
      orangeTowerStats = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Towers/OrangeTower.prefab") as GameObject;
      blueTowerStats = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Towers/BlueTower.prefab") as GameObject;
      whiteTowerStats = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Towers/WhiteTower.prefab") as GameObject;
      greenTowerStats = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Towers/GreenTower.prefab") as GameObject;
      baseTower = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Towers/BaseTower.prefab") as GameObject;
      floorTiles.Clear();
      foreach (var file in Directory.GetFiles("Assets/Prefabs/FloorTiles", "Floor*.prefab")) {
        floorTiles.Add(AssetDatabase.LoadAssetAtPath<GameObject>(file));
      }
      wallTiles.Clear();
      foreach (var file in Directory.GetFiles("Assets/Prefabs/FloorTiles", "OuterWall*.prefab")) {
        wallTiles.Add(AssetDatabase.LoadAssetAtPath<GameObject>(file));
      }
    }

    // High level entry point into the importing process.
    public static void ImportTMXLevel(string fileName) {
      width = -1;
      height = -1;
      var mapFile = XDocument.Load(fileName);
      var gidPrefabMap = ReadTileSet(mapFile, fileName);
      var levelName = Path.GetFileNameWithoutExtension(fileName);

      if (GameObject.Find(levelName) != null) {
        GameObject.DestroyImmediate(GameObject.Find(levelName));
      }

      PopulatePrefabs();
      levelHolder = new GameObject(levelName);
      boardHolder = new GameObject("Board");

      foreach (var layer in mapFile.Document.Root.Elements("layer")) {
        SpawnPrefabsFromLayer(layer, gidPrefabMap);
      }

      SpawnLevelBoundary();
      SpawnSceneScaffolding();

      Board board = boardHolder.AddComponent<Board>();
      board.width = width + 2;
      board.height = height + 2;
      boardHolder.tag = "Board";
    }

    // This maps the type properties defined in the tileset to the logical game objects we'd want to create for our level.
    private static Dictionary<uint, Prefab> generateGIDPrefabMap(List<XElement> tiles, uint firstGid) {
      var gidMap = new Dictionary<uint, Prefab>();
      foreach (var tile in tiles) {
        uint gid = Convert.ToUInt32(tile.Attribute("id").Value) + firstGid;
        XElement typeProperty = null;
        var props = tile.Element("properties").Elements("property");
        foreach (var prop in props) {
          if (prop.HasAttributes && prop.Attribute("name").Value == "type") {
            typeProperty = prop;
            break;
          }
        }

        if (typeProperty == null) {
          continue;
        }

        var type = typeProperty.Attribute("value").Value.ToString();
        switch (type) {
          case "pedestal":
            gidMap.Add(gid, Prefab.PEDESTAL);
            break;
          case "exit":
            gidMap.Add(gid, Prefab.EXIT);
            break;
          case "entrance":
            gidMap.Add(gid, Prefab.ENTRANCE);
            break;
          case "floor":
            gidMap.Add(gid, Prefab.FLOOR);
            break;
          case "wall":
            gidMap.Add(gid, Prefab.WALL);
            break;
          case "orange_tower":
            gidMap.Add(gid, Prefab.ORANGE_TOWER);
            break;
          case "white_tower":
            gidMap.Add(gid, Prefab.WHITE_TOWER);
            break;
          case "blue_tower":
            gidMap.Add(gid, Prefab.BLUE_TOWER);
            break;
          case "green_tower":
            gidMap.Add(gid, Prefab.GREEN_TOWER);
            break;
        }
      }
      return gidMap;
    }

    // Iterate through the encoded CSV that defines the tile GIDS, and spawn any prefabs that we recognize for that layer.
    private static void SpawnPrefabsFromLayer(XElement layer, Dictionary<uint, Prefab> gidPrefabMap) {
      var layerWidth = Convert.ToInt32(layer.Attribute("width").Value);
      var layerHeight = Convert.ToInt32(layer.Attribute("height").Value);
      if ((layerWidth != width && width >= 0) || (layerHeight != height && height >= 0)) {
        throw new Exception("Layers have different dimensions, wtf");
      } else {
        height = layerHeight;
        width = layerWidth;
      }

      string rawStr = layer.Element("data").Value.ToString();
      // Xml.Linq adds gross whitespace to the string, so we gotta clear out the empty split elements.
      var gidArray = rawStr.Split(",\n\r".ToCharArray()).Where(s => s.Length > 0).ToList();
      for (int idx = 0; idx < gidArray.Count(); idx++) {
        var gid = Convert.ToUInt32(gidArray[idx]);
        var x = idx % width;
        var y = idx / width;
        var position = translateFromCSV(x, y);
        if (gidPrefabMap.ContainsKey(gid)) {
          SpawnGameObject(gidPrefabMap[gid], position);
        }
      }
    }

    // Not all logical objects want to be instantiated in the same way, so we break them out here to handle that.
    private static void SpawnGameObject(Prefab prefabType, Vector3 position) {
      GameObject instance = null;
      switch (prefabType) {
        case Prefab.PEDESTAL:
          instance = GameObject.Instantiate(pedestalPrefab, position, Quaternion.identity) as GameObject;
          break;
        case Prefab.EXIT:
          instance = GameObject.Instantiate(exitPrefab, position, Quaternion.identity) as GameObject;
          break;
        case Prefab.ENTRANCE:
          instance = GameObject.Instantiate(entrancePrefab, position, Quaternion.identity) as GameObject;
          break;
        case Prefab.FLOOR:
          var floorTile = floorTiles[UnityEngine.Random.Range(0, floorTiles.Count)];
          instance = GameObject.Instantiate(floorTile, position, Quaternion.identity) as GameObject;
          break;
        case Prefab.WALL:
          var wallTile = wallTiles[UnityEngine.Random.Range(0, wallTiles.Count)];
          instance = GameObject.Instantiate(wallTile, position, Quaternion.identity) as GameObject;
          break;
        case Prefab.ORANGE_TOWER:
          InstantiateTowerWithStats(orangeTowerStats, position);
          break;
        case Prefab.BLUE_TOWER:
          InstantiateTowerWithStats(blueTowerStats, position);
          break;
        case Prefab.WHITE_TOWER:
          InstantiateTowerWithStats(whiteTowerStats, position);
          break;
        case Prefab.GREEN_TOWER:
          InstantiateTowerWithStats(greenTowerStats, position);
          break;
        default:
          break;
      }

      if (instance != null) {
        instance.transform.SetParent(boardHolder.transform);
      }
    }

    // Our levels need *walls*, yo!
    private static void SpawnLevelBoundary() {
      for (int y = 0; y < height + 2; y++) {
        for (int x = 0; x < height + 2; x++) {
          if (y == 0 || x == 0 || y == (height + 1) || x == (width + 1)) {
            var instance = GameObject.Instantiate(wallTiles[UnityEngine.Random.Range(0, wallTiles.Count)], new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
            instance.transform.SetParent(boardHolder.transform);
          }
        }
      }
    }

    // The standard components that make up level that *aren't* the board.
    private static void SpawnSceneScaffolding() {
      // var camera = GameObject.Instantiate(cameraPrefab, new Vector3(0f, 0f, -10f), Quaternion.identity) as GameObject;
      // camera.transform.SetParent(levelHolder.transform);
      // var pathfinder = GameObject.Instantiate(pathfindingPrefab, Vector3.zero, Quaternion.identity) as GameObject;
      // pathfinder.transform.SetParent(levelHolder.transform);
      // var canvas = GameObject.Instantiate(canvasPrefab, Vector3.zero, Quaternion.identity) as GameObject;
      // canvas.transform.SetParent(levelHolder.transform);
       boardHolder.transform.SetParent(levelHolder.transform);
      // var cameraMoverScript = camera.GetComponent<CameraMovement>();

      // // if we don't set this we won't be able to set camera bounds and people could zoom out forever
      // // ...
      // // spaaaace
      // cameraMoverScript.boardWidth = width + 2;
      // cameraMoverScript.boardHeight = height + 2;
    }

    // The map can either define the tileset inline, or point to a file.
    // Our files should probably do that latter, but why not handle both?
    // Things will get craaaaay if you add multiple tilesets, but we could
    // augment this method to handle that if it comes to it.
    //
    // Oh, and we need the base path, because the .tsx paths are relative to the .tmx file.
    // Not the best file format definition but eh.
    private static Dictionary<uint, Prefab> ReadTileSet(XDocument root, string basePath) {
      var tilesetNode = root.Document.Root.Element("tileset");
      uint firstGid = Convert.ToUInt32(tilesetNode.Attribute("firstgid").Value);

      // Read tileset from file
      if (tilesetNode.Attribute("source") != null) {
        var path = tilesetNode.Attribute("source").Value;
        path = Path.Combine(Path.GetDirectoryName(basePath), path);
        tilesetNode = XDocument.Load(path).Document.Root;
      }

      var gidPrefabMap = generateGIDPrefabMap(tilesetNode.Elements("tile").ToList(), firstGid);
      return gidPrefabMap;
    }

    // Setting the tower up with its stats is tedious.
    private static GameObject InstantiateTowerWithStats(GameObject statsPrefab, Vector3 position) {
      var statInstance = GameObject.Instantiate(statsPrefab) as GameObject;
      statInstance.transform.SetParent(boardHolder.transform);
      var instance = GameObject.Instantiate(baseTower, position, Quaternion.identity) as GameObject;
      instance.GetComponent<Tower>().ApplyTowerStatsAsleep(statInstance.GetComponent<TowerStats>());
      instance.transform.SetParent(boardHolder.transform);
      return instance;
    }

    // CSV is defined top-left to bottom right, our map is bottom-left to top-right.
    // ALSO we add 2 to our width and height to add an outer wall.
    private static Vector3 translateFromCSV(int x, int y) {
      return new Vector3(x + 1f, height - y, 0f);
    }
  }
}

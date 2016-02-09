using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelImporter : AssetPostprocessor {
  public static int width;
  public static int height;
  public static List<GameObject> floorTiles = new List<GameObject>();
  public static List<GameObject> wallTiles = new List<GameObject>();
  public static GameObject cameraPrefab;
  public static GameObject canvasPrefab;
  public static GameObject pathfindingPrefab;
  public static GameObject pedestalPrefab;
  public static GameObject entrancePrefab;
  public static GameObject exitPrefab;

  static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
    foreach (var assetPath in importedAssets) {
      if (!assetPath.EndsWith(".tmx")) {
        return;
      }
      ImportTMXLevel(assetPath);
    }
  }

  public static void PopulatePrefabs() {
    cameraPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Main Camera.prefab") as GameObject;
    canvasPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Canvas.prefab") as GameObject;
    pathfindingPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Pathfinding.prefab") as GameObject;
    pedestalPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/Towers/TowerPedestal.prefab") as GameObject;
    exitPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/FloorTiles/EnemyExit.prefab") as GameObject;
    entrancePrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/FloorTiles/EnemySpawn.prefab") as GameObject;
    floorTiles.Clear();
    foreach (var file in Directory.GetFiles("Assets/Prefabs/FloorTiles", "Floor*.prefab")) {
      floorTiles.Add(AssetDatabase.LoadAssetAtPath<GameObject>(file));
    }
    wallTiles.Clear();
    foreach (var file in Directory.GetFiles("Assets/Prefabs/FloorTiles", "OuterWall*.prefab")) {
      wallTiles.Add(AssetDatabase.LoadAssetAtPath<GameObject>(file));
    }
  }

  public static void ImportTMXLevel(string fileName) {
    width = -1;
    height = -1;
    PopulatePrefabs();
    var mapFile = XDocument.Load(fileName);
    uint firstGid = Convert.ToUInt32(mapFile.Document.Root.Element("tileset").Attribute("firstgid").Value);
    var gidPrefabMap = generateGIDPrefabMap(mapFile.Descendants("tileset").Elements("tile").ToList(), firstGid);

    Scene levelScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);

    GameObject.Instantiate(cameraPrefab, new Vector3(0f, 0f, -10f), Quaternion.identity);
    var pathfinder = (GameObject.Instantiate(pathfindingPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<AstarPath>();
    GameObject.Instantiate(canvasPrefab, Vector3.zero, Quaternion.identity);
    
    foreach (var layer in mapFile.Document.Root.Elements("layer")) {
      var layerWidth = Convert.ToInt32(layer.Attribute("width").Value);
      var layerHeight = Convert.ToInt32(layer.Attribute("height").Value);
      if ((layerWidth != width && width >= 0) || (layerHeight != height && height >= 0)) {
        throw new Exception("Layers have different dimensions, wtf");
      } else {
        height = layerHeight;
        width = layerWidth;
      }

      string rawStr = layer.Element("data").Value.ToString();
      var gidArray = rawStr.Split(",\n\r".ToCharArray()).Where(s => s.Length > 0).ToList();
      for (int idx = 0; idx < gidArray.Count(); idx++) {
        var gid = Convert.ToUInt32(gidArray[idx]);
        var x = idx % width;
        var y = idx / width;
        var position = translateFromCSV(x, y);
        if (gidPrefabMap.ContainsKey(gid)) {
          GameObject.Instantiate(gidPrefabMap[gid], position, Quaternion.identity);
        }
      }
    }

    for (int y = 0; y < height + 2; y++) {
      for (int x = 0; x < height + 2; x++) {
        if (y == 0 || x == 0 || y == (height + 1) || x == (width + 1)) {
          GameObject.Instantiate(wallTiles[UnityEngine.Random.Range(0, wallTiles.Count)], new Vector3(x, y, 0f), Quaternion.identity);
        } else {
          GameObject.Instantiate(floorTiles[UnityEngine.Random.Range(0, floorTiles.Count)], new Vector3(x, y, 0f), Quaternion.identity);
        }
      }
    }
  }

  public static Dictionary<uint, GameObject> generateGIDPrefabMap(List<XElement> tiles, uint firstGid) {
    var gidMap = new Dictionary<uint, GameObject>();
    foreach (var tile in tiles) {
      uint gid = Convert.ToUInt32(tile.Attribute("id").Value) + firstGid;
      XElement typeProperty = null;
      var props = tile.Element("properties").Elements("property");
      foreach(var prop in props) {
        if (prop.HasAttributes && prop.Attribute("name").Value == "type") {
          typeProperty = prop;
          break;
        }
      }

      if (typeProperty == null) {
        continue;
      }

      var type = typeProperty.Attribute("value").Value.ToString();
      switch(type) {
        case "pedestal":
          gidMap.Add(gid, AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Towers/TowerPedestal.prefab"));
          Debug.Log("pedestal");
          break;
        case "exit":
          gidMap.Add(gid, AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FloorTiles/EnemyExit.prefab"));
          Debug.Log("exit");
          break;
        case "entrance":
          gidMap.Add(gid, AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FloorTiles/EnemySpawn.prefab"));
          Debug.Log("entrance");
          break;
      }
    }
    Debug.Log(gidMap);
    return gidMap;
  }

  public static Vector3 translateFromCSV(int x, int y) {
    return new Vector3(x + 1f, y + 1f, 0f);
  }
}

using System; using System.Collections.Generic;
using UnityEngine;

namespace Wisp.ElementalDefense {
  public class TowerManager : MonoBehaviour {
    public enum TowerTypes : int { ORANGE_TOWER, GREEN_TOWER, WHITE_TOWER, BLUE_TOWER };
    public static TowerManager instance;
    public List<Tower> towers = new List<Tower>();

    private TowerManager() { }

    void Awake() {
      if (instance == null) {
        instance = this;
      } else if (instance != this) {
        Destroy(gameObject);
      }
    }

    public GameObject blankOrangeTower;
    public GameObject blankGreenTower;
    public GameObject blankBlueTower;
    public GameObject blankWhiteTower;

    public static GameObject BlankTowerForType(TowerTypes type) {
      switch (type) {
        case TowerTypes.ORANGE_TOWER:
          return instance.blankOrangeTower;
        case TowerTypes.GREEN_TOWER:
          return instance.blankGreenTower;
        case TowerTypes.WHITE_TOWER:
          return instance.blankWhiteTower;
        case TowerTypes.BLUE_TOWER:
          return instance.blankBlueTower;
        default:
          return instance.blankOrangeTower;
      }
    }
  }
}

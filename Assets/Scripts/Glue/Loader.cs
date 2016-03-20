using UnityEngine;
using System.Collections;

namespace Wisp.ElementalDefense {
  public class Loader : MonoBehaviour {

    public GameManager gameManager;
    public MusicManager musicManager;
    public SFXManager sfxManager;
    public TowerManager towerManager;
    public GameObject mainCameraPrefab;
    public GameObject mainCanvasPrefab;
    public GameObject pathfinderPrefab;


    // Use this for initialization
    void Awake() {
      DontDestroyOnLoad(gameObject);
      LoadManagers();
    }

    public void LoadManagers() {
      if (GameManager.instance == null)
        Instantiate(gameManager);
      if (MusicManager.instance == null)
        Instantiate(musicManager);
      if (SFXManager.instance == null)
        Instantiate(sfxManager);
      if (TowerManager.instance == null)
        Instantiate(towerManager);
    }

    public void LoadLevel(GameObject prefab) {
      Instantiate(prefab);
      Instantiate(mainCameraPrefab, new Vector3(0f, 0f, -10f), Quaternion.identity);
      Instantiate(mainCanvasPrefab);
      Instantiate(pathfinderPrefab);
      GameManager.instance.InitGame();
    }
  }
}

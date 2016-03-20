using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wisp.ElementalDefense;

public class LevelLoader : MonoBehaviour {

  public GameObject levelPrefab;
  
  public void LoadLevel() {
    Loader loader = GameObject.FindGameObjectWithTag("Loader").GetComponent<Loader>();
    Destroy(GameObject.FindGameObjectWithTag("Root"));
    loader.LoadLevel(levelPrefab);
  }

  public void SetSelectedStyle() {
    var text = GetComponentInChildren<Text>();
    text.fontSize = 48;
  }

  public void SetDeselectedStyle() {
    var text = GetComponentInChildren<Text>();
    text.fontSize = 36;
  }
}

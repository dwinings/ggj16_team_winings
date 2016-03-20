using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour {

  private GameObject[] levels;
  public GameObject buttonPrefab;
  public float levelDeltaY;

	// Use this for initialization
	void Start () {
    levels = Resources.LoadAll<GameObject>("Levels");

    int levelIdx = 0;
    foreach (var level in levels) {
      var button = (GameObject)Instantiate(buttonPrefab, new Vector3(1f, 1f, 1f), Quaternion.identity);
      button.SetActive(true);
      button.transform.SetParent(this.transform);
      var buttonText = button.GetComponentInChildren<Text>();
      button.name = level.name;
      button.GetComponent<LevelLoader>().levelPrefab = level;
      buttonText.text = level.name;
      button.transform.localPosition = new Vector3(-260f, -70 + levelDeltaY * levelIdx);

      if (levelIdx == 0) {
        EventSystem.current.SetSelectedGameObject(button);
      }
      levelIdx += 1;
    }
	}
}

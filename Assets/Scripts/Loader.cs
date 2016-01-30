using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

  public GameObject gameManager;
  public MusicManager musicManager;

	// Use this for initialization
  void Awake() {
    if (GameManager.instance == null) {
      Instantiate(gameManager);
    }
    if (MusicManager.instance == null) {
      Instantiate(musicManager);
    } 
	}
}

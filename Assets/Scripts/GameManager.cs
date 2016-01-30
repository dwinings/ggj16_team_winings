using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

  public float levelStartDelay = 2f;
  public float turnDelay;
  public int spawnInterval;
  public int timeTillNextSpawn;
  public static GameManager instance = null;

  private GameObject levelImage;
  public BoardManager boardScript;
  private int level = 1;
  public int playerHitPoints = 100;
  public List<Enemy> enemies;
  private bool enemiesMoving;
  public bool playersTurn = true;
  private bool doingSetup;

  public GameObject spawnPoint;
  public GameObject tower;
  public GameObject exitPoint;

	void Awake() {
    if(instance == null) {
      instance = this;
    } else if(instance != this) {
      Destroy(gameObject);
    }

    DontDestroyOnLoad(gameObject);

    boardScript = GetComponent<BoardManager>();	
    InitGame();
	}

  //This is called each time a scene is loaded.
  void OnLevelWasLoaded(int index)
  {
    //Add one to our level number.
    level++;
    //Call InitGame to initialize our level.
    InitGame();
  }
	
	void InitGame() {
    enemies.Clear();
    boardScript.SetupScene(level);
	}

  public void GameOver() {
    enabled = false;
  }

  IEnumerator MoveEnemies() {
    yield return new WaitForSeconds(turnDelay);
    if (enemies.Count == 0) {
      yield return new WaitForSeconds(turnDelay);
    }

    for (int i = 0; i < enemies.Count; i++) {
      if(enemies[i] == null) {
        enemies.RemoveAt(i);
      } else {
        enemies[i].MoveEnemy();
        yield return new WaitForSeconds(enemies[i].moveTime);
      }
    }
    enemiesMoving = false;
  }

  void Update() {
    if(enemiesMoving) {
      return;
    }

    timeTillNextSpawn -= 1;

    if (timeTillNextSpawn == 0) {
      boardScript.SpawnDude();
      timeTillNextSpawn = spawnInterval;
    }
    enemiesMoving = true;
    StartCoroutine(MoveEnemies());
  }

  public void AddEnemyToList(Enemy script) {
    enemies.Add(script);
  }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

  public float levelStartDelay = 2f;
  public float turnDelay;
  public int spawnInterval;
  private float timeTillNextSpawn;
  public static GameManager instance = null;

  public Text deathText;
  public GameObject deathImage;
  public BoardManager boardScript;
  private int level = 1;
  public int playerHitPoints = 10;
  public int playerCash = 10;
  public List<Enemy> enemies;
  private bool enemiesMoving;
  public bool playersTurn = true;
  private bool doingSetup;

  public Text healthText;
  public Text cashText;

  public GameObject spawnPoint;
  public GameObject tower;
  public GameObject exitPoint;

	void Awake() {
    if(instance == null) {
      instance = this;
      deathImage = GameObject.FindGameObjectWithTag("DeathImage");
      deathText = GameObject.FindGameObjectWithTag("DeathText").GetComponent<Text>();
      healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<Text>();
      cashText = GameObject.FindGameObjectWithTag("CashText").GetComponent<Text>();
      deathImage.SetActive(false);
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
    timeTillNextSpawn = spawnInterval;
    boardScript.SetupScene(level);
	}

  public void GameOver() {
    enabled = false;
    deathText.text = "You have been eaten by a grue";
    deathImage.SetActive(true);
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
      }
    }
    yield return new WaitForSeconds(0.1f);
    enemiesMoving = false;
  }

  void UpdateText() {
    healthText.text = "Energy: " + playerHitPoints;
    cashText.text = "\"Crystals\": " + playerCash;
  }

  void CheckIfGameOver() {
    if (playerHitPoints <= 0) {
      GameOver();
    }
  }

  void Update() {
    UpdateText();
    CheckIfGameOver();
    if(enemiesMoving) {
      return;
    }

    timeTillNextSpawn -= 1f;

    if(boardScript.spawnWave.IsWaveOver() && enemies.Count == 0) {
      boardScript.spawnWave.BeginNextLevel();
      timeTillNextSpawn = 0f;
    } else if (timeTillNextSpawn < float.Epsilon) {
      timeTillNextSpawn = boardScript.SpawnDude();
    }

    enemiesMoving = true;
    StartCoroutine(MoveEnemies());
  }

  public void AddEnemyToList(Enemy script) {
    enemies.Add(script);
  }
}

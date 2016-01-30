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

  public float WAVE_TEXT_DURATION;
  public float WAVE_COOLDOWN_DURATION;
  public GameObject deathImage;
  public BoardManager boardScript;
  private int level = 1;
  public int playerHitPoints = 10;
  public int playerCash = 10;
  public List<Enemy> enemies;
  private bool enemiesMoving;
  private bool waveTransitioning;
  public bool playersTurn = true;
  private bool doingSetup;


  public GameObject spawnPoint;
  public GameObject tower;
  public GameObject exitPoint;

  public Text waveText;
  public Text deathText;
  public Text healthText;
  public Text cashText;

	void Awake() {
    if(instance == null) {
      instance = this;
      spawnPoint = GameObject.FindGameObjectWithTag("Entrance");
      exitPoint = GameObject.FindGameObjectWithTag("Exit");
      deathImage = GameObject.FindGameObjectWithTag("DeathImage");
      waveText = GameObject.FindGameObjectWithTag("WaveText").GetComponent<Text>();
      deathText = GameObject.FindGameObjectWithTag("DeathText").GetComponent<Text>();
      healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<Text>();
      cashText = GameObject.FindGameObjectWithTag("CashText").GetComponent<Text>();
      deathImage.SetActive(false);
      deathText.text = "";
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

  IEnumerator TransitionWave() {

    if(boardScript.spawnWave.level - 1 != 0) {
      deathText.text = "Wave " + (boardScript.spawnWave.level - 1) + ", complete!";
      yield return new WaitForSeconds(WAVE_TEXT_DURATION);
      deathText.text = "";
      yield return new WaitForSeconds(WAVE_COOLDOWN_DURATION);
    }
    deathText.text = "Wave " + boardScript.spawnWave.level + ", prepare yourself!";
    yield return new WaitForSeconds(WAVE_TEXT_DURATION);
    deathText.text = "";

    boardScript.spawnWave.BeginNextLevel();
    timeTillNextSpawn = 0f;
    waveTransitioning = false;
  }

  void Update() {
    UpdateText();
    CheckIfGameOver();
    if(enemiesMoving || waveTransitioning) {
      return;
    }

    timeTillNextSpawn -= 1f;

    if(boardScript.spawnWave.IsWaveOver() && enemies.Count == 0) {
      waveTransitioning = true;
      StartCoroutine(TransitionWave());
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

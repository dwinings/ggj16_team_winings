﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

namespace Wisp.ElementalDefense {
  public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    public int orangeDamage;
    public int blueDamage;
    public int greenDamage;
    public int whiteDamage;

    public float levelStartDelay = 2f;
    public float waveTextDuration;
    public float waveCooldownDuration;

    public GameObject deathImage;
    public BoardInitializer boardScript;
    public float initialHealth;
    public int initialCash;
    public int playerHitPoints;
    public int playerCash;
    public List<Enemy> enemies;

    public Text waveText;
    public Text deathText;
    public Text healthText;
    public Text cashText;
    public GameObject spawnPoint;
    public GameObject exitPoint;
    public GameObject[] towerStats;
    public Transform debuffHolder;
    public Transform towerHolder;
    public Transform blankHolder;
    public Transform errataHolder;
    public Transform audioHolder;

    public Sprite aLittleDamage;
    public Sprite mostlyDamaged;
    public Sprite veryDamaged;

    private Canvas theCanvas;
    private float nextSpawnTime;
    private bool waveTransitioning;
    public GameObject basePallet;
    public GameObject pedestalPallet;

    void Awake() {
      if (instance == null) {
        instance = this;
        InitGame();
      } else if (instance != this) {
        Destroy(gameObject);
      }
    }

    public void InitializePallets() {
      float palletPadding = 75f;
      Vector3 palletStartVector = new Vector3(100f, -50f, 0f);
      int idx = 0;
      foreach (GameObject towerStat in towerStats) {
        TowerStats createdTowerStats = (Instantiate(towerStat) as GameObject).GetComponent<TowerStats>();
        createdTowerStats.gameObject.transform.SetParent(this.errataHolder);
        idx++;
        GameObject instance;
        // The last one is definitely the pedestal pallet;
        if (idx == towerStats.Length) {
          instance = Instantiate(pedestalPallet, palletStartVector, Quaternion.identity) as GameObject;
        } else {
          instance = Instantiate(basePallet, palletStartVector, Quaternion.identity) as GameObject;
        }
        instance.GetComponent<TowerPallet>().ApplyStats(createdTowerStats.GetComponent<TowerStats>());
        palletStartVector.y -= palletPadding;
        instance.transform.SetParent(theCanvas.transform);
      }
    }

    void InitGame() {
      theCanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<Canvas>();
      spawnPoint = GameObject.FindGameObjectWithTag("Entrance");
      exitPoint = GameObject.FindGameObjectWithTag("Exit");
      deathImage = GameObject.FindGameObjectWithTag("DeathImage");
      waveText = GameObject.FindGameObjectWithTag("WaveText").GetComponent<Text>();
      deathText = GameObject.FindGameObjectWithTag("DeathText").GetComponent<Text>();
      healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<Text>();
      cashText = GameObject.FindGameObjectWithTag("CashText").GetComponent<Text>();
      towerHolder = new GameObject("Towers").transform;
      blankHolder = new GameObject("Blanks").transform;
      debuffHolder = new GameObject("Debuffs").transform;
      errataHolder = new GameObject("Etc").transform;
      audioHolder = new GameObject("Audio").transform;

      InitializePallets();

      initialHealth = playerHitPoints;
      deathImage.SetActive(false);
      deathText.text = "";
      boardScript = GetComponent<BoardInitializer>();
      enemies.Clear();
      boardScript.SetupScene(0);
      AstarPath.active.Scan();
    }

    public void GameOver() {
      MusicManager.instance.Stop();
      SFXManager.instance.PlaySoundAt("game_over", this.transform.position);
      enabled = false;
      deathText.text = "Your \"crystals\" have been eaten.";
      deathImage.SetActive(true);
      StartCoroutine(ReturnToMenu());
    }

    IEnumerator ReturnToMenu() {
      yield return new WaitForSeconds(5);
      SceneManager.LoadScene("Menu");
    }

    public void UpdateText() {
      healthText.text = "Energy: " + playerHitPoints;
      cashText.text = "\"Crystals\": " + playerCash;
    }

    void CheckIfGameOver() {
      if (playerHitPoints <= 0) {
        GameOver();
      }
    }

    IEnumerator TransitionWave() {

      if (boardScript.spawnWave.level - 1 != 0) {
        SetText(deathText, "Wave " + (boardScript.spawnWave.level - 1) + ", complete!");
        yield return new WaitForSeconds(waveTextDuration);
        SetText(deathText, "");
      }
      yield return new WaitForSeconds(waveCooldownDuration);
      SetText(deathText, "Wave " + boardScript.spawnWave.level + ", prepare yourself!");
      yield return new WaitForSeconds(waveTextDuration);
      SetText(deathText, "");

      boardScript.spawnWave.BeginNextLevel();
      nextSpawnTime = 0f;
      waveTransitioning = false;
    }

    public void SetText(Text textObject, string text) {
      if (textObject) {
        textObject.text = text;
      }
    }

    public void UpdateCrystal() {
      SpriteRenderer crystalRenderer = exitPoint.GetComponent<SpriteRenderer>();
      float healthRatio = playerHitPoints / initialHealth;
      if (healthRatio < 0.3f) {
        crystalRenderer.sprite = veryDamaged;
      } else if (healthRatio < 0.6f) {
        crystalRenderer.sprite = mostlyDamaged;
      } else if (healthRatio < 0.99f) {
        crystalRenderer.sprite = aLittleDamage;
      }
    }

    public void UpdateEnemies() {
      for (int i = 0; i < enemies.Count; i++) {
        if (enemies[i] == null) {
          enemies.RemoveAt(i);
        }
      }
    }

    public void ProcessInput() {
      if (Input.GetKeyDown(KeyCode.N))
        MusicManager.instance.StartJoke();
      if (Input.GetKeyDown(KeyCode.Escape))
        SceneManager.LoadScene(0);
    }

    void Update() {
      UpdateText();
      UpdateCrystal();
      UpdateEnemies();
      CheckIfGameOver();
      ProcessInput();

      if (!waveTransitioning) {
        MaybeSpawnEnemy();
      }
    }

    public void MaybeSpawnEnemy() {
      if (boardScript.spawnWave.IsWaveOver()) {
        if (enemies.Count == 0) {
          waveTransitioning = true;
          StartCoroutine(TransitionWave());
        }
      } else if (Time.time > nextSpawnTime) {
        nextSpawnTime = boardScript.SpawnDude() + Time.time;
      }
    }

    public void AddEnemyToList(Enemy script) {
      enemies.Add(script);
    }
  }
}

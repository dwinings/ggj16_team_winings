using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count {
		public int minimum;
		public int maximum;

		public Count(int min, int max) {
			minimum = min;
			maximum = max;
		}
	}


	public int columns = 16;
	public int rows = 16;

	public Count wallCount = new Count(5, 9);
	public Count foodCount = new Count(1, 5);
	public GameObject exit;
	public GameObject spawnPoint;
  public GameObject tower;
	public GameObject[] floorTiles;
  public GameObject[] towerTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	public Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3>();
  private Vector3 spawnPosition;
  private Vector3 towerPosition;
  public Wave spawnWave;
  private float timeTillNextSpawn;

	void InitializeList() {
		gridPositions.Clear();
		for (int x = 1; x < columns - 1; x++) {
			for (int y = 1; y < rows - 1; y++) {
				gridPositions.Add(new Vector3 (x, y, 0f));
			}
		}
	}

	void BoardSetup() {
    spawnWave = new Wave();
		boardHolder = new GameObject("Board").transform;
		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
				// Outside of the level
        if (x == -1 || x == columns || y == -1 || y == rows) {
          toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
        } else if (((x == 6) || (x == 7) || (x == 8)) && ((y == 3) || (y == 7))) {
          toInstantiate = towerTiles [Random.Range (0, towerTiles.Length)];
        }
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent(boardHolder);
			}
		}

    spawnWave = new Wave();
	}

	Vector3 RandomPosition() {
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) {
		int objectCount = Random.Range(minimum, maximum + 1);

		for(int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}

  public GameObject GetRandomEnemy() {
    return enemyTiles[Random.Range(0, enemyTiles.Length)];
  }

  // Returns the delay for the nextdude
  public float SpawnDude() {
    GameObject enemyObject =  GetRandomEnemy();
    Enemy enemy = enemyObject.GetComponent<Enemy>();
    GameObject instance = Instantiate(enemyObject, spawnPosition, Quaternion.identity) as GameObject;
    instance.transform.SetParent(boardHolder);
    return spawnWave.generateNextSpawn(enemy.difficulty);
  }

	// Entry Point
	public void SetupScene(int level) {
    spawnPosition = GameManager.instance.spawnPoint.transform.position;
    towerPosition = GameManager.instance.tower.transform.position;
		BoardSetup();
		InitializeList();
	}
}

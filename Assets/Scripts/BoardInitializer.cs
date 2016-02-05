using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardInitializer : MonoBehaviour {

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
  public GameObject baseEnemy;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;
  public GameObject pedestal;

	public Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3>();
  private Vector3 spawnPosition;
  public Wave spawnWave;
  private List<int[]> pedestalPlacement = new List<int[]>();

	void InitializeList() {
		gridPositions.Clear();
    pedestalPlacement.Clear();
		for (int x = 1; x < columns - 1; x++) {
			for (int y = 1; y < rows - 1; y++) {
				gridPositions.Add(new Vector3 (x, y, 0f));
        if (((x == 6) || (x == 7)) && ((y == 3) || (y == 7))) {
          int[] p = new int[2];
          p[0] = x;
          p[1] = y;
          pedestalPlacement.Add(p);
        }
			}
		}
	}

	void BoardSetup() {
    spawnWave = new Wave();
		boardHolder = new GameObject("Board").transform;
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
				// Outside of the level
        if (x == 0 || x == columns - 1 || y == 0 || y == rows - 1) {
          toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
        } 
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent(boardHolder);
			}
		}
    renderPedestals ();
    spawnWave = new Wave();
	}

  void renderPedestals() {
    foreach(int[] i in pedestalPlacement) {
      float x = (float) i[0];
      float y = (float) i[1];
      Vector3 p = new Vector3 (x, y, 7f);
      GameObject instance = Instantiate (pedestal, p, Quaternion.identity) as GameObject;
      instance.transform.SetParent (boardHolder);
    }
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
    GameObject enemyObject = GetRandomEnemy();
    EnemyStats enemyStats = enemyObject.GetComponent<EnemyStats>();
    GameObject instance = Instantiate(baseEnemy, spawnPosition, Quaternion.identity) as GameObject;
    instance.GetComponent<Enemy>().AssignEnemyType(enemyStats);
    instance.transform.SetParent(boardHolder);
    return spawnWave.generateNextSpawn(enemyStats.difficulty);
  }

	// Entry Point
	public void SetupScene(int level) {
    spawnPosition = GameManager.instance.spawnPoint.transform.position;
    InitializeList();
		BoardSetup();
	}
}

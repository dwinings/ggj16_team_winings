using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : MonoBehaviour {

	public Projectile projectile;

	public GameObject blankOrangeTile;
	public GameObject blankGreenTile;
	public GameObject blankBlueTile;
	public GameObject blankWhiteTile;
	public float adjacentDistanceConstant;

	public GameObject[] enemies;
	public float range;


	private GameObject[] allOrangeTowers;
	private GameObject[] AllGreenTowers;
	private GameObject[] AllWhiteTowers;
	private GameObject[] AllBlueTowers;

	private GameObject aBOT;
	private GameObject aBGT;
	private GameObject aBBT;
	private GameObject aBWT;

	private List<int> connectedTowers = new List<int>();

	// Array numbers being given to Projectile
	// Orange = 0
	// Green  = 1
	// White  = 2
	// Blue   = 3

	void Awake () {

    int myVal = 0;
    switch(tag) {
    case "RedTower":
      myVal = 0;
      break;
    case "GreenTower":
      myVal = 1;
      break;
    case "WhiteTower":
      myVal = 2;
      break;
    case "BlueTower":
      myVal = 3;
      break;
    }
    connectedTowers.Add(myVal);
   
		InvokeRepeating ("EnemiesInRange", 2, 1f);
		InvokeRepeating ("SenseTowers", 0, 1f);
	}

	void EnemiesInRange() {
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemies) {
			float distance = (Mathf.Sqrt(Mathf.Pow((transform.position.x - enemy.transform.position.x), 2) + Mathf.Pow((transform.position.y - enemy.transform.position.y), 2)));
			if (distance > 0 && distance < range) {
				LaunchProjectile ();
			}
		}
	}

	public void SenseTowers() {
    allOrangeTowers = GameObject.FindGameObjectsWithTag("OrangeTower");
		if (allOrangeTowers.Length != 0) {
			foreach (GameObject OrangeTower in allOrangeTowers) {
				float distance = (Mathf.Sqrt (Mathf.Pow ((transform.position.x - OrangeTower.transform.position.x), 2) + Mathf.Pow ((transform.position.y - OrangeTower.transform.position.y), 2)));
				if (distance > 0 && distance < adjacentDistanceConstant) {
					if (aBOT == null) {
						aBOT = Instantiate (blankOrangeTile, transform.position, Quaternion.identity) as GameObject;
						connectedTowers.Add (0);
					}
				}
			}
		}
		AllGreenTowers = GameObject.FindGameObjectsWithTag("GreenTower");
		if (AllGreenTowers.Length != 0) {
			foreach (GameObject GreenTower in AllGreenTowers) {
				float distance = (Mathf.Sqrt (Mathf.Pow ((transform.position.x - GreenTower.transform.position.x), 2) + Mathf.Pow ((transform.position.y - GreenTower.transform.position.y), 2)));
				if (distance > 0 && distance < adjacentDistanceConstant) {
					if (aBGT == null) {
						aBGT = Instantiate (blankGreenTile, transform.position, Quaternion.identity) as GameObject;
						connectedTowers.Add (1);
					}
				}
			}
		}
		AllWhiteTowers = GameObject.FindGameObjectsWithTag("WhiteTower");
		if (AllWhiteTowers.Length != 0) {
			foreach (GameObject WhiteTower in AllWhiteTowers) {
				float distance = (Mathf.Sqrt (Mathf.Pow ((transform.position.x - WhiteTower.transform.position.x), 2) + Mathf.Pow ((transform.position.y - WhiteTower.transform.position.y), 2)));
				if (distance > 0 && distance < adjacentDistanceConstant) {
					if (aBWT == null) {
						aBWT = Instantiate (blankWhiteTile, transform.position, Quaternion.identity) as GameObject;
						connectedTowers.Add (2);
					}
				}
			}
		}
		AllBlueTowers = GameObject.FindGameObjectsWithTag("BlueTower");
		if (AllBlueTowers.Length != 0) {
			foreach (GameObject BlueTower in AllBlueTowers) {
				float distance = (Mathf.Sqrt (Mathf.Pow ((transform.position.x - BlueTower.transform.position.x), 2) + Mathf.Pow ((transform.position.y - BlueTower.transform.position.y), 2)));
				if (distance > 0 && distance < adjacentDistanceConstant) {
					if (aBBT == null) {
						aBBT = Instantiate (blankBlueTile, transform.position, Quaternion.identity) as GameObject;
						connectedTowers.Add (3);
					}
				}
			}
		}
	}

  void LaunchProjectile() {
		Projectile shot = Instantiate(projectile, transform.position, Quaternion.identity) as Projectile;
    shot.connectedTowers = new List<int>(connectedTowers);
  }
}

﻿using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public GameObject[] enemies;
	public GameObject closestEnemy;
	public Vector3 closestEnemyLastPosition;

	public float speed;

  private bool will_hit = true;

	// Use this for initialization
	void Start () {
		FindTarget ();
	}

	void FindTarget () {
		enemies = GameObject.FindGameObjectsWithTag("Enemy");

		foreach (GameObject enemy in enemies) {
			float enemyDistance = (Mathf.Sqrt(Mathf.Pow((transform.position.x - enemy.transform.position.x), 2) + Mathf.Pow((transform.position.y - enemy.transform.position.y), 2)));
			if (closestEnemy == null) {
        will_hit = false;
				closestEnemy = enemy;
				closestEnemyLastPosition =  closestEnemy.transform.position;
			}
			float currentClosestEnemyDistance = (Mathf.Sqrt(Mathf.Pow((transform.position.x - closestEnemy.transform.position.x), 2) + Mathf.Pow((transform.position.y - closestEnemy.transform.position.y), 2)));
			if (enemyDistance < currentClosestEnemyDistance) {
				closestEnemy = enemy;
				closestEnemyLastPosition =  closestEnemy.transform.position;
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Enemy") {
      SFXManager.instance.PlaySoundAt("proj_hit", this.transform.position);
			Destroy(this.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		if (closestEnemy) {
			transform.position = Vector2.MoveTowards (transform.position, closestEnemy.transform.position, speed * Time.deltaTime);
			Vector3 diff = transform.position - closestEnemy.transform.position;
			diff.Normalize ();
			float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
		} else if (transform.position == closestEnemyLastPosition) {
      SFXManager.instance.PlaySoundAt("proj_miss", this.transform.position);
			Destroy(this.gameObject);
		} else {
			transform.position = Vector2.MoveTowards (transform.position, closestEnemyLastPosition, speed * Time.deltaTime);
		}
	}
}
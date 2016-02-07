using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {

	public GameObject[] enemies;
	public GameObject closestEnemy;
	public Vector3 closestEnemyLastPosition;

	public float speed;
	public List<TowerStats> connectedTowers = new List<TowerStats>();

	//public float lifeTime;
	private float destroyByTime;

	// Use this for initialization
	void Start () {
    if (!FindTarget()) {
      Destroy(gameObject);
    }
    transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
	}

	private bool FindTarget () {
		enemies = GameObject.FindGameObjectsWithTag("Enemy");

    if (enemies.Length == 0) {
      return false;
    }

		foreach (GameObject enemy in enemies) {
			float enemyDistance = (Mathf.Sqrt(Mathf.Pow((transform.position.x - enemy.transform.position.x), 2) + Mathf.Pow((transform.position.y - enemy.transform.position.y), 2)));
			if (closestEnemy == null) {
				closestEnemy = enemy;
				closestEnemyLastPosition =  closestEnemy.transform.position;
			}
			float currentClosestEnemyDistance = (Mathf.Sqrt(Mathf.Pow((transform.position.x - closestEnemy.transform.position.x), 2) + Mathf.Pow((transform.position.y - closestEnemy.transform.position.y), 2)));
			if (enemyDistance < currentClosestEnemyDistance) {
				closestEnemy = enemy;
				closestEnemyLastPosition =  closestEnemy.transform.position;
			}
		}
    return true;
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Enemy") {
			Destroy(this.gameObject);
		}
	}

  // Update is called once per frame
  void Update() {
    Vector3 targetPosition;
    if (closestEnemy != null) {
      targetPosition = closestEnemy.transform.position;
      closestEnemyLastPosition = targetPosition;
    } else {
      targetPosition = closestEnemyLastPosition;
    }

    targetPosition.z = 0f;

    if (closestEnemy == null && Vector3.Distance(transform.position, targetPosition) < 0.005) {
      SFXManager.instance.PlaySoundAt("proj_miss", this.transform.position);
      Destroy(this.gameObject);
    } else {
      transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
      Vector3 diff = transform.position - targetPosition;
      diff.Normalize();
      float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }
  }
}

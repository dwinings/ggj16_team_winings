using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

	private Vector2 projectileSpawn;
  public GameObject projectile;
	public GameObject[] enemies;
	public float range;

	void Awake () {
    projectileSpawn = transform.position;
    InvokeRepeating ("InRange", 2, 1f);
	}

	void InRange() {
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemies) {
			float distance = (Mathf.Sqrt(Mathf.Pow((transform.position.x - enemy.transform.position.x), 2) + Mathf.Pow((transform.position.y - enemy.transform.position.y), 2)));
			if (distance > 0 && distance < range) {
				LaunchProjectile ();
			}
		}
	}

  void LaunchProjectile() {
    Instantiate (projectile, projectileSpawn, Quaternion.identity);
  }
		
}

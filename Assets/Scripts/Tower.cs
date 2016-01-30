using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

  private Vector2 projectileSpawn;
  public GameObject projectile;
	void Awake () {
    projectileSpawn = transform.position;
    InvokeRepeating ("LaunchProjectile", 2, 1f);
	}

  void LaunchProjectile() {
    Instantiate (projectile, projectileSpawn, Quaternion.identity);
  }
}

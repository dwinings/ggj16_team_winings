using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

  private Vector2 projectileSpawn;
  public GameObject projectile;
	// Use this for initialization
	void Awake () {
    // projectileSpawn = transform.Find ("ProjectileSpawn").transform.position;
    // InvokeRepeating ("LaunchProjectile", 2, 1f);
	}

  void LaunchProjectile() {
    Instantiate (projectile, projectileSpawn, Quaternion.identity);
  }
}

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Tower : MonoBehaviour {

	public Projectile projectile;

	public float adjacentDistanceConstant;
  public float fireSpeed = 1f;

	public GameObject[] enemies;
	public float range;
	public List<TowerManager.TowerTypes> connectedTowers = new List<TowerManager.TowerTypes>();
  public TowerManager.TowerTypes towerType;
  private List<Vector2> directionsRemaining;
  private float cooldownUntil = 0f;

	void Awake () {
    directionsRemaining = new List<Vector2>(new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right });
    connectedTowers.Add(towerType);
    TowerManager.instance.towers.Add(this);
    TowerManager.instance.towers.ForEach(tower => tower.RegenerateAdjacentTowers());
  }

  void Update() {
    if (Time.time >= cooldownUntil) {
      if (MaybeShootHim()) {
        cooldownUntil = Time.time + (1 / fireSpeed);
      } else {
        cooldownUntil = Time.time + 0.1f;
      }
    }
  }

 public bool MaybeShootHim() {
		enemies = GameObject.FindGameObjectsWithTag("Enemy");

    Physics2D.Raycast(transform.position, Vector2.up);
		foreach (GameObject enemy in enemies) {
			float distance = (Mathf.Sqrt(Mathf.Pow((transform.position.x - enemy.transform.position.x), 2) + Mathf.Pow((transform.position.y - enemy.transform.position.y), 2)));
			if (distance > 0 && distance < range) {
				LaunchProjectile();
        return true;
			}
		}
    return false;
	}

	public void RegenerateAdjacentTowers() {
    GameObject[] towerObjects = GameObject.FindGameObjectsWithTag("Tower");
    List<Vector2> toDelete = new List<Vector2>();
    Collider2D ourCollider = GetComponent<Collider2D>();

    ourCollider.enabled = false;
    foreach(Vector2 direction in directionsRemaining) {
      RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, adjacentDistanceConstant);
      foreach(RaycastHit2D hit in hits) {
        if (hit.collider.gameObject.CompareTag("Tower")) {
          Tower tower = hit.collider.gameObject.GetComponent<Tower>();
          connectedTowers.Add(tower.towerType);
          Instantiate(TowerManager.BlankTowerForType(tower.towerType), transform.position, Quaternion.identity);
          toDelete.Add(direction);
        }
      }
    }
    ourCollider.enabled = true;

    // Do this so we skip rays in directions for which we have towers
    toDelete.ForEach(deletion => directionsRemaining.Remove(deletion));
	}

  void LaunchProjectile() {
		Projectile shot = Instantiate(projectile, transform.position, Quaternion.identity) as Projectile;
    shot.connectedTowers = new List<TowerManager.TowerTypes>(connectedTowers);
  }
}

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Tower : MonoBehaviour {

	public GameObject projectile;

	public float adjacentDistanceConstant;
	public GameObject[] enemies;
	public List<TowerStats> connectedTowers = new List<TowerStats>();
  private List<Vector2> directionsRemaining;
  private float cooldownUntil = 0f;
  public TowerStats towerStats;

	void Awake () {
    directionsRemaining = new List<Vector2>(new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right });
  }

  public void ApplyTowerStats(TowerStats stats) {
    this.towerStats = stats;
    GetComponent<SpriteRenderer>().sprite = stats.sprite;
    projectile.GetComponent<SpriteRenderer>().sprite = stats.projectile;
    connectedTowers.Add(towerStats);
    TowerManager.instance.towers.Add(this);
    TowerManager.instance.towers.ForEach(tower => tower.RegenerateAdjacentTowers());
  }

  void Update() {
    if (Time.time >= cooldownUntil) {
      if (MaybeShootHim()) {
        cooldownUntil = Time.time + (1 / towerStats.firingSpeed);
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
			if (distance > 0 && distance < towerStats.range) {
				LaunchProjectile();
        return true;
			}
		}
    return false;
	}

	public void RegenerateAdjacentTowers() {
    List<Vector2> toDelete = new List<Vector2>();
    Collider2D ourCollider = GetComponent<Collider2D>();

    ourCollider.enabled = false;
    foreach(Vector2 direction in directionsRemaining) {
      RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, adjacentDistanceConstant);
      foreach(RaycastHit2D hit in hits) {
        if (hit.collider.gameObject.CompareTag("Tower")) {
          Tower tower = hit.collider.gameObject.GetComponent<Tower>();
          connectedTowers.Add(tower.towerStats);
          GameObject instance = Instantiate(tower.towerStats.blankObject, transform.position, Quaternion.identity) as GameObject;
          instance.transform.SetParent(GameManager.instance.blankHolder);
          toDelete.Add(direction);
        }
      }
    }
    ourCollider.enabled = true;

    // Do this so we skip rays in directions for which we have towers
    toDelete.ForEach(deletion => directionsRemaining.Remove(deletion));
	}

  void LaunchProjectile() {
		GameObject shot = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
    shot.GetComponent<SpriteRenderer>().sprite = towerStats.projectile;
    shot.GetComponent<Projectile>().connectedTowers = new List<TowerStats>(connectedTowers);
  }
}

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Wisp.ElementalDefense {
  public class Tower : MonoBehaviour {

    public GameObject projectile;

    public float adjacentDistanceConstant;
    public GameObject[] enemies;
    public List<TowerStats> connectedTowers = new List<TowerStats>();
    private List<Vector2> directionsRemaining;
    private float cooldownUntil = 0f;
    public TowerStats towerStats;

    void Awake() {
      directionsRemaining = new List<Vector2>(new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right });
      if (towerStats != null) {
        ApplyTowerStats(towerStats);
      }
    }

    public void ApplyTowerStatsAsleep(TowerStats stats) {
      this.towerStats = stats;
      GetComponent<SpriteRenderer>().sprite = stats.sprite;
      projectile.GetComponent<SpriteRenderer>().sprite = stats.projectile;
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

      float minDistance = float.MaxValue;
      Enemy closestEnemy = null;
      foreach (GameObject enemy in enemies) {
        float distance = (Mathf.Sqrt(Mathf.Pow((transform.position.x - enemy.transform.position.x), 2) + Mathf.Pow((transform.position.y - enemy.transform.position.y), 2)));
        if (distance < minDistance) {
          closestEnemy = enemy.GetComponent<Enemy>();
          minDistance = distance;
        }
      }

      if (closestEnemy != null && minDistance < towerStats.range) {
        LaunchProjectile(closestEnemy);
        return true;
      }

      return false;
    }

    public void RegenerateAdjacentTowers() {
      List<Vector2> toDelete = new List<Vector2>();
      Collider2D ourCollider = GetComponent<Collider2D>();

      ourCollider.enabled = false;
      foreach (Vector2 direction in directionsRemaining) {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, adjacentDistanceConstant);
        foreach (RaycastHit2D hit in hits) {
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

    void LaunchProjectile(Enemy enemy) {
      // Oh look I accidentally invented polar coordinates.
      if (towerStats.projectileType == Projectile.Type.MULTI) {
        int shots = Mathf.RoundToInt(towerStats.projectileSizeModifier);
        float cone = 30f;
        float dist = towerStats.range;
        float originAngle = Mathf.Atan2(enemy.transform.position.y - transform.position.y, enemy.transform.position.x - transform.position.x);
        List<Vector3> targets = new List<Vector3>();
        if (shots % 2 == 1) {
          var x = dist * Mathf.Cos(originAngle) + transform.position.x;
          var y = dist * Mathf.Sin(originAngle) + transform.position.y;
          targets.Add(new Vector3(x, y, 0f));
          shots -= 1;
        }

        for (int i = 1; i <= (shots / 2); i++) {
          float anglePerPair = 20f * Mathf.Deg2Rad;
          float[] multipliers = { 1, -1 };
          foreach (var mult in multipliers) {
            float currentAngle = originAngle + (anglePerPair * i * mult);
            var x = dist * Mathf.Cos(currentAngle) + transform.position.x;
            var y = dist * Mathf.Sin(currentAngle) + transform.position.y;
            targets.Add(new Vector3(x, y, 0f));
          }
        }

        foreach (var target in targets) {
          GameObject shot = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
          shot.GetComponent<SpriteRenderer>().sprite = towerStats.projectile;
          var proj = shot.GetComponent<Projectile>();
          proj.connectedTowers = new List<TowerStats>(connectedTowers);
          proj.projectileType = towerStats.projectileType;
          proj.projectileEffectSizeModifier = towerStats.projectileSizeModifier;
          proj.SetTarget(target);
        }
      } else {
        GameObject shot = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        shot.GetComponent<SpriteRenderer>().sprite = towerStats.projectile;
        var proj = shot.GetComponent<Projectile>();
        proj.connectedTowers = new List<TowerStats>(connectedTowers);
        proj.projectileType = towerStats.projectileType;
        proj.projectileEffectSizeModifier = towerStats.projectileSizeModifier;
      }
    }
  }
}

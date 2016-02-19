using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Wisp.ElementalDefense {
  public class Projectile : MonoBehaviour {
    public enum Type { SINGLE, PIERCE, MULTI, ARTILLERY, PULSE }

    public Type projectileType;
    public float projectileEffectSizeModifier;
    public float projectileEffectDeviationModifier;
    public ParticleSystem splashParticle;
    public GameObject[] enemies;
    private List<Enemy> enemiesDamaged = new List<Enemy>();
    public GameObject closestEnemy;
    public Vector3 closestEnemyLastPosition;

    public float speed;
    public List<TowerStats> connectedTowers = new List<TowerStats>();

    // Use this for initialization
    void Start() {
      if (UnitTargeted() && !FindTarget()) {
        Destroy(gameObject);
      }

      // Pulse projectiles aren't really projectiles per se, so we blow it up here so that the sprite never touches the screen.
      if (projectileType == Type.PULSE) {
        DamageEnemiesInRadius(projectileEffectSizeModifier);
        CreateSplashFX(transform.position);
        Destroy(this.gameObject);
      }

      transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    // Update is called once per frame
    void Update() {
      Vector3 targetPosition;
      if (closestEnemy != null && projectileType != Type.MULTI) {
        targetPosition = closestEnemy.transform.position;
        closestEnemyLastPosition = targetPosition;
      } else {
        targetPosition = closestEnemyLastPosition;
      }

      targetPosition.z = 0f;

      if ((closestEnemy == null || PointTargeted()) && Vector3.Distance(transform.position, targetPosition) < 0.05f) {
        if (!PointTargeted()) {
          SFXManager.instance.PlaySoundAt("proj_miss", this.transform.position);
        }
        Destroy(this.gameObject);
      } else {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        Vector3 diff = transform.position - targetPosition;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
      }
    }

    private bool FindTarget() {
      enemies = GameObject.FindGameObjectsWithTag("Enemy");

      if (enemies.Length == 0) {
        return false;
      }

      foreach (GameObject enemy in enemies) {
        float enemyDistance = Vector2.Distance(transform.position, enemy.transform.position);
        if (closestEnemy == null) {
          closestEnemy = enemy;
          closestEnemyLastPosition = closestEnemy.transform.position;
        }
        float currentClosestEnemyDistance = Vector2.Distance(transform.position, closestEnemy.transform.position);
        if (enemyDistance < currentClosestEnemyDistance) {
          closestEnemy = enemy;
          closestEnemyLastPosition = closestEnemy.transform.position;
        }
      }

      return true;
    }

    public void SetTarget(Vector3 pos) {
      closestEnemyLastPosition = pos;
    }

    private void MaybeDamageEnemy(Enemy enemy, float projectileScaler = 1f) {
      if (!enemiesDamaged.Contains(enemy)) {
        enemy.ApplyProjectileEffects(this, projectileScaler);
        enemiesDamaged.Add(enemy);
      }
    }

    public void DamageEnemiesInRadius(float radius, Enemy target = null) {
      var others = Physics2D.OverlapCircleAll(transform.position, projectileEffectSizeModifier);
      foreach (var potentialEnemy in others) {
        if (potentialEnemy.gameObject.CompareTag("Enemy")) {
          var distance = Vector2.Distance(potentialEnemy.transform.position, this.transform.position);
          var blastRadiusRatio = (projectileEffectSizeModifier - distance) / projectileEffectSizeModifier;
          blastRadiusRatio = 1f - (1f - projectileEffectDeviationModifier) * (1f - blastRadiusRatio);
          var enemy = potentialEnemy.GetComponent<Enemy>();
          // Forget all this math if this dude is the primary target.
          if (enemy == target) {
            blastRadiusRatio = 1f;
          }
          MaybeDamageEnemy(enemy, blastRadiusRatio);
        }
      }

    }

    public void OnTriggerEnter2D(Collider2D other) {
      switch (projectileType) {
        case Type.ARTILLERY:
          DamageEnemiesInRadius(projectileEffectSizeModifier, other.GetComponent<Enemy>());
          CreateSplashFX(other.transform.position);
          Destroy(this.gameObject);
          break;
        case Type.PIERCE:
          if (other.tag == "Enemy") {
            MaybeDamageEnemy(other.GetComponent<Enemy>());
          }
          break;
        case Type.MULTI:
          if (other.tag == "Enemy") {
            MaybeDamageEnemy(other.GetComponent<Enemy>());
          }
          Destroy(this.gameObject);
          break;
        default:
          if (other.tag == "Enemy") {
            MaybeDamageEnemy(other.GetComponent<Enemy>());
          }
          Destroy(this.gameObject);
          break;
      }
    }

    public bool UnitTargeted() {
      return !PointTargeted();
    }

    public bool PointTargeted() {
      switch(projectileType) {
        case Type.PIERCE:
          return true;
        case Type.MULTI:
          return true;
        case Type.PULSE:
          return true;
        default:
          return false;
      }
    }

    private void CreateSplashFX(Vector3 position) {
      var fx = Instantiate(splashParticle, position, Quaternion.identity) as ParticleSystem;
      fx.startSize = projectileEffectSizeModifier * 4;
      fx.startColor = connectedTowers[0].color;
    }
  }
}

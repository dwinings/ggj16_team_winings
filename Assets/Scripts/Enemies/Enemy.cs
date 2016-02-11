using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

namespace Wisp.ElementalDefense {
  public class Enemy : MonoBehaviour {

    public enum DamageType { NORMAL, TRUE }

    public Transform gotToCrystalParticleEffect;
    public Transform iAmSlainParticleEffect;

    // Pathfinding tutorial

    private Seeker seeker;
    public Path path;

    private Transform target;
    public EnemyStats enemyStats;
    public GameObject healthBar;
    public GameObject damageNumberPrefab;

    public float tickTime = 1f;
    private float lastTick;
    public float effectDamageMultiplier = 1f;
    public float currentSpeed;

    // There is a baseMaxHitPoints, this can scale
    public float maxHitPoints;
    public float hitPoints;

    private List<Debuff> debuffs = new List<Debuff>();
    private int currentWaypoint;
    private const float nextWaypointDistance = 0.1f;
    private Dictionary<Type, int> debuffDamageThisTick = new Dictionary<Type, int>();

    protected void Start() {
      GameManager.instance.AddEnemyToList(this);
      healthBar = Instantiate(healthBar, transform.position, Quaternion.identity) as GameObject;
      healthBar.transform.SetParent(transform);
      target = GameManager.instance.exitPoint.transform;
      lastTick = Time.time - 1f;
      RefreshPath();
    }

    public void AssignEnemyType(EnemyStats stats) {
      enemyStats = stats;
      this.maxHitPoints = enemyStats.baseMaxHitPoints * GameManager.instance.boardScript.spawnWave.HealthMultiplier();
      this.hitPoints = this.maxHitPoints;
      this.currentSpeed = enemyStats.speed;
      GetComponent<Animator>().runtimeAnimatorController = stats.animatiorController;
    }

    public void Update() {
      if (lastTick + tickTime < Time.time) {
        lastTick = Time.time;
        TickDebuffs();
      }

      healthBar.GetComponent<HealthBar>().UpdatePercent(hitPoints / (float)maxHitPoints);
      Pathfind();
    }

    public void ApplyDebuffDamage(Type debuffType, int damage) {
      int oldDamage;
      if (!debuffDamageThisTick.TryGetValue(debuffType, out oldDamage)) {
        oldDamage = 0;
      }
      debuffDamageThisTick[debuffType] = oldDamage + damage;
    }

    public void TickDebuffs() {
      effectDamageMultiplier = 1f;
      currentSpeed = enemyStats.speed;

      debuffs.RemoveAll(debuff => debuff.IsExpired());
      debuffs.ForEach(debuff => debuff.ApplyToEnemy(this));

      foreach (var entry in debuffDamageThisTick) {
        Type debuff = entry.Key;
        int damage = entry.Value;
        DamageType type = (DamageType)debuff.GetField("damageType").GetValue(null);
        ApplyDamage(damage, type);
      }

      debuffDamageThisTick.Clear();

      List<Color> colors = new List<Color>();
      colors.Add(Color.white);
      debuffs.ForEach(debuff => colors.Add(debuff.debuffColor));
      Color result = CombineColors(colors);
      this.GetComponent<SpriteRenderer>().color = result;
    }

    public static Color CombineColors(List<Color> aColors) {
      Color result = new Color(0f, 0f, 0f, 0f);
      foreach (Color c in aColors) {
        result += c;
      }
      return result / aColors.Count;
    }

    public void MoveEnemy() {
      float xDist = (Mathf.Abs(target.position.x - transform.position.x));
      float yDist = (Mathf.Abs(target.position.y - transform.position.y));

      if (xDist < float.Epsilon && yDist < float.Epsilon) {
        Destroy(this.gameObject);
      }

      Vector3 movementVector = Vector3.MoveTowards(transform.position, target.position, currentSpeed) - transform.position;
      transform.Translate(movementVector);
    }

    public void ApplyDebuff(Debuff debuff, float duration, float intensity) {
      switch (debuff.stackingType) {
        case Debuff.StackingType.DURATION:
          Debuff oldDebuff = debuffs.Find(db => db.GetType() == debuff.GetType());
          if (oldDebuff != null) {
            oldDebuff.expiration += duration;
            Destroy(debuff.gameObject);
          } else {
            debuffs.Add(debuff);
          }
          break;
        case Debuff.StackingType.INTENSITY:
          debuffs.Add(debuff);
          break;
      }
    }

    public void OnDestroy() {
      debuffs.ForEach(debuff => Destroy(debuff.gameObject));
    }

    public void OnTriggerEnter2D(Collider2D other) {
      if (other.tag == "Exit") {
        Instantiate(gotToCrystalParticleEffect, target.position, Quaternion.identity);
        Destroy(this.gameObject);
        GameManager.instance.playerHitPoints -= enemyStats.playerDamage;
        SFXManager.instance.PlaySoundAt("crystal_hit", this.transform.position);
      } else if (other.tag == "Projectile") {
        Projectile proj = other.gameObject.GetComponent<Projectile>();
        List<TowerStats> effects = proj.connectedTowers;
        int totalDamage = 0;

        float damageComboScaler = 1f;
        foreach (TowerStats effect in effects) {
          if (effect.debuff != null) {
            ApplyDebuff(effect.CreateNewDebuff(), effect.duration, effect.intensity);
          }
          totalDamage += (int)(effect.damage * damageComboScaler);
          damageComboScaler = 0.5f;
        }
        ApplyDamage(totalDamage, effects[0].damageType);
      }
    }

    public void ApplyDamage(int damage, DamageType damageType) {
      int realDamage;
      float realMultiplier = 0f;

      switch (damageType) {
        case DamageType.NORMAL:
          realMultiplier = enemyStats.baseDamageMultiplier * effectDamageMultiplier;
          break;
        case DamageType.TRUE:
          realMultiplier = 1;
          break;
      }
      realDamage = (int)(damage * realMultiplier);

      // Debug.Log("Applying " + damage + " damage to target with a " + realMultiplier + " multiplier (" + realDamage + ")");
      hitPoints -= realDamage;

      GameObject damageNumber = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity) as GameObject;
      damageNumber.transform.SetParent(transform);
      Text dnText = damageNumber.GetComponentInChildren<Text>();
      dnText.color = damageType == DamageType.TRUE ? Color.white : new Color(1f, 86f / 255f, 86f / 255f);
      dnText.text = "" + realDamage;

      if (hitPoints <= 0) {
        Instantiate(iAmSlainParticleEffect, transform.position, Quaternion.identity);
        SFXManager.instance.PlaySoundAt("enemy_die_2", this.transform.position);
        GameManager.instance.playerCash += Bounty();
        Destroy(this.gameObject);
      } else {
        SFXManager.instance.PlaySoundAt("proj_hit", this.transform.position);
      }
    }

    public int Bounty() {
      float math = (-0.333f * Mathf.Log((3f * GameManager.instance.boardScript.spawnWave.level) + 1)) + 1.8f;
      return (int)(enemyStats.difficulty * 2f * math);
    }

    // Here is all the pathfinding stuff.
    // RefreshPath() is called when there is a nav graph update.
    public void Pathfind() {
      if (path == null) {
        return;
      }

      if (currentWaypoint > path.vectorPath.Count) {
        Debug.Log("End Of Path Reached");
        return;
      }
      Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
      transform.Translate(dir * currentSpeed * Time.deltaTime);
      if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
        currentWaypoint++;
        return;
      }
    }

    public void RefreshPath() {
      seeker = GetComponent<Seeker>();
      seeker.StartPath(transform.position, target.position, OnPathComplete);
      currentWaypoint = 0;
    }

    public void OnPathComplete(Path path) {
      if (!path.error) {
        this.path = path;
        currentWaypoint = 0;
      }
    }
  }
}

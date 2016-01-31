using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject {

  public int playerDamage;

  private Animator animator;
  private Transform target;
  private bool skipMove;
  public int baseMaxHitPoints;
  public int maxHitPoints;
  public int hitPoints;
  public float speed;
  public float currentSpeed;
  public float difficulty;
  public float baseDamageMultiplier;
  public float damageMultiplier;
	public int cashVal;

  private List<Debuff> debuffs = new List<Debuff>();

  enum TowerTypes: int {ORANGE_TOWER, GREEN_TOWER, WHITE_TOWER, BLUE_TOWER};

  protected override void Start() {
    GameManager.instance.AddEnemyToList(this);
    animator = GetComponent<Animator>();    
    maxHitPoints = (int)(baseMaxHitPoints * GameManager.instance.boardScript.spawnWave.HealthMultiplier());
    hitPoints = maxHitPoints;
    target = GameManager.instance.exitPoint.transform;
    base.Start();
  }

  public int Bounty() {
    return ((int)difficulty * 3);
  }

  public void TickDebuff() {
    currentSpeed = speed;
    damageMultiplier = baseDamageMultiplier;

    debuffs.RemoveAll(debuff => debuff.IsExpired());
    debuffs.ForEach(debuff => debuff.ApplyToEnemy(this));
  }

  protected override void AttemptMove<T> (float xDir, float yDir) {
    base.AttemptMove<T>(xDir, yDir);
  }

  public void MoveEnemy() {
    float xDist = (Mathf.Abs(target.position.x - transform.position.x));
    float yDist = (Mathf.Abs(target.position.y - transform.position.y));


    if (xDist < float.Epsilon && yDist < float.Epsilon) {
      Destroy(this.gameObject);
    }

    Vector3 movementVector = Vector3.MoveTowards(transform.position, target.position, currentSpeed) - transform.position;

    AttemptMove<Player>(movementVector.x, movementVector.y);
  }

  public void ApplyDebuff(System.Type clazz, float duration) {
    bool foundOne = false;
    foreach (Debuff debuff in debuffs) {
      if (debuff.GetType() == clazz) {
        debuff.expiration = Time.time + duration;
        foundOne = true;
      }
    }

    if(!foundOne) {
      Debuff debuff = (Debuff) Activator.CreateInstance(clazz, duration); 
      debuffs.Add(debuff);
    }
  }

  public void OnTriggerEnter2D(Collider2D other) {
    if (other.tag == "Exit") {
      Destroy(this.gameObject);
      GameManager.instance.playerHitPoints -= playerDamage;
    }
		else if (other.tag == "Projectile") {
      int index = 0;
      Projectile proj = other.gameObject.GetComponent<Projectile>();
      List<int> effects = proj.connectedTowers;

      foreach(int effect in effects) {
        float damageComboScaler = index > 0 ? 0.5f : 1f;
        int healthLoss;
        switch ((TowerTypes)effect) {
        case TowerTypes.ORANGE_TOWER:
          ApplyDebuff(typeof(Burn), 5);
          ApplyDamage((int)(GameManager.instance.orangeDamage * damageComboScaler));
          break;
        case TowerTypes.BLUE_TOWER:
          ApplyDebuff(typeof(Slow), 2);
          ApplyDamage((int)(GameManager.instance.blueDamage * damageComboScaler));
          break;
        case TowerTypes.GREEN_TOWER:
          ApplyDamage((int)(GameManager.instance.greenDamage * damageComboScaler));
          break;
        case TowerTypes.WHITE_TOWER:
          ApplyDebuff(typeof(Rend), 4);
          ApplyDamage((int)(GameManager.instance.whiteDamage * damageComboScaler));
          break;
        }
        index += 1;
      }

			if (hitPoints <= 0) {
        GameManager.instance.playerCash += Bounty();
				Destroy(this.gameObject);
			}
		}
  }

  public void ApplyDamage(int damage) {
    hitPoints -= ((int)(damage * damageMultiplier));
  }

  protected override void OnCantMove <T> (T component) {
    Destroy(this);
  }
}

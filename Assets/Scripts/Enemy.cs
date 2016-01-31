using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject {

  public int playerDamage;

  private Animator animator;
  private Transform target;
  private bool skipMove;
  public int hitPoints;
  public float speed;
  public float currentSpeed;
  public float difficulty;
	public int cashVal;

  private List<Debuff> debuffs = new List<Debuff>();

  enum TowerTypes: int {ORANGE_TOWER, GREEN_TOWER, WHITE_TOWER, BLUE_TOWER};

  protected override void Start() {
    GameManager.instance.AddEnemyToList(this);
    animator = GetComponent<Animator>();    
    target = GameManager.instance.exitPoint.transform;
    base.Start();
  }

  public void TickDebuff() {
    currentSpeed = speed;
    debuffs.RemoveAll(debuff => debuff.IsExpired());
    debuffs.ForEach(debuff => debuff.Update(this));
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

  public void applyDebuff(System.Type clazz, float duration) {
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
      Projectile proj = other.gameObject.GetComponent<Projectile>();
      List<int> effects = proj.connectedTowers;

      foreach(int effect in effects) {
        switch ((TowerTypes)effect) {
        case TowerTypes.ORANGE_TOWER:
          hitPoints -= GameManager.instance.orangeDamage;
          applyDebuff(typeof(Burn), 5);
          break;
        case TowerTypes.BLUE_TOWER:
          hitPoints -= GameManager.instance.blueDamage;
          applyDebuff(typeof(Slow), 2);
          break;
        case TowerTypes.GREEN_TOWER:
          hitPoints -= GameManager.instance.greenDamage;
          break;
        case TowerTypes.WHITE_TOWER:
          hitPoints -= GameManager.instance.whiteDamage;
          break;
        }
      }

			if (hitPoints <= 0) {
				GameManager.instance.playerCash += cashVal;
				Destroy(this.gameObject);
			}
		}
  }

  protected override void OnCantMove <T> (T component) {
    Destroy(this);
  }
}

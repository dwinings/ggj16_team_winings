using UnityEngine;
using System.Collections;

public class Enemy : MovingObject {

  public int playerDamage;

  private Animator animator;
  private Transform target;
  private bool skipMove;
  public int hitPoints;
  public float speed;
  public float difficulty;
	public int cashVal;

  protected override void Start() {
    GameManager.instance.AddEnemyToList(this);
    animator = GetComponent<Animator>();    
    target = GameManager.instance.exitPoint.transform;
    base.Start();
  }

  protected override void AttemptMove<T> (float xDir, float yDir) {
    base.AttemptMove<T>(xDir, yDir);
  }

  public void MoveEnemy() {
    int xDir = 0;
    int yDir = 0;
    float xDist = (Mathf.Abs(target.position.x - transform.position.x));
    float yDist = (Mathf.Abs(target.position.y - transform.position.y));


    if (xDist < float.Epsilon && yDist < float.Epsilon) {
      Destroy(this.gameObject);
    }

    Vector3 movementVector = Vector3.MoveTowards(transform.position, target.position, speed) - transform.position;

    AttemptMove<Player>(movementVector.x, movementVector.y);
  }

  public void OnTriggerEnter2D(Collider2D other) {
    if (other.tag == "Exit") {
      Destroy(this.gameObject);
      GameManager.instance.playerHitPoints -= playerDamage;
    }
		else if (other.tag == "Projectile") {
			hitPoints -= 7;
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

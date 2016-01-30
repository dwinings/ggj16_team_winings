using UnityEngine;
using System.Collections;

public class Enemy : MovingObject {

  public int playerDamage;

  private Animator animator;
  private Transform target;
  private bool skipMove;

  protected override void Start() {
    GameManager.instance.AddEnemyToList(this);
    animator = GetComponent<Animator>();    
    target = GameManager.instance.exitPoint.transform;
    base.Start();
  }

  protected override void AttemptMove<T> (int xDir, int yDir) {
    base.AttemptMove<T>(xDir, yDir);
  }

  public void MoveEnemy() {
    int xDir = 0;
    int yDir = 0;
    if(Mathf.Abs(target.position.x - transform.position.x) < 0.5f) {
      if(Mathf.Abs(target.position.y - transform.position.y) < 0.5f) {
        Destroy(this.gameObject);
      } else {
        yDir = target.position.y > transform.position.y ? 1 : -1;
      }
    } else {
      xDir = target.position.x > transform.position.x ? 1 : -1;
    }

    AttemptMove<Player>(xDir, yDir);
  }

  protected override void OnCantMove <T> (T component) {
    Destroy(this);
  }
}

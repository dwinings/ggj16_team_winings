using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

  public float moveTime = 0.1f;
  public LayerMask blockingLayer;


  private BoxCollider2D boxCollider;
  private Rigidbody2D rb2D;
  private float inverseMoveTime;

  // This lets me override things
	protected virtual void Start () {
    boxCollider = GetComponent<BoxCollider2D>();
    rb2D = GetComponent<Rigidbody2D>();
    inverseMoveTime = 1f / moveTime;
	}

  protected void Move(float xDir, float yDir) {
    Vector2 start = transform.position;
    Vector2 end = start + new Vector2(xDir, yDir);
    StartCoroutine(SmoothMovement(end));
  }

  protected IEnumerator SmoothMovement (Vector3 end) {
    float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

    while (sqrRemainingDistance > float.Epsilon) {
      Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
      rb2D.MovePosition(newPosition);
      sqrRemainingDistance = (transform.position - end).sqrMagnitude;
      yield return null;
    }
  }

  protected virtual void AttemptMove<T> (float xDir, float yDir) where T: Component {
    Move(xDir, yDir);
  }

  protected abstract void OnCantMove<T>(T Component);

	
	// Update is called once per frame
	void TickDebuff () {
	
	}
}

using UnityEngine;
using System.Collections;

public class DamageNumber : MonoBehaviour {
  private const float speed = 2f;
  private const float duration = 0.6f;
  private Vector3 direction;
  private float expiration;

	void Start () {
    expiration = duration + Time.time;
    direction = Vector3.up;
    direction.x += Random.Range(-0.4f, 0.4f);
	}
	
	void Update () {
    if (expiration <= Time.time) {
      Destroy(gameObject);
    } else {
      transform.position = transform.position + (direction * speed * Time.deltaTime);
    }
	}
}

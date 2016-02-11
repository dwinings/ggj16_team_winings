using UnityEngine;
using System.Collections;

namespace Wisp.ElementalDefense {
  public class DamageNumber : MonoBehaviour {
    private const float speed = 3f;
    private const float gravity = 6f;
    private const float duration = 0.4f;
    private Vector3 direction;
    private float expiration;

    void Start() {
      expiration = duration + Time.time;
      direction = Vector3.up;
      direction.x += Random.Range(-0.4f, 0.4f);
      transform.position += new Vector3(0f, 0.5f);
    }

    void Update() {
      if (expiration <= Time.time) {
        Destroy(gameObject);
      } else {
        transform.position = transform.position + (direction * speed * Time.deltaTime);
        direction += Vector3.down * Time.deltaTime * gravity;
      }
    }
  }
}
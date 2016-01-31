using UnityEngine;
using System;

public abstract class Debuff {
  public float expiration;

  public abstract Color MyColor();

  public Debuff(float duration) {
    expiration = Time.time + duration;
  }

  public bool IsExpired() {
    return Time.time > expiration;
  }

  public abstract void ApplyToEnemy(Enemy target);
}

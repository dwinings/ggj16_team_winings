using UnityEngine;
using System;

public abstract class Debuff {
  public enum StackingType { DURATION, INTENSITY };
  public float expiration;
  public float intensity;

  public abstract StackingType stackingType { get; }
  public abstract Color debuffColor { get; }

  public Debuff(float duration, float intensity) {
    expiration = Time.time + duration;
    this.intensity = intensity;
  }

  public bool IsExpired() {
    return Time.time > expiration;
  }

  public abstract void ApplyToEnemy(Enemy target);
}

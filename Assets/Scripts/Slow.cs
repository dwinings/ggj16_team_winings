using System;
using UnityEngine;

public class Slow : Debuff {
  public float slowPercent = 0.5f;
  public Slow(float duration) : base(duration) {}

  public override void ApplyToEnemy(Enemy other) {
    Debug.Log("slowing");
    other.currentSpeed = other.currentSpeed - (other.currentSpeed * slowPercent);
  }
}


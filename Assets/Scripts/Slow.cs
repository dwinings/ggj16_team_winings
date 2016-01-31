using System;
using UnityEngine;

public class Slow : Debuff {
  public float slowPercent = 0.3f;
  public Slow(float duration) : base(duration) {}

  public override void Update(Enemy other) {
    other.currentSpeed = other.currentSpeed - (other.currentSpeed * slowPercent);
  }
}


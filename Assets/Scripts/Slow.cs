using System;
using UnityEngine;

public class Slow : Debuff {
  public float slowPercent = 0.5f;
  public Slow(float duration) : base(duration) {}

  public override Color MyColor() {
    return new Color(13f / 255f, 219f / 255f, 219f / 255f);
  }

  public override void ApplyToEnemy(Enemy other) {
    other.currentSpeed = other.currentSpeed - (other.currentSpeed * slowPercent);
  }
}


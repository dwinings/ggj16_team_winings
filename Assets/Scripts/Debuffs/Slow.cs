using System;
using UnityEngine;

public class Slow : Debuff {
  public Slow(float duration, float intensity) : base(duration, intensity) {}

  public override StackingType stackingType { get { return StackingType.DURATION; } }
  public override Color debuffColor { get { return new Color(13f / 255f, 219f / 255f, 219f / 255f); } }

  public override void ApplyToEnemy(Enemy other) {
    other.currentSpeed = other.currentSpeed - (other.currentSpeed * intensity);
  }
}


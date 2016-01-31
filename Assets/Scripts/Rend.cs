using System;
using UnityEngine;

public class Rend : Debuff {
  public Rend(float duration) : base(duration) {}

  public override Color MyColor() {
    return new Color(100f / 255f, 100f / 255f, 100f / 255f);
  }

  public override void ApplyToEnemy(Enemy target) {
    target.damageMultiplier *= 1.1f;
  }
}




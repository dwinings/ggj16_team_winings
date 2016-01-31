using System;
using UnityEngine;

public class Rend : Debuff {
  public Rend(float duration) : base(duration) {}

  public override void ApplyToEnemy(Enemy target) {
    target.damageMultiplier *= 1.2f;
  }
}




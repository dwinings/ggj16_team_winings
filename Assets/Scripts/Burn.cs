using System;
using UnityEngine;

public class Burn : Debuff {
  public int burnDamage = 3;
  public Burn(float duration) : base(duration) {}

  public override void ApplyToEnemy(Enemy target) {
    target.hitPoints -= burnDamage;
  }
}


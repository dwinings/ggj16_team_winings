using System;
using UnityEngine;

public class Burn : Debuff {
  public int burnDamage = 3;

  public override Color MyColor(){
    return new Color(227f / 255f, 144f / 255f, 55f / 255f);
  }

  public Burn(float duration) : base(duration) {}

  public override void ApplyToEnemy(Enemy target) {
    target.hitPoints -= burnDamage;
  }
}


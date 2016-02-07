using System;
using UnityEngine;

public class Burn : Debuff {
  public Burn(float duration, float intensity) : base(duration, intensity) {}

  new public static Enemy.DamageType damageType = Enemy.DamageType.TRUE;

  public override StackingType stackingType { get { return StackingType.INTENSITY; } }
  public override Color debuffColor { get { return new Color(227f / 255f, 144f / 255f, 55f / 255f); } }

  public override void ApplyToEnemy(Enemy target) {
    target.ApplyDebuffDamage(GetType(), (int)intensity);
  }
}


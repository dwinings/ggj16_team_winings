using System;
using UnityEngine;

namespace Wisp.ElementalDefense {
  public class Rend : Debuff {
    public Rend(float duration, float intensity) : base(duration, intensity) { }

    public override StackingType stackingType { get { return StackingType.INTENSITY; } }
    public override Color debuffColor { get { return new Color(100f / 255f, 100f / 255f, 100f / 255f); } }

    public override void ApplyToEnemy(Enemy target) {
      target.effectDamageMultiplier += intensity;
    }
  }
}




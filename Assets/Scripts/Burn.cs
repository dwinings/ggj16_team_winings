using System;
using UnityEngine;

public class Burn : Debuff {
  public int burnDamage = 3;
  public Burn(float duration) : base(duration) {}

  public override void Update(Enemy target) {
    Debug.Log(String.Format("Burning enemy with {0} hitpoints by {1}", target.hitPoints, burnDamage));
    target.hitPoints -= burnDamage;
  }
}


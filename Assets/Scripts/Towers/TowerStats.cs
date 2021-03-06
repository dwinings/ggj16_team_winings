﻿using System;
using UnityEngine;

namespace Wisp.ElementalDefense {
  public class TowerStats : MonoBehaviour {
    public Sprite sprite;
    public Color color;
    public Sprite projectile;
    public Projectile.Type projectileType;
    public float projectileSizeModifier;
    public float projectileDeviationModifier;
    public GameObject blankObject;
    public Enemy.DamageType damageType;
    public int damage;
    public int cost;
    public float range;
    public float firingSpeed;
    public Debuff debuff;
    public float duration;
    public float intensity;

    public void Start() {
      debuff = GetComponent<Debuff>();
    }

    public Debuff CreateNewDebuff() {
      if (debuff != null) {
        return Debuff.Create(debuff, duration, intensity);
      } else {
        return null;
      }
    }
  }
}

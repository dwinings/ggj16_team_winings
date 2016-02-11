using UnityEngine;
using System;

namespace Wisp.ElementalDefense {
  public abstract class Debuff : MonoBehaviour {
    public enum StackingType { DURATION, INTENSITY };
    public static Enemy.DamageType damageType = Enemy.DamageType.NORMAL;
    public float expiration;
    public float intensity;

    public abstract StackingType stackingType { get; }
    public abstract Color debuffColor { get; }

    public Debuff(float duration, float intensity) {
      expiration = Time.time + duration;
      this.intensity = intensity;
    }

    public static Debuff Create(Debuff baseInstance, float duration, float intensity) {
      Debuff newInstance = Instantiate(baseInstance);
      newInstance.gameObject.transform.SetParent(GameManager.instance.debuffHolder);
      newInstance.expiration = Time.time + duration;
      newInstance.intensity = intensity;
      return newInstance;
    }

    public bool IsExpired() {
      return Time.time > expiration;
    }

    public abstract void ApplyToEnemy(Enemy target);
  }
}

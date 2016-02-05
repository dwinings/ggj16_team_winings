using UnityEngine;
using System.Collections;
using UnityEditor.Animations;

// Here lie the stats that differentiate different types of enemies.
// Use this class to make prefabs.
public class EnemyStats : MonoBehaviour {
  public int baseMaxHitPoints;
  public float speed;
  public float difficulty;
  public float baseDamageMultiplier;
  public int playerDamage;

  public RuntimeAnimatorController animatiorController;

	void Start () {}
  void Update () {}
}

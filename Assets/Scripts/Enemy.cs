﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject {

  public enum DamageType { NORMAL, TRUE }

  public int playerDamage;
  public Transform gotToCrystalParticleEffect;
  public Transform iAmSlainParticleEffect;

  private Transform target;
  public GameObject healthBar;
  public GameObject damageNumberPrefab;
  public int baseMaxHitPoints;
  public int maxHitPoints;
  public int hitPoints;
  public float tickTime = 1f;
  private float lastTick;
  public float speed;
  public float currentSpeed;
  public float difficulty;
  public float baseDamageMultiplier;
  public float damageMultiplier;
  public float effectDamageMultiplier = 1f;

  private List<Debuff> debuffs = new List<Debuff>();

  protected override void Start() {
    GameManager.instance.AddEnemyToList(this);
    healthBar = Instantiate(healthBar, transform.position, Quaternion.identity) as GameObject;
    healthBar.transform.parent = transform;
    healthBar.transform.position = healthBar.transform.position + (new Vector3(0, 1)) * 0.7f;
    maxHitPoints = (int)(baseMaxHitPoints * GameManager.instance.boardScript.spawnWave.HealthMultiplier());
    hitPoints = maxHitPoints;
    target = GameManager.instance.exitPoint.transform;
    lastTick = Time.time - 1f;
    base.Start();
  }

  private void UpdateHealthBar() {
    float healthPercent = ((float) hitPoints) / maxHitPoints;
    Image image = healthBar.GetComponentInChildren<Image>();
    image.rectTransform.sizeDelta = new Vector2((1.2f * healthPercent), 0.2f);
    image.color = Color.Lerp(Color.red, Color.green, healthPercent);
  }

  public void Update() {
    if (lastTick + tickTime < Time.time) {
      lastTick = Time.time;
      TickDebuffs();
    }
    transform.position =  Vector2.MoveTowards(transform.position, GameManager.instance.exitPoint.transform.position, currentSpeed * Time.deltaTime);
    UpdateHealthBar();
  }

  public int Bounty() {
	float math = (-0.333f * Mathf.Log ((3f * GameManager.instance.boardScript.spawnWave.level) + 1)) + 1.8f;
		return (int)(difficulty * 2f * math);
  }

  public void TickDebuffs() {
    effectDamageMultiplier = 1f;
    currentSpeed = speed;

    debuffs.RemoveAll(debuff => debuff.IsExpired());
    debuffs.ForEach(debuff => debuff.ApplyToEnemy(this));
    List<Color> colors  = new List<Color>();
    colors.Add(Color.white);
    debuffs.ForEach(debuff => colors.Add(debuff.debuffColor));
    Color result = CombineColors(colors);
    this.GetComponent<SpriteRenderer>().color = result;
  }

  public static Color CombineColors(List<Color> aColors) {
    Color result = new Color(0f, 0f, 0f, 0f);
    foreach (Color c in aColors) {
      result += c;
    }
    return result / aColors.Count;
  }

  protected override void AttemptMove<T> (float xDir, float yDir) {
    base.AttemptMove<T>(xDir, yDir);
  }

  public void MoveEnemy() {
    float xDist = (Mathf.Abs(target.position.x - transform.position.x));
    float yDist = (Mathf.Abs(target.position.y - transform.position.y));

    if (xDist < float.Epsilon && yDist < float.Epsilon) {
      Destroy(this.gameObject);
    }

    Vector3 movementVector = Vector3.MoveTowards(transform.position, target.position, currentSpeed) - transform.position;
    Move(movementVector.x, movementVector.y);
  }

  public void ApplyDebuff(System.Type clazz, float duration, float intensity) {
    Debuff debuff = (Debuff)Activator.CreateInstance(clazz, duration, intensity);

    switch(debuff.stackingType) {
      case Debuff.StackingType.DURATION:
        Debuff oldDebuff = debuffs.Find(db => db.GetType() == debuff.GetType());
        if (oldDebuff != null) {
          oldDebuff.expiration += duration;
        } else {
          debuffs.Add(debuff);
        }
        break;
      case Debuff.StackingType.INTENSITY:
        debuffs.Add(debuff);
        break;
    }
  }

  public void OnTriggerEnter2D(Collider2D other) {
    if (other.tag == "Exit") {
      Instantiate(gotToCrystalParticleEffect, target.position, Quaternion.identity);
      Destroy(this.gameObject);
      GameManager.instance.playerHitPoints -= playerDamage;
      SFXManager.instance.PlaySoundAt ("crystal_hit", this.transform.position);
    }
		else if (other.tag == "Projectile") {
      int index = 0;
      Projectile proj = other.gameObject.GetComponent<Projectile>();
      List<TowerManager.TowerTypes> effects = proj.connectedTowers;
      int damage = 0;

      foreach(TowerManager.TowerTypes effect in effects) {
        float damageComboScaler = index > 0 ? 0.5f : 1f;
        switch (effect) {
        case TowerManager.TowerTypes.ORANGE_TOWER:
          ApplyDebuff(typeof(Burn), 5, 3f);
          damage += ((int)(GameManager.instance.orangeDamage * damageComboScaler));
          break;
        case TowerManager.TowerTypes.BLUE_TOWER:
          ApplyDebuff(typeof(Slow), 2, 0.3f);
          damage += ((int)(GameManager.instance.blueDamage * damageComboScaler));
          break;
        case TowerManager.TowerTypes.GREEN_TOWER:
          damage += ((int)(GameManager.instance.greenDamage * damageComboScaler));
          break;
        case TowerManager.TowerTypes.WHITE_TOWER:
          ApplyDebuff(typeof(Rend), 2, 0.1f);
          damage += ((int)(GameManager.instance.whiteDamage * damageComboScaler));
          break;
        }
        index += 1;
      }

      ApplyDamage(damage, DamageType.NORMAL);

      if (hitPoints <= 0) {
        Instantiate(iAmSlainParticleEffect, transform.position, Quaternion.identity);
        SFXManager.instance.PlaySoundAt ("enemy_die_2", this.transform.position);
        GameManager.instance.playerCash += Bounty ();
        Destroy (this.gameObject);
      } else {
        SFXManager.instance.PlaySoundAt ("proj_hit", this.transform.position);
      }
		}
  }

  public void ApplyDamage(int damage, DamageType damageType) {
    int realDamage;
    float realMultiplier = 0f;

    switch (damageType) {
      case DamageType.NORMAL:
        realMultiplier = baseDamageMultiplier * effectDamageMultiplier;
        break;
      case DamageType.TRUE:
        realMultiplier = 1;
        break;
    }
    realDamage = (int)(damage * realMultiplier);

    // Debug.Log("Applying " + damage + " damage to target with a " + realMultiplier + " multiplier (" + realDamage + ")");
    hitPoints -= realDamage;

    GameObject dn = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity) as GameObject;
    dn.transform.parent = transform;
    Text dnText = dn.GetComponentInChildren<Text>();
    dnText.color = damageType == DamageType.TRUE ? Color.white : new Color(1f, 86f / 255f, 86f / 255f);
    dnText.text = "" + realDamage;
  }

  protected override void OnCantMove <T> (T component) {
    Destroy(this);
  }
}

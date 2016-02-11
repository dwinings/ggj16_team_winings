using UnityEngine;
using System.Collections;

namespace Wisp.ElementalDefense {
  public class Wave {
    public const float DIFFICULTY_COEFFICIENT = 10;
    public const float MIN_SPAWN_TIME = 0.5f;
    public const float MAX_SPAWN_TIME = 3f;
    public int level = 1;
    public float maxDifficulty;
    public float difficultyAlreadySpawned = 0f;


    public void BeginNextLevel() {
      level += 1;
      if (level > 2)
        SFXManager.instance.PlaySound("wave_clear");
      difficultyAlreadySpawned = 0f;
    }

    // monsters per wave scale at nlogn rate.
    public float MaxDifficulty() {
      return DIFFICULTY_COEFFICIENT * level * Mathf.Log(level);
    }

    public float HealthMultiplier() {
      return 1f + ((level - 1f) * (level / 10f));
    }

    public float generateNextSpawn(float lastDifficulty) {
      difficultyAlreadySpawned += lastDifficulty;
      int finishedPercent = (int)(difficultyAlreadySpawned * 100f / MaxDifficulty());
      finishedPercent = Mathf.Min(100, finishedPercent);
      GameManager.instance.SetText(GameManager.instance.waveText, string.Format("Wave {0}: {1}%", level - 1, finishedPercent));
      float decayFactor = 100f * ((difficultyAlreadySpawned / MaxDifficulty()) / MaxDifficulty());
      float normalizedDifficulty = (lastDifficulty / MaxDifficulty());
      float result = normalizedDifficulty * 10f * decayFactor;
      result = Mathf.Max(MIN_SPAWN_TIME, result);
      result = Mathf.Min(MAX_SPAWN_TIME, result);

      // Debug.Log("Spawning in " + result + " seconds after adding " + lastDifficulty + " difficulty");
      // Debug.Log("Decay Factor: " + decayFactor + " ND: " + normalizedDifficulty);

      return result;
    }

    public bool IsWaveOver() {
      return MaxDifficulty() <= difficultyAlreadySpawned;
    }
  }
}

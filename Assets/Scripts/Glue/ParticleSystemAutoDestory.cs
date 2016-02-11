using UnityEngine;
using System.Collections;

namespace Wisp.ElementalDefense {
  public class ParticleSystemAutoDestory : MonoBehaviour {

    // Use this for initialization
    void Start() {
      Destroy(gameObject, GetComponent<ParticleSystem>().duration);
    }
  }
}

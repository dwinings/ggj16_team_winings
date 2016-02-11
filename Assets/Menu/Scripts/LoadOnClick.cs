using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Wisp.ElementalDefense {
  public class LoadOnClick : MonoBehaviour {
    public void LoadScene(int level) {
      SceneManager.LoadScene(level);
    }
  }
}

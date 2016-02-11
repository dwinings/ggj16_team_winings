using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Wisp.ElementalDefense {
  public class QuitOnClick : MonoBehaviour {

    void Update() {
      if (Input.GetMouseButtonDown(0))
        SceneManager.LoadScene(0);
    }
  }
}

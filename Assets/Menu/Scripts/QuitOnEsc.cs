using UnityEngine;
using System.Collections;

namespace Wisp.ElementalDefense {
  public class QuitOnEsc : MonoBehaviour {

    void Update() {
      if (Input.GetKey("escape"))
        Application.Quit();
    }
  }
}

using UnityEngine;
using System.Collections;

public class QuitOnEsc : MonoBehaviour {

  void Update () {
    if (Input.GetKey("escape"))
      Application.Quit();
  }
}

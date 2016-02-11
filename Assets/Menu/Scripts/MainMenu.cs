using UnityEngine;
using System.Collections;

namespace Wisp.ElementalDefense {
  public class MainMenu : MonoBehaviour {
    public Texture backgroundTexture;

    void onGUI() {
      // Display dat background swagswagswag

      GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);
    }
  }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Wisp.ElementalDefense {
  public class HealthBar : MonoBehaviour {
    public float baseHeight;
    public float baseWidth;
    public float heightOffset;

    void Start() {
      transform.position = transform.position + (new Vector3(0f, heightOffset));
      Image image = GetComponentInChildren<Image>();
      image.transform.SetParent(transform);
      gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(baseWidth, baseHeight);
    }

    public void UpdatePercent(float percent) {
      Image image = GetComponentInChildren<Image>();
      float newCenter = 0.5f * baseWidth * (percent - 1f);
      image.rectTransform.sizeDelta = new Vector2((baseWidth * percent), baseHeight);
      image.transform.localPosition = new Vector2(newCenter, 0f);
      image.color = Color.Lerp(Color.red, Color.green, percent);
    }
  }
}

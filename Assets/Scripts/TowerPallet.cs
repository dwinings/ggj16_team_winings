using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerPallet : MonoBehaviour
{
  public GameObject tower;
  public int towerCost;
  public Text towerCostText;

  private Camera camera;
  private static GameObject draggable;
  private static Ray ray;
  private static RaycastHit hit;
  private Vector2 relativePosition;
  private Vector3 offset = new Vector3 (0f, 0f, 0f);
  private int currentCash;
  private static int initted = 0;

  public void Start() {
    if (initted < 4) {
      camera = Camera.main;
      towerCostText.transform.position = camera.WorldToScreenPoint (transform.position) + offset;
      towerCostText.text = "" + towerCost;
      initted += 1;
    }
  }

  public void OnMouseDown() {
    currentCash = GameManager.instance.playerCash;
    if (currentCash >= towerCost) {
      GameManager.instance.UpdateText ();
      draggable = getDraggable ();
    } else {
      Destroy (draggable);
    }
  }

  public void OnMouseDrag() {
    if (draggable) {
      Transform dt = draggable.transform;
      relativePosition = camera.ScreenToWorldPoint (Input.mousePosition);
      dt.position = relativePosition;
    }
  }

 public void OnMouseUp() {
    if (draggable) {
      Destroy (draggable);
      ray = camera.ScreenPointToRay (Input.mousePosition);
      if (Physics.Raycast (ray, out hit) && (currentCash >= towerCost)) {
        Instantiate (tower, hit.transform.position, Quaternion.identity);
        Destroy (hit.transform.gameObject.GetComponent<BoxCollider> ());
        GameManager.instance.playerCash -= towerCost;
      }
      currentCash = 0;
    }
  }

  private GameObject getDraggable() {
    if (draggable != null)
      Destroy (draggable);
    return Instantiate (gameObject);
  }
}
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pathfinding;

public class TowerPallet : MonoBehaviour
{
  public GameObject tower;
  public int towerCost;
  public Text towerCostText;

  private Camera palletCamera;
  private static GameObject draggable;
  private static Ray ray;
  private Vector2 relativePosition;
  private Vector3 offset = new Vector3(0f, 0f, 0f);
  private int currentCash;
  private static int initted = 0;

  public void Start() {
    if (initted < 4) {
      palletCamera = Camera.main;
      towerCostText.transform.position = palletCamera.WorldToScreenPoint(transform.position) + offset;
      towerCostText.text = "" + towerCost;
      initted += 1;
    }
    initted = 0;
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
      relativePosition = palletCamera.ScreenToWorldPoint (Input.mousePosition);
      dt.position = relativePosition;
    }
  }

 public void OnMouseUp() {
    if (draggable) {
      Destroy (draggable);
      Vector3 mouseWorldPosition = palletCamera.ScreenToWorldPoint(Input.mousePosition);
      mouseWorldPosition.z = 0f; // Just in case
      ray = palletCamera.ScreenPointToRay (Input.mousePosition);
      RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.up, 0.0001f);
      if (hit.collider && (currentCash >= towerCost)) {
        if (hit.transform.gameObject.CompareTag("Pedestal")) {
          GameObject instance = Instantiate(tower, hit.transform.position, Quaternion.identity) as GameObject;
          Destroy(hit.transform.gameObject.GetComponent<BoxCollider2D>());
          GameManager.instance.playerCash -= towerCost;
        }
      }
      currentCash = 0;
    }
  }

  private GameObject getDraggable() {
    if (draggable != null) {
      Destroy(draggable);
    }
    return Instantiate (gameObject);
  }
}
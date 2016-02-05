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
  public GameObject draggableBlank;
  private static GameObject draggable;
  private Vector3 offset = new Vector3(0f, 0f, 0f);
  private int currentCash;
  private float palletTextOffset = -30;

  public void Start() {
    palletCamera = Camera.main;
    towerCostText = GetComponentInChildren<Text>();
    towerCostText.text = "" + towerCost;
    transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    towerCostText.transform.SetParent(transform);
    towerCostText.transform.localPosition = new Vector3(palletTextOffset, 0f, 0f);
  }

  public void OnMouseDown() {
    currentCash = GameManager.instance.playerCash;
    if (currentCash >= towerCost) {
      GameManager.instance.UpdateText();
      draggable = getDraggable();
    } else {
      Destroy(draggable);
    }
  }

  public void OnMouseDrag() {
    if (draggable) {
      Vector3 newPos = palletCamera.ScreenToWorldPoint(Input.mousePosition);
      newPos.z = 0;
      draggable.transform.position = newPos;
    }
  }

  public void OnMouseUp() {
    if (draggable) {
      Destroy(draggable);
      Vector3 mouseWorldPosition = palletCamera.ScreenToWorldPoint(Input.mousePosition);
      mouseWorldPosition.z = 0f; // Just in case
      RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.up, 0.0001f);
      if (hit.collider && (currentCash >= towerCost)) {
        if (hit.transform.gameObject.CompareTag("Pedestal")) {
          Instantiate(tower, hit.transform.position, Quaternion.identity);
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
    return Instantiate(draggableBlank, palletCamera.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity) as GameObject;
  }
}
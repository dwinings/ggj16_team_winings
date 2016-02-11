using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pathfinding;

namespace Wisp.ElementalDefense {
  public class TowerPallet : MonoBehaviour {
    public GameObject baseTower;
    public Text towerCostText;

    public GameObject draggableBlank;
    public TowerStats stats;
    protected Camera palletCamera;
    protected static GameObject draggable;
    protected Vector3 offset = new Vector3(0f, 0f, 0f);
    protected int currentCash;
    protected float palletTextOffset = -30;

    public void Start() {
      palletCamera = Camera.main;
      towerCostText = GetComponentInChildren<Text>();
      transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
      towerCostText.transform.SetParent(transform);
      towerCostText.transform.localPosition = new Vector3(palletTextOffset, 0f, 0f);
    }

    public void OnMouseDown() {
      currentCash = GameManager.instance.playerCash;
      if (currentCash >= stats.cost) {
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

    public virtual void OnMouseUp() {
      if (draggable) {
        Destroy(draggable);
        Vector3 mouseWorldPosition = palletCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f; // Just in case
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.up, 0.0001f);
        if (hit.collider && (currentCash >= stats.cost)) {
          if (hit.transform.gameObject.CompareTag("Pedestal")) {
            GameObject instance = Instantiate(baseTower, hit.transform.position, Quaternion.identity) as GameObject;
            instance.transform.SetParent(GameManager.instance.towerHolder);
            instance.GetComponent<Tower>().ApplyTowerStats(stats);
            Destroy(hit.transform.gameObject.GetComponent<BoxCollider2D>());
            GameManager.instance.playerCash -= stats.cost;
          }
        }
        currentCash = 0;
      }
    }

    public void ApplyStats(TowerStats stats) {
      this.stats = stats;
      GetComponent<Image>().sprite = stats.sprite;
      towerCostText.text = "" + stats.cost;
    }

    protected GameObject getDraggable() {
      if (draggable != null) {
        Destroy(draggable);
      }
      return Instantiate(stats.blankObject, palletCamera.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity) as GameObject;
    }
  }
}
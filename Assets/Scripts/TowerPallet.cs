using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TowerPallet : MonoBehaviour
{
  public GameObject tower;

  private Vector3 startPosition;
  private Vector2 relativePosition;

  private Camera camera;
  private static GameObject draggable;
  private static Ray ray;
  private static RaycastHit hit;

  public void Start() {
    camera = Camera.main;
  }

  public void OnMouseDown() {
    draggable = getDraggable ();
  }

  public void OnMouseDrag() {
    Transform dt = draggable.transform;
    relativePosition = camera.ScreenToWorldPoint (Input.mousePosition);
    dt.position = relativePosition;
  }

  public void OnMouseUp() {
    Destroy (draggable);
    ray = camera.ScreenPointToRay (Input.mousePosition);
    if (Physics.Raycast (ray, out hit)) {
      Instantiate (tower, hit.transform.position, Quaternion.identity);
    }
  }

  private GameObject getDraggable() {
    if (draggable != null)
      Destroy (draggable);
    return Instantiate (gameObject);
  }
}
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TowerPallet : MonoBehaviour
{
  public TOWER_TYPE tower_type;

  private Vector3 startPosition;
  private Vector2 relativePosition;

  public enum TOWER_TYPE{TYPE1, TYPE2, TYPE3, TYPE4};

  private Camera camera;
  private static GameObject draggable;

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
  }

  private GameObject getDraggable() {
    if (draggable != null)
      Destroy (draggable);
    return Instantiate (gameObject);
  }
}
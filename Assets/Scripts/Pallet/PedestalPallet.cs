using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pathfinding;

public class PedestalPallet : MonoBehaviour {
  public GameObject pedestal;
  public int pedestalCost;
  public Text pedestalCostText;
  public float adjacentDistanceConst;

  private Camera palletCamera;
  private static GameObject draggable;
  private Vector2 relativePosition;
  private Vector3 offset;
  private int currentCash;
  private static int initted = 0;
  private Ray ray;
  private RaycastHit hit;
  private Vector3 failureVector = new Vector3(-255, -255, -255);

  public void Start() {
    if (initted < 1) {
      palletCamera = Camera.main;
      offset = GetComponent<SpriteRenderer>().bounds.size;
      offset.y = 0f;
      offset.z = 0f;
      pedestalCostText.transform.position = palletCamera.WorldToScreenPoint (transform.position) + offset;
      pedestalCostText.text = "" + pedestalCost;
      initted += 1;
    }
    initted = 0;
  }

  public void OnMouseDown() {
    currentCash = GameManager.instance.playerCash;
    if (currentCash >= pedestalCost) {
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
      Vector3 v = FindPedestalPlacement();
      if (!v.Equals(failureVector)) {
        GameObject instance = Instantiate (pedestal, v, Quaternion.identity) as GameObject;
        if (NoPathBlockage(instance)) {
          GameManager.instance.playerCash -= pedestalCost;
          currentCash = 0;
        } else {
          Destroy(instance);
          AstarPath.active.UpdateGraphs(instance.GetComponent<Collider2D>().bounds);
        }
      }
    }
  }

  private GameObject getDraggable() {
    if (draggable != null)
      Destroy (draggable);
    GameObject d =  Instantiate (gameObject);
    Destroy (d.GetComponent<BoxCollider> ());
    return d;
  }


  private Vector3 FindPedestalPlacement() {
    Vector3 worldPoint = palletCamera.ScreenToWorldPoint(Input.mousePosition);
    worldPoint.z = 0f;

    // Round onto our grid.
    worldPoint.x = Mathf.Round(worldPoint.x);
    worldPoint.y = Mathf.Round(worldPoint.y);

    // Check if we're close to spawn/egress points.
    GameObject enter = GameObject.FindGameObjectWithTag("Entrance");
    GameObject exit = GameObject.FindGameObjectWithTag("Exit");
    if ( (Vector3.Distance(worldPoint, enter.transform.position) <= 2)
      || (Vector3.Distance(worldPoint, exit.transform.position) <= 2)) {
      return failureVector;
    }

    // Very small raycast so we can see if we're on top of something
    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.up, 0.0001f);

    if (hit.collider == null) {
      return worldPoint;
    } else {
      return failureVector;
    }
  }

  private bool NoPathBlockage(GameObject placedObject) {
    var updateObject = new GraphUpdateObject(placedObject.GetComponent<Collider2D>().bounds);
    Vector3 start = GameManager.instance.spawnPoint.transform.position;
    Vector3 end = GameManager.instance.exitPoint.transform.position;
    var startNode = AstarPath.active.GetNearest(start).node;
    var endNode = AstarPath.active.GetNearest(end).node;
    return GraphUpdateUtilities.UpdateGraphsNoBlock(updateObject, startNode, endNode);
  }
}
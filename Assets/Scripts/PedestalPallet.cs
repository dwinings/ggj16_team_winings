using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PedestalPallet : MonoBehaviour {
  public GameObject pedestal;
  public int pedestalCost;
  public Text pedestalCostText;
  public float adjacentDistanceConst;

  private Camera camera;
  private static GameObject draggable;
  private Vector2 relativePosition;
  private Vector3 offset;
  private int currentCash;
  private static int initted = 0;
  private Ray ray;
  private RaycastHit hit;

  public void Start() {
    if (initted < 1) {
      camera = Camera.main;
      offset = GetComponent<SpriteRenderer> ().bounds.size;
      offset.y = 0f;
      offset.z = 0f;
      pedestalCostText.transform.position = camera.WorldToScreenPoint (transform.position) + offset;
      pedestalCostText.text = "" + pedestalCost;
      initted += 1;
    }
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
      relativePosition = camera.ScreenToWorldPoint (Input.mousePosition);
      dt.position = relativePosition;
    }
  }

  public void OnMouseUp() {
    if (draggable) {
      Destroy (draggable);
      Vector3 v = FindPedestalPlacement();
      if (!v.Equals(Vector3.zero)) {
        GameObject instance = Instantiate (pedestal, v, Quaternion.identity) as GameObject;
        //instance.transform.SetParent (GameManager.instance.boardScript.boardHolder);
        GameManager.instance.playerCash -= pedestalCost;
        currentCash = 0;
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
    Vector3 curr = camera.ScreenToWorldPoint(Input.mousePosition);
    Vector3 closestObj = Vector3.zero;
    float closestDist = -1;
    GameObject[] pedestals = GameObject.FindGameObjectsWithTag ("Pedestal");
    foreach (GameObject p in pedestals) {
      float distance = (Mathf.Sqrt (Mathf.Pow ((curr.x - p.transform.position.x), 2) + Mathf.Pow ((curr.y -p.transform.position.y), 2)));
      Debug.Log (distance);
      if (closestDist == -1 || closestDist > distance) {
        closestDist = distance;
        closestObj = p.transform.position;
      }
    }
    if (closestDist <= adjacentDistanceConst) {
      float x = closestObj.x;
      float y = closestObj.y;
      float xDifference = Mathf.Abs(curr.x - x);
      float yDifference = Mathf.Abs(curr.y - y);

      if (yDifference > xDifference) {
        if (curr.y - y > 0) {
          y = closestObj.y + 1;
        } else {
          y = closestObj.y - 1;
        }
      } else {
        if (curr.x - x > 0) {
          x = closestObj.x + 1;
        } else {
          x = closestObj.x - 1;
        }
      }
      return new Vector3 (x, y, 0f);
    } else {
      GameObject enter = GameObject.FindGameObjectWithTag ("Entrance");
      GameObject exit = GameObject.FindGameObjectWithTag ("Exit");
      ray = camera.ScreenPointToRay (Input.mousePosition);
      Debug.Log (Vector3.Distance (draggable.transform.position, enter.transform.position));
      if((Vector3.Distance(draggable.transform.position, enter.transform.position) > 2) && 
        (Vector3.Distance(draggable.transform.position, exit.transform.position) > 2) &&
        (!(Physics.Raycast(ray, out hit)))) {
          float x = (float)(int)Mathf.Round (curr.x);
          float y = (float)(int)Mathf.Round (curr.y);
          return new Vector3 (x, y, 0f);
      } else {
        return Vector3.zero;
      }
    }
  } 
}
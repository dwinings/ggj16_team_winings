using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pathfinding;
using System.Collections.Generic;
using System;

public class PedestalPallet : MonoBehaviour {
  public GameObject pedestal;
  public int pedestalCost;
  public Text pedestalCostText;
  public float adjacentDistanceConst;

  private Camera palletCamera;
  public GameObject draggableBlank;
  private static GameObject draggable;
  private Vector2 relativePosition;
  private Vector3 offset;
  private int currentCash;
  private static int initted = 0;
  private float palletTextOffset = -30;
  private Vector3 failureVector = new Vector3(-255, -255, -255);

  public void Start() {
    palletCamera = Camera.main;
    pedestalCostText = GetComponentInChildren<Text>();
    pedestalCostText.text = "" + pedestalCost;
    transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    pedestalCostText.transform.SetParent(transform);
    pedestalCostText.transform.localPosition = new Vector3(palletTextOffset, 0f, 0f);
  }

  public void OnMouseDown() {
    currentCash = GameManager.instance.playerCash;
    if (currentCash >= pedestalCost) {
      GameManager.instance.UpdateText ();
      draggable = getDraggable();
    } else {
      Destroy (draggable);
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
      Destroy (draggable);
      Vector3 v = FindPedestalPlacement();
      if (!v.Equals(failureVector)) {
        GameObject instance = Instantiate (pedestal, v, Quaternion.identity) as GameObject;
        if (NoPathBlockage(instance)) {
          AllEnemiesResetPath();
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
    if (draggable != null) {
      Destroy(draggable);
    }
    return Instantiate(draggableBlank, palletCamera.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity) as GameObject;
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

  private void AllEnemiesResetPath() {
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    Array.ForEach(enemies, enemy => enemy.GetComponent<Enemy>().RefreshPath());
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
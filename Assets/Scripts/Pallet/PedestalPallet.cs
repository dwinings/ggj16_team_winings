using UnityEngine;
using Pathfinding;
using System;

public class PedestalPallet : TowerPallet {
  public float adjacentDistanceConst;
  public GameObject pedestal;
  private Vector3 failureVector = new Vector3(-255, -255, -255);

  public override void OnMouseUp() {
    if (draggable) {
      Destroy(draggable);
      Vector3 v = FindPedestalPlacement();
      if (!v.Equals(failureVector)) {
        GameObject instance = Instantiate(pedestal, v, Quaternion.identity) as GameObject;
        if (NoPathBlockage(instance)) {
          AllEnemiesResetPath();
          GameManager.instance.playerCash -= stats.cost;
          currentCash = 0;
        } else {
          Destroy(instance);
        }
      }
    }
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
    if ( (Vector3.Distance(worldPoint, enter.transform.position) <= 1.5)
      || (Vector3.Distance(worldPoint, exit.transform.position) <= 1.5)) {
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
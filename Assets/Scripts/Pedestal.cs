using UnityEngine;
using System.Collections;

public class Pedestal : MonoBehaviour {

	void Awake() {
    AstarPath.active.UpdateGraphs(GetComponent<Collider2D>().bounds);
	}
	
	void Update() {}
}

using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
  public float cameraZoomSpeed;
  public float cameraPanSpeed;
  public bool middleMouseButtonPressed;
  private Vector3 lastMousePosition = Vector3.zero;
  private Camera theCamera;

	void Start () {
    theCamera = GetComponent<Camera>();
	
	}
	
	void Update () {
    if (Input.GetButtonDown("Fire3")) {
      middleMouseButtonPressed = true;
    }

    if (Input.GetButtonUp("Fire3")) {
      middleMouseButtonPressed = false;
    }

    if (middleMouseButtonPressed) {
      Vector3 worldCoordDelta = theCamera.ScreenToWorldPoint(lastMousePosition) - theCamera.ScreenToWorldPoint(Input.mousePosition);
      theCamera.transform.position += worldCoordDelta;
    }
    lastMousePosition = Input.mousePosition;

    float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
    theCamera.orthographicSize = theCamera.orthographicSize - (mouseScroll * cameraZoomSpeed);
  }
}

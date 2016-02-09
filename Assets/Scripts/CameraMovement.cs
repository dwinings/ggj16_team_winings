using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
  public int boardWidth;
  public int boardHeight;
  public float cameraZoomSpeed;
  public float cameraPanSpeed;
  public bool middleMouseButtonPressed;
  private Vector3 lastMousePosition = Vector3.zero;
  private Camera theCamera;

	void Start () {
    theCamera = GetComponent<Camera>();
    theCamera.orthographicSize = MaxOrthoSize();
    TransformCameraPosition(Vector3.zero);
	}
	
	void Update () {
    if (Input.GetButtonDown("Fire3") || Input.GetButtonDown("Fire2")) {
      middleMouseButtonPressed = true;
    }

    if (Input.GetButtonUp("Fire3") || Input.GetButtonUp("Fire2")) {
      middleMouseButtonPressed = false;
    }

    if (middleMouseButtonPressed) {
      Vector3 worldCoordDelta = theCamera.ScreenToWorldPoint(lastMousePosition) - theCamera.ScreenToWorldPoint(Input.mousePosition);
      TransformCameraPosition(worldCoordDelta);
    } else if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > float.Epsilon) {
      float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
      theCamera.orthographicSize = Mathf.Clamp(theCamera.orthographicSize - (mouseScroll * cameraZoomSpeed), 1f, MaxOrthoSize());
      TransformCameraPosition(Vector3.zero);
    }

    lastMousePosition = Input.mousePosition;
  }

  public void TransformCameraPosition(Vector3 worldCoordDelta) {
    float verticalExtent = theCamera.orthographicSize;
    // Half the camera width
    float horizontalExtent = verticalExtent * ((float)Screen.width / Screen.height);

    // we want to say, the place where position - horizontalExtent = 0
    float minX = -0.5f + horizontalExtent;
    float maxX = boardWidth - 0.5f - horizontalExtent;
    float minY = -0.5f + verticalExtent;
    float maxY = boardHeight - 0.5f - verticalExtent;
    Vector3 newPosition = new Vector3(
      Mathf.Clamp(theCamera.transform.position.x + worldCoordDelta.x, minX, maxX),
      Mathf.Clamp(theCamera.transform.position.y + worldCoordDelta.y, minY, maxY),
      theCamera.transform.position.z
    );
    theCamera.transform.position = newPosition;
  }

  public float MaxOrthoSize() {
    // this works since our tile is 1 world-unit
    // set a sane minimum max as well.
    var horizontalOrtho = boardWidth * ((float)Screen.height / Screen.width) / 2.5f;
    return Mathf.Max(4, Mathf.Min(horizontalOrtho, boardHeight / 2.5f));
  }
}

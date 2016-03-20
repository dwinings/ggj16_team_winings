using UnityEngine;
using System.Collections;

namespace Wisp.ElementalDefense {
  public class CameraMovement : MonoBehaviour {
    private int boardWidth = -1;
    private int boardHeight = -1;
    public float cameraZoomSpeed;
    public float cameraPanSpeed;
    public bool middleMouseButtonPressed;
    private Vector3 lastMousePosition = Vector3.zero;
    private Camera theCamera;

    void Start() {
      theCamera = GetComponent<Camera>();
      theCamera.orthographicSize = MaxOrthoSize();
      TransformCameraPosition(Vector3.zero);
    }

    public int BoardWidth {
      get {
        if (boardWidth <= 0) {
          Board board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
          boardWidth = board.width;
        }
        return boardWidth;
      }
    }

    public int BoardHeight {
      get {
        if (boardHeight <= 0) {
          Board board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
          boardHeight = board.height;
        }
        return boardHeight;
      }
    }

    void Update() {
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
      float maxX = BoardWidth - 0.5f - horizontalExtent;
      float minY = -0.5f + verticalExtent;
      float maxY = BoardHeight - 0.5f - verticalExtent;
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
      var horizontalOrtho = BoardWidth * ((float)Screen.height / Screen.width) / 2.5f;
      return Mathf.Max(4, Mathf.Min(horizontalOrtho, BoardHeight / 2.5f));
    }
  }
}

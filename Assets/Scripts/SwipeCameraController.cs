using UnityEngine;
using System.Collections;

public class SwipeCameraController : MonoBehaviour {

    Rotation cameraRotate;
    bool isRotating = false;
    [SerializeField] Transform cameraTrans;

	// Use this for initialization
	void Start () {
        cameraRotate = Camera.main.GetComponent<Rotation>();
	}

    private void FixedUpdate()
    {
        transform.position = cameraTrans.position;
    }

    private void LateUpdate()
    {
        transform.position = cameraTrans.position;
    }

    // Update is called once per frame
    void Update()
    {
        #region Debug Controls
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                cameraRotate.TriggerRotation(Direction.Left);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                cameraRotate.TriggerRotation(Direction.Right);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                cameraRotate.TriggerRotation(Direction.Up);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                cameraRotate.TriggerRotation(Direction.Down);
            }

            #endregion

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                if (hit.transform == transform)
                {
                    Direction direction;
                    Vector2 dir = Input.GetTouch(0).deltaPosition;

                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        if (dir.x > 0)
                            direction = Direction.Left;
                        else
                            direction = Direction.Right;
                    }
                    else
                    {
                        if (dir.y > 0)
                            direction = Direction.Up;
                        else
                            direction = Direction.Down;
                    }

                    cameraRotate.TriggerRotation(direction);
                }
            }
        }
    }
}

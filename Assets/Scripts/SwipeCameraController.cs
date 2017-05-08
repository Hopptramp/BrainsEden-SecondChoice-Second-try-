using UnityEngine;
using System.Collections;

public class SwipeCameraController : MonoBehaviour {

    [SerializeField] float rotateSpeed = 0.04f;
    enum SwipeDirection { Up, Down, Left, Right, }
    Rotation cameraRotate;
    bool isRotating = false;
    [SerializeField] Transform cameraPos;

	// Use this for initialization
	void Start () {
        cameraRotate = Camera.main.GetComponent<Rotation>();
	}

    private void FixedUpdate()
    {
        transform.position = cameraPos.position;
    }

    private void LateUpdate()
    {
        transform.position = cameraPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = cameraPos.position;
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Rotate(SwipeDirection.Left);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Rotate(SwipeDirection.Right);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Rotate(SwipeDirection.Up);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Rotate(SwipeDirection.Down);
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                if (hit.transform == transform)
                {
                    SwipeDirection direction;
                    Vector2 dir = Input.GetTouch(0).deltaPosition;

                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        if (dir.x > 0)
                            direction = SwipeDirection.Left;
                        else
                            direction = SwipeDirection.Right;
                    }
                    else
                    {
                        if (dir.y > 0)
                            direction = SwipeDirection.Up;
                        else
                            direction = SwipeDirection.Down;
                    }

                    Rotate(direction);
                }
            }
        }
    }

    void Rotate(SwipeDirection _direction)
    {
        Vector3 rotation = Vector3.zero;
        switch (_direction)
        {
            case SwipeDirection.Up:
                rotation = new Vector3(90, 0, 0);
                break;
            case SwipeDirection.Down:
                rotation = new Vector3(-90, 0, 0);
                break;
            case SwipeDirection.Left:
                rotation = new Vector3(0, -90, 0);
                break;
            case SwipeDirection.Right:
                rotation = new Vector3(0, 90, 0);
                break;
            default:
                break;
        }

        StopAllCoroutines();
        StartCoroutine(RotateSelf(rotation));
        cameraRotate.TriggerRotation((Direction)((int)_direction));
    }

    IEnumerator RotateSelf(Vector3 _rotation)
    {
        Quaternion LocalRotation = Quaternion.Inverse(Quaternion.Euler(_rotation)) * cameraPos.rotation;
        isRotating = true;
        Quaternion newRotation = transform.localRotation * Quaternion.Euler(_rotation);
        while(Quaternion.Angle(transform.localRotation, LocalRotation) > 1)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, LocalRotation, rotateSpeed);
            yield return null; 
        }
        transform.localRotation = LocalRotation;
        isRotating = false;
        print(transform.forward);
    }
}

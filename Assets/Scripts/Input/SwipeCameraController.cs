using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SwipeCameraController : MonoBehaviour, IPointerEnterHandler
{

    Rotation cameraRotate;
    MenuNavigation menuNavigation;
    Animator animator;
    bool isRotating = false;
    [SerializeField] Transform target;
    [SerializeField] bool isMenu = false;

	// Use this for initialization
	void Start () {
        if (!isMenu)
        {
            cameraRotate = Camera.main.GetComponent<Rotation>();
            animator = GetComponent<Animator>();
        }
        else
            menuNavigation = Camera.main.GetComponent<MenuNavigation>();
	}

    private void FixedUpdate()
    {
        if (!isMenu)
            transform.position = target.position;
    }

    private void LateUpdate()
    {
        if(!isMenu)
            transform.position = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMenu)
        {
            if (menuNavigation.isRotating)
                return;
        }
        else
        {
            if (cameraRotate.isRotating)
                return;
        }
        #region Debug Controls
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cameraRotate.TriggerRotation(Direction.Left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cameraRotate.TriggerRotation(Direction.Right);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cameraRotate.TriggerRotation(Direction.Up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
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
                        direction = Direction.Down;
                    else
                        direction = Direction.Up;
                }

                if (!isMenu)
                {
                    cameraRotate.TriggerRotation(direction);
                    TriggerAnimation(direction);
                }
                else
                    menuNavigation.TriggerRotation(direction);
            }

        }
    }

    void TriggerAnimation(Direction _direction)
    {
        switch (_direction)
        {
            case Direction.Up:
                animator.SetTrigger("CameraUp");
                break;
            case Direction.Down:
                animator.SetTrigger("CameraDown");
                break;
            case Direction.Left:
                animator.SetTrigger("CameraLeft");
                break;
            case Direction.Right:
                animator.SetTrigger("CameraRight");
                break;
            default:
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("The cursor entered the selectable UI element.");
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
                direction = Direction.Down;
            else
                direction = Direction.Up;
        }

        if (!isMenu)
            cameraRotate.TriggerRotation(direction);
        else
            menuNavigation.TriggerRotation(direction);
    }
}

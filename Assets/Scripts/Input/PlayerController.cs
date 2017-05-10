using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    [SerializeField] Transform target;
    [SerializeField] FixedPlayerMovement playerMovement;
    [SerializeField] GameObject[] buttons;

    private RegisterTouchInput[] inputs;

    #region MonoBehaviour

    private void Awake()
    {
        inputs = GetComponentsInChildren<RegisterTouchInput>();
        foreach(RegisterTouchInput input in inputs)
        {
            input.controller = this;
        }
    }

    // Use this for initialization
    void Start () {
        CheckNewOrientation();

        // add delegate event
        GameManager.instance.postRotation += CheckNewOrientation;
    }

    private void FixedUpdate()
    {
        transform.position = target.position;
    }

    private void LateUpdate()
    {
        transform.position = target.position;
    }

    #endregion

    #region Button input

    /// <summary>
    /// Trigger player movement left or right
    /// </summary>
    /// <param name="isLeft"></param>
    public void MoveHorizontal(bool isLeft)
    {
        if (isLeft)
            playerMovement.MoveLeft();
        else
            playerMovement.MoveRight();
    }

    /// <summary>
    /// Trigger player movement up or down
    /// </summary>
    /// <param name="isUp"></param>
    public void MoveVertical(bool isUp)
    {
        if (isUp)
            playerMovement.MoveUp();
        else
            playerMovement.MoveDown();
    }

    public void CheckNewOrientation()
    {
        CameraState cameraState = GameManager.instance.m_CameraState;

        switch (cameraState)
        {
            case CameraState.Above: // all active
                for(int i = 0; i < buttons.Length; ++i)
                {
                    buttons[i].SetActive(true);
                }

                if(target.up == Vector3.right)
                {
                    inputs[0].direction = Direction.Up;
                    inputs[1].direction = Direction.Down;
                    inputs[2].direction = Direction.Left;
                    inputs[3].direction = Direction.Right;
                }
                else if (target.up == -Vector3.right)
                {
                    inputs[0].direction = Direction.Up;
                    inputs[1].direction = Direction.Down;
                    inputs[2].direction = Direction.Right;
                    inputs[3].direction = Direction.Left;
                }
                else if (target.up == Vector3.forward)
                {
                    inputs[0].direction = Direction.Left;
                    inputs[1].direction = Direction.Right;
                    inputs[2].direction = Direction.Up;
                    inputs[3].direction = Direction.Down;
                }
                else
                {
                    inputs[0].direction = Direction.Right;
                    inputs[1].direction = Direction.Left;
                    inputs[2].direction = Direction.Down;
                    inputs[3].direction = Direction.Up;
                }

                
                break;
            case CameraState.Left: // front & behind active
                for (int i = 0; i < buttons.Length; ++i)
                {
                    if (i >= 2)
                        buttons[i].SetActive(true);
                    else
                        buttons[i].SetActive(false);
                }

                inputs[2].direction = Direction.Left;
                inputs[3].direction = Direction.Right;
                break;
            case CameraState.Right: // front & behind active
                for (int i = 0; i < buttons.Length; ++i)
                {
                    if (i >= 2)
                        buttons[i].SetActive(true);
                    else
                        buttons[i].SetActive(false);
                }

                inputs[2].direction = Direction.Right;
                inputs[3].direction = Direction.Left;
                break;
            case CameraState.Front: // left & right active
                for (int i = 0; i < buttons.Length; ++i)
                {
                    if (i < 2)
                        buttons[i].SetActive(true);
                    else
                        buttons[i].SetActive(false);
                }

                inputs[0].direction = Direction.Right;
                inputs[1].direction = Direction.Left;
                break;
            case CameraState.Behind: // left & right active
                for (int i = 0; i < buttons.Length; ++i)
                {
                    if (i < 2)
                        buttons[i].SetActive(true);
                    else
                        buttons[i].SetActive(false);
                }

                inputs[0].direction = Direction.Left;
                inputs[1].direction = Direction.Right;
                break;
            case CameraState.Below: // all active (or none?)
                for (int i = 0; i < buttons.Length; ++i)
                {
                    buttons[i].SetActive(true);
                }

                inputs[0].direction = Direction.Left;
                inputs[1].direction = Direction.Right;
                inputs[2].direction = Direction.Up;
                inputs[3].direction = Direction.Down;
                break;
            default:
                break;
        }
    }

    #endregion
}

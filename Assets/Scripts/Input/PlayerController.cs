using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    [SerializeField] Transform target;
    [SerializeField] FixedPlayerMovement playerMovement;
    [SerializeField] GameObject[] buttons;
    Animator animator;

    private RegisterTouchInput[] inputs;

    #region MonoBehaviour

    private void Awake()
    {
        inputs = GetComponentsInChildren<RegisterTouchInput>();
        foreach(RegisterTouchInput input in inputs)
        {
            input.controller = this;
        }

        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
        CheckNewOrientation();

        // add delegate event
        GameManager.instance.postRotation += PostRotation;
    }

    /// <summary>
    /// Use for any logic required after rotation completion
    /// </summary>
    void PostRotation()
    {
        CheckNewOrientation();

    }

    private void Update()
    {
        #region Debug Controls
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveHorizontal(true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveHorizontal(false);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveVertical(true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveVertical(false);
        }

        #endregion
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

    #region Button Animation

    /// <summary>
    /// Trigger rotation animation
    /// </summary>
    /// <param name="_direction"></param>
    void PressAnimation(Direction _direction, CameraState _cameraState)
    {
        // if above or below, use default animation
        if (_cameraState == CameraState.Above || _cameraState == CameraState.Below)
        {
            animator.SetTrigger("Wobble" + _direction.ToString());
        }
        else // else inform of camera state
            animator.SetTrigger("Wobble" + _direction.ToString() + _cameraState.ToString());
    }

    void ButtonDeactivationAnimation(CameraState _cameraState)
    {
        switch (_cameraState)
        {
            case CameraState.Above:
                animator.SetTrigger("ShowAll");
                break;
            case CameraState.Left:
                animator.SetTrigger("ShowLeft");
                break;
            case CameraState.Right:
                animator.SetTrigger("ShowRight");
                break;
            case CameraState.Front:
                animator.SetTrigger("ShowFront");
                break;
            case CameraState.Behind:
                animator.SetTrigger("ShowBehind");
                break;
            case CameraState.Below:
                animator.SetTrigger("ShowBelow");
                break;
            default:
                break;
        }
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
        {
            playerMovement.MoveLeft();
            //PressAnimation(Direction.Left, GameManager.instance.m_CameraState);
            return;
        }

        playerMovement.MoveRight();
        //PressAnimation(Direction.Right, GameManager.instance.m_CameraState);
    }

    /// <summary>
    /// Trigger player movement up or down
    /// </summary>
    /// <param name="isUp"></param>
    public void MoveVertical(bool isUp)
    {
        if (isUp)
        {
            playerMovement.MoveUp();
            //PressAnimation(Direction.Up, GameManager.instance.m_CameraState);
            return;
        }

        playerMovement.MoveDown();
        //PressAnimation(Direction.Down, GameManager.instance.m_CameraState);

    }

    /// <summary>
    /// Ensure correct buttons are displayed after rotation
    /// Ensure correct direction applied
    /// </summary>
    public void CheckNewOrientation()
    {
        CameraState cameraState = GameManager.instance.m_CameraState;
        ButtonDeactivationAnimation(cameraState);

        switch (cameraState)
        {
            case CameraState.Above: // all active
                for(int i = 0; i < buttons.Length; ++i)
                {
                    buttons[i].SetActive(true);
                }
                
                // above perspective has 4 variations
                if(target.up == Vector3.right)
                {
                    inputs[0].direction = Direction.Down;
                    inputs[1].direction = Direction.Up;
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

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    [SerializeField] Transform target;
    [SerializeField] FixedPlayerMovement playerMovement;
    [SerializeField] Collider[] buttons; // LEFT, RIGHT, FRONT, BEHIND
    Animator animator;

    private CameraState cameraState;

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
        GameManager.instance.preRotation += PreRotation;
    }


    /// <summary>
    /// Use for any logic required before rotation completion
    /// </summary>
    /// <param name="_intendedState"> The state that is being rotated to</param>
    void PreRotation(CameraState _intendedState)
    {
        ButtonDeactivationAnimation(_intendedState);


    }
    /// <summary>
    /// Use for any logic required after rotation completion
    /// </summary>
    void PostRotation(CameraState _cameraState)
    {
        cameraState = _cameraState;
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

    /// <summary>
    /// Trigger button change animation
    /// </summary>
    /// <param name="_intendedState"></param>
    void ButtonDeactivationAnimation(CameraState _intendedState)
    {
        if(_intendedState == CameraState.Above || _intendedState == CameraState.Below)
        {
            if (target.forward == Vector3.right || target.forward == -Vector3.right)
            {
                animator.SetTrigger("ShowAllLeft");
            }
            else if (target.forward == Vector3.forward || target.forward == -Vector3.forward)
            {
                animator.SetTrigger("ShowAllFront");
            }
            return;
        }
        if (_intendedState == CameraState.Left || _intendedState == CameraState.Right)
        {
            if(cameraState == CameraState.Above || cameraState == CameraState.Below)
                animator.SetTrigger("ShowFrontAbove");
            else
                animator.SetTrigger("ShowFront");
        }
        else
        {
            if (cameraState == CameraState.Above || cameraState == CameraState.Below)
                animator.SetTrigger("ShowLeftAbove");
            else 
                animator.SetTrigger("ShowLeft");
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
            return;
        }

        playerMovement.MoveRight();
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
            return;
        }

        playerMovement.MoveDown();

    }

    /// <summary>
    /// Ensure correct buttons are displayed after rotation
    /// Ensure correct direction applied
    /// </summary>
    public void CheckNewOrientation()
    {
        switch (cameraState)
        {
            case CameraState.Above: // all active
                for(int i = 0; i < buttons.Length; ++i)
                {
                    buttons[i].enabled = true;
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
                        buttons[i].enabled = true;
                    else
                        buttons[i].enabled = false;
                }

                inputs[2].direction = Direction.Left;
                inputs[3].direction = Direction.Right;
                break;
            case CameraState.Right: // front & behind active
                for (int i = 0; i < buttons.Length; ++i)
                {
                    if (i >= 2)
                        buttons[i].enabled = true;
                    else
                        buttons[i].enabled = false;
                }

                inputs[2].direction = Direction.Right;
                inputs[3].direction = Direction.Left;
                break;
            case CameraState.Front: // left & right active
                for (int i = 0; i < buttons.Length; ++i)
                {
                    if (i < 2)
                        buttons[i].enabled = true;
                    else
                        buttons[i].enabled = false;
                }

                inputs[0].direction = Direction.Right;
                inputs[1].direction = Direction.Left;
                break;
            case CameraState.Behind: // left & right active
                for (int i = 0; i < buttons.Length; ++i)
                {
                    if (i < 2)
                        buttons[i].enabled = true;
                    else
                        buttons[i].enabled = false;
                }

                inputs[0].direction = Direction.Left;
                inputs[1].direction = Direction.Right;
                break;
            case CameraState.Below: // all active (or none?)
                for (int i = 0; i < buttons.Length; ++i)
                {
                    buttons[i].enabled = false;
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

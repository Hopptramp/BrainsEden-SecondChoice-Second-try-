using UnityEngine;
using System.Collections;

public enum Direction
{
    Up, Down, Left, Right,
}

public class FixedPlayerMovement : MonoBehaviour {

    [SerializeField] float movementDuration = 1;
    [SerializeField] float jumpDuration = 1;
    [SerializeField] float fallDuration = 1;
    CameraState camState;
    Transform cameraParent;
    [SerializeField] LayerMask obstuctionObjects;
    enum ObstructionType { None, Drop, CanJump, Obstruction }
    [SerializeField]
    private AnimationCurve LinearMovement, JumpLinear, DropLinear;
    private Animator m_animator;
    [SerializeField]
    private Transform child;
    private bool moving;
    public bool jumping;
    private float childOffset;  
    public Vector3 jumpCamTarget { get { return jumping ? new Vector3(transform.position.x, m_animator.transform.localPosition.y, transform.position.z): transform.position; } }
    

	// Use this for initialization
	void Start () {
        cameraParent = Camera.main.transform.parent;
        m_animator = GetComponentInChildren<Animator>();
        m_animator.Play("Idle");
        childOffset = child.transform.localPosition.y;
       
	}
	
	// Update is called once per frame
	void Update ()
    {

    }

    /// <summary>
    /// move the character in the direction required
    /// </summary>
    /// <param name="_direction"></param>
    void MoveCharacter(Vector3 _direction)
    {
        ObstructionType temp =(CheckObstruction(_direction));
        if (!moving)
        {
            switch (temp)
            {
                case ObstructionType.None:
                    m_animator.SetTrigger("Move");
                    StartCoroutine(SmoothMoveCharacter(_direction, LinearMovement));
                    break;
                case ObstructionType.Drop:
                    m_animator.SetTrigger("Drop");
                    StartCoroutine(SmoothMoveCharacter(_direction, DropLinear));                    
                    break;
                case ObstructionType.CanJump:
                    StartCoroutine(Jump(_direction));
                    break;
                case ObstructionType.Obstruction:
                    break;
                default:
                    break;
            }            
        }
    }

    /// <summary>
    /// check if any blocks are in the way
    /// </summary>
    /// <param name="_direction"></param>
    /// <returns></returns>
    ObstructionType CheckObstruction(Vector3 _direction)
    {
        if (Physics.Raycast(transform.position, _direction, 1, obstuctionObjects))
        {
            if (!Physics.Raycast(transform.position + _direction, Vector3.up, 1, obstuctionObjects))
            {
                return ObstructionType.CanJump;
            }
            return ObstructionType.Obstruction;
        }
        if (!Physics.Raycast(transform.position + _direction, Vector3.down, 1, obstuctionObjects))
            return ObstructionType.Drop;

            return ObstructionType.None;
    }

    /// <summary>
    /// smooth movement of the character (possibly to be replaced with root animation)
    /// </summary>
    /// <param name="_translation"></param>
    /// <returns></returns>
    IEnumerator SmoothMoveCharacter(Vector3 _translation, AnimationCurve _linearCurve)
    {
        Vector3 newPos = transform.position + _translation;
        Vector3 origin = transform.position;
        Vector3 temp = origin;
        moving = true;  
        float t = 0;
        
        transform.LookAt(newPos);

        while (t <= movementDuration)
        {
            temp = Vector3.Lerp(origin, newPos, _linearCurve.Evaluate(t / movementDuration));
            transform.position = temp;
            yield return null;
            t += Time.deltaTime;
        }
        transform.position = newPos;
        moving = false;

        OnMovementComplete();
    }

    IEnumerator Fall(Vector3 _translation)
    {
        Vector3 newPos = transform.position + _translation;
        Vector3 origin = transform.position;
        float t = 0;
        moving = true;
        while (t<fallDuration)
        {
            
            transform.position = Vector3.Lerp(origin, newPos, t/fallDuration);          
            yield return null;
            t += Time.deltaTime;
        }
        transform.position = newPos;
        moving = false;
        OnMovementComplete();
        
    }

    IEnumerator Jump(Vector3 _translation)
    {
        moving = true;
        jumping = true;
        Vector3 newPos = transform.position + _translation;
        Vector3 origin = transform.position;
        transform.LookAt(newPos);
        m_animator.SetTrigger("JumpNew");
        GameManager.instance.rotation.TriggerJumpingTracking(origin ,newPos + Vector3.up, jumpDuration, JumpLinear.keys[1].time -0.1f);
        float t = 0;
        while(t<jumpDuration)
        {
            transform.position = Vector3.Lerp(origin, newPos, JumpLinear.Evaluate(t / jumpDuration));
            
            yield return null;
            t += Time.deltaTime;
        }
        transform.position = newPos;
        yield return null;        
    }

    public void EndJump()
    {
        jumping = false;
        moving = false;
        transform.position += Vector3.up;
        m_animator.transform.localPosition = Vector3.zero;
        OnMovementComplete();
    }

    /// <summary>
    /// to be called after movement is applied to player
    /// </summary>
    public void OnMovementComplete()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, 1, obstuctionObjects))
        {
            //MoveCharacter(new Vector3(0, -10, 0));
            StartCoroutine(Fall(new Vector3(0, -1, 0)));
        }
    }

    /// <summary>
    /// button input for move up
    /// </summary>
    public void MoveUp()
    {
        DefineTranslation(Direction.Up);
    }

    /// <summary>
    /// button input for move down
    /// </summary>
    public void MoveDown()
    {
        DefineTranslation(Direction.Down);
    }

    /// <summary>
    /// button input for move left
    /// </summary>
    public void MoveLeft()
    {
        DefineTranslation(Direction.Left);
    }

    /// <summary>
    /// button input for move right
    /// </summary>
    public void MoveRight()
    {
        DefineTranslation(Direction.Right);
    }

    /// <summary>
    /// Define the translation direction, using the camera vectors
    /// </summary>
    /// <param name="_direction"></param>
    void DefineTranslation(Direction _direction)
    {
        switch (_direction)
        {
            case Direction.Up:
                MoveCharacter(cameraParent.up);
                break;
            case Direction.Down:
                MoveCharacter(-cameraParent.up);
                break;
            case Direction.Left:
                MoveCharacter(-cameraParent.right);
                break;
            case Direction.Right:
                MoveCharacter(cameraParent.right);
                break;
            default:
                break;
        }
    }
}

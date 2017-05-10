using UnityEngine;
using System.Collections;

public enum Direction
{
    Up, Down, Left, Right,
}

public class FixedPlayerMovement : MonoBehaviour {

    [SerializeField] float movementDuration = 1;
    CameraState camState;
    Transform cameraParent;
    [SerializeField] LayerMask obstuctionObjects;
    enum ObstructionType { None, Drop, CanJump, Obstruction }
    [SerializeField]
    private AnimationCurve LinearMovement, VerticalMovement;
    private Animator m_animator;
    [SerializeField]
    private Transform child;
    private bool moving;
    private float childOffset;
    



	// Use this for initialization
	void Start () {
        cameraParent = Camera.main.transform.parent;
        m_animator = GetComponentInChildren<Animator>();
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
                    StartCoroutine(SmoothMoveCharacter(_direction));
                    break;
                case ObstructionType.Drop:
                    StartCoroutine(SmoothMoveCharacter(_direction));
                    break;
                case ObstructionType.CanJump:
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
    IEnumerator SmoothMoveCharacter(Vector3 _translation)
    {
        Vector3 newPos = transform.position + _translation;
        Vector3 origin = transform.position;
        Vector3 temp = origin;
        moving = true;  
        float t = 0;
        if (m_animator)
            m_animator.SetTrigger("Move");
        transform.LookAt(newPos);
        while (t <= movementDuration)
        {
            temp = Vector3.Lerp(origin, newPos, LinearMovement.Evaluate(t / movementDuration));
            if (child!= null)
            {
                transform.position = temp;
                child.transform.localPosition = new Vector3(0, childOffset + VerticalMovement.Evaluate(t / movementDuration), 0);
            }
            else
                transform.position = new Vector3(temp.x, origin.y + VerticalMovement.Evaluate(t / movementDuration), temp.z);
            

            yield return null;
            t += Time.deltaTime;
        }
        transform.position = newPos;
        moving = false;

        //while (transform.position != newPos)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, newPos, movementSpeed);
        //    yield return null;
        //}

        OnMovementComplete();
    }

    /// <summary>
    /// to be called after movement is applied to player
    /// </summary>
    public void OnMovementComplete()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, 1, obstuctionObjects))
        {
            MoveCharacter(new Vector3(0, -10, 0));
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

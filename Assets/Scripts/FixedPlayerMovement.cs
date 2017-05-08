using UnityEngine;
using System.Collections;

public enum Direction
{
    Up, Down, Left, Right,
}

public class FixedPlayerMovement : MonoBehaviour {

    [SerializeField] float movementSpeed = 1;
    CameraState camState;
    Transform cameraParent;
    [SerializeField] LayerMask obstuctionObjects;


	// Use this for initialization
	void Start () {
        cameraParent = Camera.main.transform.parent;
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
        if(!CheckObstruction(_direction))
            StartCoroutine(SmoothMoveCharacter(_direction));
    }

    /// <summary>
    /// check if any blocks are in the way
    /// </summary>
    /// <param name="_direction"></param>
    /// <returns></returns>
    bool CheckObstruction(Vector3 _direction)
    {
        if (Physics.Raycast(transform.position, _direction, 1, obstuctionObjects))
            return true;
        return false;
    }

    /// <summary>
    /// smooth movement of the character (possibly to be replaced with root animation)
    /// </summary>
    /// <param name="_translation"></param>
    /// <returns></returns>
    IEnumerator SmoothMoveCharacter(Vector3 _translation)
    {
        Vector3 newPos = transform.position + _translation;
        while (transform.position != newPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, movementSpeed);
            yield return null;
        }

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

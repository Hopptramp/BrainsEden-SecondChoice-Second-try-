using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour
{
	[SerializeField]
	//the distance for the camera to rotate around the world
	private float cameraDist = 10f;

	[SerializeField]
	//time taken for rotation
	private float rotateTime = 0.5f;

    CameraState currentState;

	public GameObject compassPrefab, compassOrigin;

	private GameObject origin;
    public GameObject player;
    [SerializeField]
    private bool useKeyboard;
    public bool isRotating = false;

    private bool left, right, up, down;

	[SerializeField]
	private bool clear = true;

	void Awake ()
	{
		if (!compassOrigin)
		{
			//-GameObject.Instantiate (compassPrefab);
		}


        //sanity check for origin
        if (!origin)
        {
            origin = new GameObject();
            origin.transform.position = Vector3.zero;
            origin.transform.rotation.eulerAngles.Set(0f, 0f, 0f);
        }

        transform.parent = origin.transform;
        transform.position = transform.parent.position - new Vector3(0f, 0f, cameraDist);

    }

	//set the camera to start facing the origin at cameraDist distance
	void Start ()
	{
		if (!compassOrigin)
		{
			compassOrigin = Compass.instance.compassOrigin;
		}


        currentState = GameManager.instance.m_CameraState;
	}
    

	void FixedUpdate ()
	{
		if (compassOrigin)
		{
			compassOrigin.transform.rotation = origin.transform.rotation;
		}
	}

	void Update ()
	{
        if (useKeyboard)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow) && clear)
            {
                RotateUp();
                ScoreManager.instance.IncreaseFlips();
            }

            else if (Input.GetKeyUp(KeyCode.DownArrow) && clear)
            {
                RotateDown();
                ScoreManager.instance.IncreaseFlips();
            }

            else if (Input.GetKeyUp(KeyCode.RightArrow) && clear)
            {
                RotateRight();
                ScoreManager.instance.IncreaseFlips();
            }

            else if (Input.GetKeyUp(KeyCode.LeftArrow) && clear)
            {
                RotateLeft();
                ScoreManager.instance.IncreaseFlips();
            }

        }
        else
        {
            if (up && clear)
            {
                RotateUp();
                ScoreManager.instance.IncreaseFlips();
            }

            else if (down && clear)
            { 
                RotateDown();
                ScoreManager.instance.IncreaseFlips();
            }

            else if (right && clear)
            { 
                RotateRight();
                ScoreManager.instance.IncreaseFlips();
            }

            else if (left && clear)
            { 
                RotateLeft();
                ScoreManager.instance.IncreaseFlips();
            }
        }
        ResetBools();
		origin.transform.position = player.transform.position;
	}
    void ResetBools()
    {
        left = false;
        right = false;
        up = false;
        down = false;
    }

    public void LeftPress() {left = true; }
    public void RightPress() { right = true; }
    public void UpPress() { up = true; }
    public void DownPress() { down = true; }

    public void TriggerRotation(Direction _direction)
    {
        // trigger any pre rotation logic via delegates
        GameManager.instance.preRotation(PredictCameraState(_direction));


        isRotating = true;
        switch (_direction)
        {
            case Direction.Up:
                RotateUp();
                break;
            case Direction.Down:
                RotateDown();
                break;
            case Direction.Left:
                RotateLeft();
                break;
            case Direction.Right:
                RotateRight();
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// Predicts what camera state it will be after the rotation
    /// </summary>
    /// <param name="_direction"></param>
    /// <returns></returns>
    CameraState PredictCameraState(Direction _direction)
    {
        switch (GameManager.instance.m_CameraState)
        {
            case CameraState.Above:
                switch (_direction)
                {
                    case Direction.Up:
                        if (transform.up == Vector3.right)
                            return CameraState.Right;

                        else if (transform.up == -Vector3.right)
                            return CameraState.Left;

                        else if (transform.up == Vector3.forward)
                            return CameraState.Behind;

                        else
                            return CameraState.Front;

                    case Direction.Down:
                        if (transform.up == Vector3.right)
                            return CameraState.Left;

                        else if (transform.up == -Vector3.right)
                            return CameraState.Right;

                        else if (transform.up == Vector3.forward)
                            return CameraState.Front;

                        else
                            return CameraState.Behind;

                    case Direction.Left:
                        if (transform.up == Vector3.right)
                            return CameraState.Behind;

                        else if (transform.up == -Vector3.right)
                            return CameraState.Front;

                        else if (transform.up == Vector3.forward)
                            return CameraState.Left;

                        else
                            return CameraState.Right;

                    case Direction.Right:
                        if (transform.up == Vector3.right)
                            return CameraState.Front;

                        else if (transform.up == -Vector3.right)
                            return CameraState.Behind;

                        else if (transform.up == Vector3.forward)
                            return CameraState.Right;

                        else
                            return CameraState.Left;

                    default:
                        break;
                }
                break;
            case CameraState.Left:
                switch (_direction)
                {
                    case Direction.Up:
                        return CameraState.Above;

                    case Direction.Down:
                        return CameraState.Below;

                    case Direction.Left:
                        return CameraState.Behind;

                    case Direction.Right:
                        return CameraState.Front;

                    default:
                        break;
                }
                break;
            case CameraState.Right:
                switch (_direction)
                {
                    case Direction.Up:
                        return CameraState.Above;

                    case Direction.Down:
                        return CameraState.Below;

                    case Direction.Left:
                        return CameraState.Front;

                    case Direction.Right:
                        return CameraState.Behind;

                    default:
                        break;
                }
                break;
            case CameraState.Front:
                switch (_direction)
                {
                    case Direction.Up:
                        return CameraState.Above;

                    case Direction.Down:
                        return CameraState.Below;

                    case Direction.Left:
                        return CameraState.Left;

                    case Direction.Right:
                        return CameraState.Right;

                    default:
                        break;
                }
                break;
            case CameraState.Behind:
                switch (_direction)
                {
                    case Direction.Up:
                        return CameraState.Above;

                    case Direction.Down:
                        return CameraState.Below;

                    case Direction.Left:
                        return CameraState.Right;

                    case Direction.Right:
                        return CameraState.Left;

                    default:
                        break;
                }
                break;

            case CameraState.Below:
                switch (_direction)
                {
                    case Direction.Up:
                        if (transform.up == Vector3.right)
                            return CameraState.Left;

                        else if (transform.up == -Vector3.right)
                            return CameraState.Right;

                        else if (transform.up == Vector3.forward)
                            return CameraState.Front;

                        else
                            return CameraState.Behind;

                    case Direction.Down:
                        if (transform.up == Vector3.right)
                            return CameraState.Right;

                        else if (transform.up == -Vector3.right)
                            return CameraState.Left;

                        else if (transform.up == Vector3.forward)
                            return CameraState.Behind;

                        else
                            return CameraState.Front;

                    case Direction.Left:
                        if (transform.up == Vector3.right)
                            return CameraState.Front;

                        else if (transform.up == -Vector3.right)
                            return CameraState.Behind;

                        else if (transform.up == Vector3.forward)
                            return CameraState.Right;

                        else
                            return CameraState.Left;

                    case Direction.Right:
                        if (transform.up == Vector3.right)
                            return CameraState.Behind;

                        else if (transform.up == -Vector3.right)
                            return CameraState.Front;

                        else if (transform.up == Vector3.forward)
                            return CameraState.Left;

                        else
                            return CameraState.Right;

                    default:
                        break;
                }
                break;
        }
        return CameraState.None;
    }


    void TriggeriTween(float _angle, string _axis)
    {
        isRotating = true;
        iTween.RotateAdd(origin, iTween.Hash(_axis, _angle, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", player, "oncomplete", "CheckRotation"));
    }

    public void RotateUp ()
	{
		//pass the camera's new fwd
		//BlockManager.instance.DepthShade (origin.transform.up);

		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("x", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc,"onstarttarget",player,"onstart", "FreezeUnfreeze", "onstartparams" , true,"oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
    }

	public void RotateDown ()
	{
		//pass the camera's new fwd
		//BlockManager.instance.DepthShade (origin.transform.up * -1);

		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("x", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
    }

	public void RotateLeft ()
	{
		//pass the camera's new fwd
		//BlockManager.instance.DepthShade (origin.transform.right * -1);

		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("y", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete" ,"CheckRotation"));
    }

	public void RotateRight ()
	{
		//pass the camera's new fwd
		//BlockManager.instance.DepthShade (origin.transform.right);

		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("y", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
    }

    /// <summary>
	/// Checks the rotation of the camera and rites it if at a stupid orientation
	/// </summary>
	void CheckRotation()
    {
        //if not looking up or down and the camera is off orientation
        if (transform.right == Vector3.up || (-1 * transform.right) == Vector3.up || transform.up == Vector3.down)
        {
            StartCoroutine(CorrectRotation());
        }
        else
        {
            ClearToRotate();
        }
    }

    IEnumerator CorrectRotation()
    {
        //pause for a little while?
        //yield return new WaitForSeconds (0.25f)

        //correct the camera rotation
        iTween.RotateTo(origin, iTween.Hash("z", 0f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "ClearToRotate"));

        yield return null;
    }

    void ClearToRotate()
    {
        origin.transform.position = player.transform.position;
        GameManager.instance.UpdateCameraState();
        clear = true;
        isRotating = false;
        //player.GetComponent<Player_Movement>().FreezeUnfreeze(false);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rotation : MonoBehaviour
{
	[SerializeField]
	//the distance for the camera to rotate around the world
	private float cameraDist = 10f;
	[SerializeField]
	//time taken for rotation
	private float rotateTime = 0.5f;

    RotationData rotationData;
	public GameObject compassPrefab, compassOrigin;
	private GameObject origin;
    private GameObject player;
    private Vector3 tempTarget;
    FixedPlayerMovement playerScript;


    public bool isRotating = false;
    private bool left, right, up, down;
	private bool clear = true;

    #region MonoBehaviour

    void Awake ()
	{
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
        
        rotationData = GameManager.rotationData;
        player = rotationData.target;
        playerScript = player.GetComponent<FixedPlayerMovement>();
	}
    
	void FixedUpdate ()
	{
		if (compassOrigin)
		{
			compassOrigin.transform.rotation = origin.transform.rotation;
		}
	}

    void LateUpdate()
    {
       // origin.transform.position = playerScript.cameraTarget;
    }

	void Update ()
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
        
        ResetBools();
		//origin.transform.position = player.transform.position;

        if(!playerScript.jumping)
            origin.transform.position = playerScript.transform.position;
	}

    #endregion

    #region Calculating States
    /// <summary>
    /// initialise the rotation data struct and tell game manager
    /// </summary>
    /// <param name="_direction"></param>
    void InitialisePreRotation(Direction _direction)
    {
        KeyValuePair<int, int> states = PredictCameraAndTransitionStates(_direction);
        rotationData.intendedState = (CameraState)states.Key;
        rotationData.transitionState = (TransitionState)states.Value;

        // trigger any pre rotation logic via delegates
        GameManager.instance.preRotation(rotationData);
    }

    /// <summary>
    /// Predicts what camera state it will be after the rotation
    /// KEY - CameraState
    /// VALUE - TransitionState
    /// </summary>
    /// <param name="_direction"></param>
    /// <returns></returns>
    KeyValuePair<int, int> PredictCameraAndTransitionStates(Direction _direction)
    {
        // KEY - CameraState
        // VALUE - TransitionState
        switch (GameManager.instance.cameraState)
        {
            case CameraState.Above:
                switch (_direction)
                {
                    case Direction.Up:
                        if (transform.up == Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromAboveToRight));

                        else if (transform.up == -Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromAboveToLeft));

                        else if (transform.up == Vector3.forward)
                            return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromAboveToFront));

                        else
                            return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromAboveToBehind));

                    case Direction.Down:
                        if (transform.up == Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromAboveToLeft));

                        else if (transform.up == -Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromAboveToRight));

                        else if (transform.up == Vector3.forward)
                            return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromAboveToBehind));

                        else
                            return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromAboveToFront));

                    case Direction.Left:
                        if (transform.up == Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromAboveToBehind));

                        else if (transform.up == -Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromAboveToFront));

                        else if (transform.up == Vector3.forward)
                            return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromAboveToLeft));
                        else
                            return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromAboveToRight));

                    case Direction.Right:
                        if (transform.up == Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromAboveToFront));

                        else if (transform.up == -Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromAboveToBehind));

                        else if (transform.up == Vector3.forward)
                            return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromAboveToRight));

                        else
                            return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromAboveToLeft));
                    default:
                        break;
                }
                break;
            case CameraState.Left:
                switch (_direction)
                {
                    case Direction.Up:
                        return new KeyValuePair<int, int>((int)CameraState.Above, ((int)TransitionState.FromLeftToAbove));

                    case Direction.Down:
                        return new KeyValuePair<int, int>((int)CameraState.Below, ((int)TransitionState.FromLeftToBelow));

                    case Direction.Left:
                        return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromLeftToFront));

                    case Direction.Right:
                        return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromLeftToBehind));
                    default:
                        break;
                }
                break;
            case CameraState.Right:
                switch (_direction)
                {
                    case Direction.Up:
                        return new KeyValuePair<int, int>((int)CameraState.Above, ((int)TransitionState.FromRightToAbove));

                    case Direction.Down:
                        return new KeyValuePair<int, int>((int)CameraState.Below, ((int)TransitionState.FromRightToBelow));

                    case Direction.Left:
                        return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromRightToBehind));

                    case Direction.Right:
                        return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromRightToFront));

                    default:
                        break;
                }
                break;
            case CameraState.Front:
                switch (_direction)
                {
                    case Direction.Up:
                        return new KeyValuePair<int, int>((int)CameraState.Above, ((int)TransitionState.FromFrontToAbove));

                    case Direction.Down:
                        return new KeyValuePair<int, int>((int)CameraState.Below, ((int)TransitionState.FromFrontToBelow));

                    case Direction.Left:
                        return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromFrontToRight));

                    case Direction.Right:
                        return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromFrontToLeft));

                    default:
                        break;
                }
                break;
            case CameraState.Behind:
                switch (_direction)
                {
                    case Direction.Up:
                        return new KeyValuePair<int, int>((int)CameraState.Above, ((int)TransitionState.FromBehindToAbove));

                    case Direction.Down:
                        return new KeyValuePair<int, int>((int)CameraState.Below, ((int)TransitionState.FromBehindToBelow));

                    case Direction.Left:
                        return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromBehindToLeft));

                    case Direction.Right:
                        return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromBehindToRight));

                    default:
                        break;
                }
                break;

            case CameraState.Below:
                switch (_direction)
                {
                    case Direction.Up:
                        if (transform.up == Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromBelowToRight));

                        else if (transform.up == -Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromBelowToLeft));

                        else if (transform.up == Vector3.forward)
                            return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromBelowToFront));

                        else
                            return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromBelowToBehind));

                    case Direction.Down:
                        if (transform.up == Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromBelowToLeft));

                        else if (transform.up == -Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromBelowToRight));

                        else if (transform.up == Vector3.forward)
                            return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromBelowToBehind));

                        else
                            return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromBelowToFront));

                    case Direction.Left:
                        if (transform.up == Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromBelowToBehind));

                        else if (transform.up == -Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromBelowToFront));

                        else if (transform.up == Vector3.forward)
                            return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromBelowToRight));

                        else
                            return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromBelowToLeft));

                    case Direction.Right:
                        if (transform.up == Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Front, ((int)TransitionState.FromBelowToFront));

                        else if (transform.up == -Vector3.right)
                            return new KeyValuePair<int, int>((int)CameraState.Behind, ((int)TransitionState.FromBelowToBehind));

                        else if (transform.up == Vector3.forward)
                            return new KeyValuePair<int, int>((int)CameraState.Left, ((int)TransitionState.FromBelowToLeft));

                        else
                            return new KeyValuePair<int, int>((int)CameraState.Right, ((int)TransitionState.FromBelowToRight));

                    default:
                        break;
                }
                break;
        }
        return new KeyValuePair<int, int>((int)CameraState.None, ((int)TransitionState.None)); ;
    }

    #endregion

    public void TriggerRotation(Direction _direction)
    {
        InitialisePreRotation(_direction);


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

    public void TriggerJumpingTracking(Vector3 _target, float _duration)
    {
        Vector3 target = _target;
        switch (rotationData.currentState)
        {
            case CameraState.Above:
                target.z = cameraDist;
                break;
            case CameraState.Left:
                target.x = cameraDist;
                break;
            case CameraState.Right:
                target.x = -cameraDist;
                break;
            case CameraState.Front:
                target.y = -cameraDist;
                break;
            case CameraState.Behind:
                target.y = cameraDist;
                break;
            case CameraState.Below:
                target.z = -cameraDist;
                break;
            case CameraState.None:
                break;
            default:
                break;
        }

        StartCoroutine(JumpTracking(target, _duration));
    }

    IEnumerator JumpTracking(Vector3 _target, float _duration)
    {
        Vector3 currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / _duration;
            transform.position = Vector3.Lerp(currentPos, _target, t);
            yield return null;
        }
    }


    

    #region Input
    public void RotateUp ()
	{
		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("x", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc,"oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
    }

	public void RotateDown ()
	{
		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("x", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
    }

	public void RotateLeft ()
	{
		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("y", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete" ,"CheckRotation"));
    }

	public void RotateRight ()
	{
		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("y", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
    }

    void ResetBools()
    {
        left = false;
        right = false;
        up = false;
        down = false;
    }

    public void LeftPress() { left = true; }
    public void RightPress() { right = true; }
    public void UpPress() { up = true; }
    public void DownPress() { down = true; }

    #endregion

    #region Rotation Logic

    //void TriggeriTween(float _angle, string _axis)
    //{
    //    isRotating = true;
    //    iTween.RotateAdd(origin, iTween.Hash(_axis, _angle, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", player, "oncomplete", "CheckRotation"));
    //}

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
        GameManager.instance.UpdateRotationData(false);
        clear = true;
        isRotating = false;
        //player.GetComponent<Player_Movement>().FreezeUnfreeze(false);
    }

    #endregion
}

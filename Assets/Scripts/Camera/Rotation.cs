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
    private Transform tempTarget;
    FixedPlayerMovement playerScript;


    public bool isRotating = false;
    private bool isStatictarget = false;

    #region MonoBehaviour

    void Awake ()
	{
        //sanity check for origin
        if (!origin)
        {
            origin = new GameObject();        
        }

        origin.transform.position = Vector3.zero;
        origin.transform.rotation.eulerAngles.Set(-90f, 0f, 0f);
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

        GameManager.instance.postRotation += PostRotation;
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
        //origin.transform.position = playerScript.jumpCamTarget;
    }

	void Update ()
	{
        if(!playerScript.jumping)
            origin.transform.position = isStatictarget ? tempTarget.transform.position : playerScript.transform.position;
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
        rotationData.fromState = GameManager.cameraState;

        // trigger any pre rotation logic via delegates
        GameManager.instance.preRotation(rotationData);
    }

    /// <summary>
    /// From post rotation delegate
    /// </summary>
    /// <param name="_rotationData"></param>
    /// <param name="isInit"></param>
    void PostRotation(RotationData _rotationData, bool isInit)
    {
        rotationData = _rotationData;
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
        switch (GameManager.cameraState)
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

    #region Jumping

    public void TriggerJumpingTracking(Vector3 _origin, Vector3 _target, float _duration, float _keyframeValue)
    {
        
        StartCoroutine(JumpTracking(_origin, _target, _duration, _keyframeValue));
    }

    IEnumerator JumpTracking(Vector3 _origin, Vector3 _target, float _duration, float _keyframeValue)
    {
        Vector3 currentPos = origin.transform.position;
        Vector3 firstPos = currentPos + Vector3.up;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / _duration;
            if (t< _keyframeValue)
            {
                origin.transform.position = Vector3.Slerp(currentPos, firstPos, t / _keyframeValue);
            }
            else
            {
                origin.transform.position = Vector3.Slerp(firstPos, _target, (t-_keyframeValue) / (1-_keyframeValue));
            }
            
            yield return null;
        }
    }

    #endregion

    #region Rotation Logic

    public void AttachToTempTarget(bool _isAttaching)
    {
        isStatictarget = _isAttaching;
        if (!tempTarget)
            tempTarget = new GameObject().transform;
        tempTarget.transform.position = player.transform.position;
    }

    /// <summary>
    /// Trigger rotation in _axis to _angle
    /// </summary>
    /// <param name="_angle"></param>
    /// <param name="_axis"></param>
    public void TriggerRotation(float _angle, string _axis)
    {
        if (isRotating)
            return;

        RotateTo(_angle, _axis);   
    }

    /// <summary>
    /// Trigger rotation in _axis to _angle, over _rotationDuration
    /// </summary>
    /// <param name="_angle"></param>
    /// <param name="_axis"></param>
    public void TriggerRotation(float _angle, string _axis, float _rotationDuration)
    {
        if (isRotating)
            return;

        RotateTo(_angle, _axis, _rotationDuration);
    }

    /// <summary>
    /// Trigger rotation 90 degrees in direction
    /// </summary>
    /// <param name="_direction"></param>
    public void TriggerRotation(Direction _direction)
    {
        if (isRotating)
            return;

        // prompt prerotation
        InitialisePreRotation(_direction);

        isRotating = true;
        switch (_direction)
        {
            case Direction.Up:
                RotateAdd(90, "x");
                break;
            case Direction.Down:
                RotateAdd(-90, "x");
                break;
            case Direction.Left:
                RotateAdd(90, "y");
                break;
            case Direction.Right:
                RotateAdd(-90, "y");
                break;
            default:
                break;
        }
    }

    void RotateAdd(float _angle, string _axis)
    {
        isRotating = true;
        iTween.RotateAdd(origin, iTween.Hash(_axis, _angle, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "InitPostRotation"));
    }

    void RotateTo(float _angle, string _axis)
    {
        isRotating = true;
        iTween.RotateTo(origin, iTween.Hash(_axis, _angle, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "InitPostRotation"));
    }

    void RotateTo(float _angle, string _axis, float _rotateDuration)
    {
        isRotating = true;
        iTween.RotateTo(origin, iTween.Hash(_axis, _angle, "time", _rotateDuration, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "InitPostRotation"));
    }

    /// <summary>
    /// Checks the rotation of the camera and rights it if not correctly oriented
    /// </summary>
    void InitPostRotation()
    {
        //if not looking up or down and the camera is off orientation
        if (transform.right == Vector3.up || (-1 * transform.right) == Vector3.up || transform.up == Vector3.down)
            RotateTo(0, "z");
        else
        {
            origin.transform.position = player.transform.position;
            GameManager.instance.UpdateRotationData(false);
            isRotating = false;
        }
    }

    #endregion
}

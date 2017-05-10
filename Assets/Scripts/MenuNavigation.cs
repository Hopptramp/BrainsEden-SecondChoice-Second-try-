using UnityEngine;
using System.Collections;

public class MenuNavigation : MonoBehaviour {

    [SerializeField]
    //the distance for the camera to rotate around the world
    private float cameraDist = 10f;

    [SerializeField]
    //time taken for rotation
    private float rotateTime = 0.5f;

    CameraState currentState;

    GameObject desiredRotation, cameraParent;
    public GameObject target;

    public bool isRotating = false;

    private bool left, right, up, down;

    [SerializeField]
    private bool clear = true;

    #region MonoBehaviour

    // Use this for initialization
    void Awake () {

        if (!cameraParent)
        {
            cameraParent = new GameObject("Camera Parent");
            cameraParent.transform.position = Vector3.zero;
            cameraParent.transform.rotation.eulerAngles.Set(0f, 0f, 0f);
        }
        if (!desiredRotation)
        {
            desiredRotation = new GameObject("Desired Rotation");
            desiredRotation.transform.position = Vector3.zero;
            desiredRotation.transform.rotation.eulerAngles.Set(0f, 0f, 0f);        
        }

        transform.parent = cameraParent.transform;
        transform.position = transform.parent.position - new Vector3(0f, 0f, cameraDist);
    }

    // Update is called once per frame
    void Update()
    {
        #region Debug Controls
        if (!isRotating)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
                RotateUp();

            else if (Input.GetKeyUp(KeyCode.DownArrow))
                RotateDown();

            else if (Input.GetKeyUp(KeyCode.RightArrow))
                RotateRight();

            else if (Input.GetKeyUp(KeyCode.LeftArrow))
                RotateLeft();
        }
        #endregion

      //  cameraParent.transform.position = target.transform.position;
    }

    void FixedUpdate()
    {
        if (desiredRotation)
        {
            desiredRotation.transform.rotation = cameraParent.transform.rotation;
        }
    }


    #endregion

    #region Rotation Behaviour

    /// <summary>
    /// Triggers the iTween action
    /// </summary>
    /// <param name="_angle"></param>
    /// <param name="_axis"></param>
    void TriggeriTween(float _angle, string _axis)
    {
        isRotating = true;
        iTween.RotateAdd(cameraParent, iTween.Hash(_axis, _angle, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "PostRotate"));
    }

    public void TriggerRotation(Direction _direction)
    {
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

    public void RotateUp()
    {
        TriggeriTween(90f, "x");
    }

    public void RotateDown()
    {
        TriggeriTween(-90f, "x");
    }

    public void RotateLeft()
    {
        TriggeriTween(90f, "y");
    }

    public void RotateRight()
    {
        TriggeriTween(-90f, "y");
    }

    /// <summary>
    /// Any logic required when rotation ends.
    /// </summary>
    void PostRotate()
    {
        //if not looking up or down and the camera is off orientation
        if (transform.right == Vector3.up || -transform.right == Vector3.up || transform.up == Vector3.down)
        {
            iTween.RotateTo(cameraParent, iTween.Hash("z", 0f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "ClearToRotate"));
        }
        else
        {
            FinishedRotating();
        }
        //cameraParent.transform.position = target.transform.position;
    }

    //IEnumerator CorrectRotation()
    //{
    //    TriggeriTween()
    //    //correct the camera rotation
    //    iTween.RotateTo(origin, iTween.Hash("z", 0f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "ClearToRotate"));

    //    yield return null;
    //}

    void FinishedRotating()
    {
        isRotating = false;
    }
    #endregion
}

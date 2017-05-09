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

    GameObject compassOrigin, origin;
    public GameObject target;

    public bool isRotating = false;

    private bool left, right, up, down;

    [SerializeField]
    private bool clear = true;

    #region MonoBehaviour

    // Use this for initialization
    void Awake () {

        if (!origin)
        {
            origin = new GameObject("origin");
            origin.transform.position = Vector3.zero;
            origin.transform.rotation.eulerAngles.Set(0f, 0f, 0f);
        }
        if (!compassOrigin)
        {
            compassOrigin = new GameObject("compassOrigin");
            compassOrigin.transform.position = Vector3.zero;
            compassOrigin.transform.rotation.eulerAngles.Set(0f, 0f, 0f);        
        }

        transform.parent = origin.transform;
        transform.position = transform.parent.position - new Vector3(0f, 0f, cameraDist);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow) && clear)
            RotateUp();

        else if (Input.GetKeyUp(KeyCode.DownArrow) && clear)
            RotateDown();

        else if (Input.GetKeyUp(KeyCode.RightArrow) && clear)
            RotateRight();
         
        else if (Input.GetKeyUp(KeyCode.LeftArrow) && clear)
            RotateLeft();

        origin.transform.position = target.transform.position;
    }

    void FixedUpdate()
    {
        if (compassOrigin)
        {
            compassOrigin.transform.rotation = origin.transform.rotation;
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
        clear = false;
        iTween.RotateAdd(origin, iTween.Hash("x", _angle, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", target, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "PostRotate"));
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
        origin.transform.position = target.transform.position;
        isRotating = false;
    }
    #endregion
}

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


    private GameObject origin;
    public GameObject player;

    public bool isRotating = false;

    private bool left, right, up, down;

    [SerializeField]
    private bool clear = true;

    // Use this for initialization
    void Start () {
        origin.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Rotation Behaviour

    public void RotateUp()
    {
        //pass the camera's new fwd
        //BlockManager.instance.DepthShade (origin.transform.up);

        clear = false;
        iTween.RotateAdd(origin, iTween.Hash("x", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
    }

    public void RotateDown()
    {
        //pass the camera's new fwd
        //BlockManager.instance.DepthShade (origin.transform.up * -1);

        clear = false;
        iTween.RotateAdd(origin, iTween.Hash("x", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "ClearToRotate"));
    }

    public void RotateLeft()
    {
        //pass the camera's new fwd
        //BlockManager.instance.DepthShade (origin.transform.right * -1);

        clear = false;
        iTween.RotateAdd(origin, iTween.Hash("y", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
    }

    public void RotateRight()
    {
        //pass the camera's new fwd
        //BlockManager.instance.DepthShade (origin.transform.right);

        clear = false;
        iTween.RotateAdd(origin, iTween.Hash("y", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
    }
    #endregion
}

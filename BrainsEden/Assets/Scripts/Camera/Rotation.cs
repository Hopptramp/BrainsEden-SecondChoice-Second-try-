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

	private GameObject origin;
    public GameObject player;
    [SerializeField]
    private bool useKeyboard;

    private bool left, right, up, down;

	[SerializeField]
	private bool clear = true;

	//set the camera to start facing the origin at cameraDist distance
	void Start ()
	{
		//sanity check for origin
		if (!origin)
		{
			origin = new GameObject ();
			origin.transform.position = Vector3.zero;
			origin.transform.rotation.eulerAngles.Set(0f, 0f, 0f);
		}

		transform.parent = origin.transform;
		transform.position = transform.parent.position - new Vector3 (0f, 0f, cameraDist);

        currentState = GameManager.instance.m_CameraState;
	}
    

	void Update ()
	{
        if (useKeyboard)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow) && clear)
            {
                RotateUp();
            }

            else if (Input.GetKeyUp(KeyCode.DownArrow) && clear)
            {
                RotateDown();
            }

            else if (Input.GetKeyUp(KeyCode.RightArrow) && clear)
            {
                RotateRight();
            }

            else if (Input.GetKeyUp(KeyCode.LeftArrow) && clear)
            {
                RotateLeft();
            }

        }
        else
        {
            if (up && clear)            
               RotateUp();            
                                                      
            else if (down && clear)      
                RotateDown();            

            else if (right && clear)            
               RotateRight();           

            else if (left && clear)
                RotateLeft();
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

    public void RotateUp ()
	{
		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("x", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc,"onstarttarget",player,"onstart", "FreezeUnfreeze", "onstartparams" , true,"oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
	}

	public void RotateDown ()
	{
		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("x", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "ClearToRotate"));
	}

	public void RotateLeft ()
	{
		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("y", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
	}

	public void RotateRight ()
	{
		clear = false;
		iTween.RotateAdd(origin, iTween.Hash("y", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "onstarttarget", player, "onstart", "FreezeUnfreeze", "onstartparams", true, "oncompletetarget", gameObject, "oncomplete", "CheckRotation"));
	}


//    void FixedUpdate()
//    {
//        
//    }

	/// <summary>
	/// Checks the rotation of the camera and rites it if at a stupid orientation
	/// </summary>
	void CheckRotation()
	{
		//if not looking up or down and the camera is off orientation
		if (transform.right == Vector3.up || (-1 * transform.right) == Vector3.up || transform.up == Vector3.down)
		{
			StartCoroutine (CorrectRotation ());
		}
		else
		{
			ClearToRotate ();
		}
	}

	IEnumerator CorrectRotation()
	{
		//pause for a little while?
		//yield return new WaitForSeconds (0.25f)

		//correct the camera rotation
		iTween.RotateTo (origin, iTween.Hash ("z", 0f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc, "oncompletetarget", gameObject, "oncomplete", "ClearToRotate"));

		yield return null;
	}

    void ClearToRotate()
	{
        origin.transform.position = player.transform.position;
        GameManager.instance.UpdateCameraState();
        clear = true;
        player.GetComponent<Player_Movement>().FreezeUnfreeze(false);
	}
}

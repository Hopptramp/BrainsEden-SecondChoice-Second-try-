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

	private GameObject origin;

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
	}

	void Update ()
	{
		if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			iTween.RotateAdd(origin, iTween.Hash("x", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc));
		}

		else if (Input.GetKeyUp(KeyCode.DownArrow))
		{
			iTween.RotateAdd(origin, iTween.Hash("x", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc));
		}

		else if (Input.GetKeyUp(KeyCode.RightArrow))
		{
			iTween.RotateAdd(origin, iTween.Hash("y", -90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc));
		}

		else if (Input.GetKeyUp(KeyCode.LeftArrow))
		{
			iTween.RotateAdd(origin, iTween.Hash("y", 90f, "time", rotateTime, "easetype", iTween.EaseType.easeInOutCirc));
		}
	}
}

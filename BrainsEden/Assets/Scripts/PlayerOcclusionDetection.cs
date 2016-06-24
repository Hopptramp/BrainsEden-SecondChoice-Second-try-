using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerOcclusionDetection : MonoBehaviour
{
	[SerializeField]
	//the points that the raycast will happen from
	private Transform[] raycastPoints;

	[SerializeField]
	private Transform mainCam;

	private List<GameObject> objectsHit = new List<GameObject>();

	void Start ()
	{
		if (!mainCam)
		{
			GameObject.FindGameObjectWithTag ("MainCamera");
		}
	}


	void Update ()
	{
		if (objectsHit.Count != 0)
		{
			foreach (GameObject _obj in objectsHit)
			{
				//alter the shader to half alpha
				Color col = _obj.GetComponent<MeshRenderer> ().material.color;

				col.a = 1f;

				_obj.GetComponent<MeshRenderer> ().material.color = col;
			}

			objectsHit.Clear ();
		}

		foreach (Transform _trans in raycastPoints)
		{
			Ray r = new Ray ();
			r.direction = mainCam.forward * -1;
			r.origin = _trans.position;

			RaycastHit[] hits;
			hits = Physics.RaycastAll (r);

			if (hits.Length != 0)
			{
				for (int i = 0; i < hits.Length; i++)
				{
					if (hits [i].collider)
					{
						if (hits [i].collider.gameObject != gameObject)
						{
							//alter the shader to half alpha
							Color col = hits [i].collider.gameObject.GetComponent<MeshRenderer> ().material.color;

							col.a = 0.5f / hits.Length;

							hits [i].collider.gameObject.GetComponent<MeshRenderer> ().material.color = col;

							objectsHit.Add (hits [i].collider.gameObject);
						}
					}
				}
			}
		}


	}
}

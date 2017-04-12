using UnityEngine;
using System.Collections;

public class RotateTheGoose : MonoBehaviour {

    Rigidbody rb;
    Quaternion rot;
    Vector3 vel2D;

	void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        vel2D = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rot = vel2D.magnitude > 0.1f ? Quaternion.LookRotation(vel2D) : transform.rotation;
        transform.rotation = rot;
	}
}

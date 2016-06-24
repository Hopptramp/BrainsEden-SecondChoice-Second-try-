using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {

    [SerializeField]
    Player_Movement player;

	// Use this for initialization
	void Awake ()
    {
	    if (player == null)
        {
            player = GetComponentInParent<Player_Movement>();
        }
	}    

    void OnTriggerStay(Collider col)
    {
        if (col.tag != "Player")
        {
            player.SetGrounded(true);
        }
    }
}
